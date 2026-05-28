using System.Collections.Generic;
using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    [Header("Plastic Area (Red Box)")]
    public BoxCollider2D plasticArea;
    public GameObject[] plasticPrefabs;
    public int plasticSpawnCount = 5;

    [Header("Anorganic Area (Blue Box)")]
    public BoxCollider2D anorganicArea;
    public GameObject[] anorganicPrefabs;
    public int anorganicSpawnCount = 5;

    [Header("Organic Area (Green Box)")]
    public BoxCollider2D organicArea;
    public GameObject[] organicPrefabs;
    public int organicSpawnCount = 5;

    [Header("Spacing Settings")]
    [Tooltip("Extra padding (in Unity units) added around each item's collider size. Increase to space items further apart.")]
    public float paddingBetweenItems = 0.3f;
    [Tooltip("Number of placement attempts per item before giving up.")]
    public int maxAttempts = 50;

    void Start()
    {
        SpawnAllAreas();
    }

    public void SpawnAllAreas()
    {
        // Shared list of BOUNDS (not just positions) across all areas.
        // Each Bounds entry represents the footprint of a spawned item.
        List<Bounds> occupiedBounds = new List<Bounds>();

        if (plasticArea != null)
            SpawnArea(plasticArea.bounds, plasticPrefabs, plasticSpawnCount, occupiedBounds);

        if (anorganicArea != null)
            SpawnArea(anorganicArea.bounds, anorganicPrefabs, anorganicSpawnCount, occupiedBounds);

        if (organicArea != null)
            SpawnArea(organicArea.bounds, organicPrefabs, organicSpawnCount, occupiedBounds);
    }

    private void SpawnArea(Bounds areaBounds, GameObject[] prefabs, int count, List<Bounds> occupiedBounds)
    {
        if (prefabs == null || prefabs.Length == 0)
        {
            Debug.LogWarning("[TrashSpawner] No prefabs assigned for this area!");
            return;
        }

        if (count <= 0) return;

        for (int i = 0; i < count; i++)
        {
            GameObject prefabToSpawn = prefabs[Random.Range(0, prefabs.Length)];

            // Read the actual collider size from the prefab itself.
            // This means large gallons and small cups each get the right amount of space.
            Vector2 itemSize = GetPrefabColliderSize(prefabToSpawn);
            Vector2 checkSize = itemSize + Vector2.one * paddingBetweenItems;

            Vector2 spawnPosition = Vector2.zero;
            bool validPositionFound = false;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                // Keep the candidate center away from the area edges by half the item size,
                // so items never partially stick outside the spawn zone.
                float minX = areaBounds.min.x + itemSize.x * 0.5f;
                float maxX = areaBounds.max.x - itemSize.x * 0.5f;
                float minY = areaBounds.min.y + itemSize.y * 0.5f;
                float maxY = areaBounds.max.y - itemSize.y * 0.5f;

                if (minX >= maxX || minY >= maxY)
                {
                    // Area is too small for the item — just spawn at center
                    spawnPosition = areaBounds.center;
                    validPositionFound = true;
                    break;
                }

                float randomX = Random.Range(minX, maxX);
                float randomY = Random.Range(minY, maxY);
                Vector2 candidate = new Vector2(randomX, randomY);

                // Build a Bounds representing this candidate item (with padding).
                Bounds candidateBounds = new Bounds(candidate, checkSize);

                // Check against every already-placed item's bounds.
                bool overlaps = false;
                foreach (Bounds existing in occupiedBounds)
                {
                    if (candidateBounds.Intersects(existing))
                    {
                        overlaps = true;
                        break;
                    }
                }

                if (!overlaps)
                {
                    spawnPosition = candidate;
                    validPositionFound = true;
                    break;
                }
            }

            if (!validPositionFound)
            {
                // All attempts failed — log a warning and skip this item to avoid stacking.
                Debug.LogWarning($"[TrashSpawner] Could not find a free spot for item {i} in area after {maxAttempts} attempts. " +
                                 $"Try reducing spawn count or increasing the spawn area size.");
                continue; // <-- skip instead of forcing a bad spawn
            }

            // Register the ACTUAL item bounds (no padding) so the overlap check is fair.
            occupiedBounds.Add(new Bounds(spawnPosition, itemSize + Vector2.one * paddingBetweenItems));

            // Spawn the item.
            GameObject spawnedItem = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
            spawnedItem.transform.parent = this.transform;
        }
    }

    /// <summary>
    /// Returns the collider size of the given prefab.
    /// Falls back to (1, 1) if no BoxCollider2D is found.
    /// </summary>
    private Vector2 GetPrefabColliderSize(GameObject prefab)
    {
        BoxCollider2D col = prefab.GetComponent<BoxCollider2D>();
        if (col != null)
        {
            // Multiply by the prefab's local scale to get world size
            Vector3 scale = prefab.transform.localScale;
            return new Vector2(col.size.x * Mathf.Abs(scale.x), col.size.y * Mathf.Abs(scale.y));
        }

        // Fallback: try SpriteRenderer bounds
        SpriteRenderer sr = prefab.GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite != null)
        {
            Vector3 scale = prefab.transform.localScale;
            return new Vector2(
                sr.sprite.bounds.size.x * Mathf.Abs(scale.x),
                sr.sprite.bounds.size.y * Mathf.Abs(scale.y)
            );
        }

        return Vector2.one;
    }

    private void OnDrawGizmos()
    {
        if (plasticArea != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
            Gizmos.DrawCube(plasticArea.bounds.center, plasticArea.bounds.size);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(plasticArea.bounds.center, plasticArea.bounds.size);
        }
        if (anorganicArea != null)
        {
            Gizmos.color = new Color(0f, 0f, 1f, 0.2f);
            Gizmos.DrawCube(anorganicArea.bounds.center, anorganicArea.bounds.size);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(anorganicArea.bounds.center, anorganicArea.bounds.size);
        }
        if (organicArea != null)
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
            Gizmos.DrawCube(organicArea.bounds.center, organicArea.bounds.size);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(organicArea.bounds.center, organicArea.bounds.size);
        }
    }
}
