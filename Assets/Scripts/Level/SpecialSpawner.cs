using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns Special items randomly inside a custom-shaped area defined by a PolygonCollider2D.
///
/// SETUP STEPS:
///   1. On your "SpecialSpawn" GameObject, REMOVE the EdgeCollider2D.
///   2. ADD a PolygonCollider2D component instead.
///   3. In the PolygonCollider2D Inspector, click "Edit Collider" and drag the
///      vertices to match your custom shape.
///   4. Tick "Is Trigger" on the PolygonCollider2D (so it doesn't block movement).
///   5. Assign this script + the SpecialItem prefabs in the Inspector below.
/// </summary>
public class SpecialSpawner : MonoBehaviour
{
    [Header("Spawn Zone")]
    [Tooltip("The PolygonCollider2D that defines the custom shape of the spawn area. " +
             "Must be on this GameObject or assigned manually.")]
    public PolygonCollider2D spawnZone;

    [Header("Special Item Prefabs")]
    [Tooltip("Assign all Special item prefabs here. One will be chosen randomly per spawn.")]
    public GameObject[] specialItemPrefabs;

    [Header("Spawn Settings")]
    [Tooltip("How many Special items to spawn.")]
    public int spawnCount = 5;
    [Tooltip("Extra gap (Unity units) added around each item's collider to prevent visual overlap.")]
    public float paddingBetweenItems = 0.3f;
    [Tooltip("How many random candidate positions to try per item before giving up.")]
    public int maxAttempts = 100;

    void Start()
    {
        // Auto-find the PolygonCollider2D on this GameObject if not assigned
        if (spawnZone == null)
            spawnZone = GetComponent<PolygonCollider2D>();

        if (spawnZone == null)
        {
            Debug.LogError("[SpecialSpawner] No PolygonCollider2D found! " +
                           "Please add a PolygonCollider2D to this GameObject or assign it in the Inspector.");
            return;
        }

        SpawnSpecialItems();
    }

    public void SpawnSpecialItems()
    {
        if (specialItemPrefabs == null || specialItemPrefabs.Length == 0)
        {
            Debug.LogWarning("[SpecialSpawner] No Special item prefabs assigned!");
            return;
        }

        // Get the axis-aligned bounding box of the polygon.
        // We will randomly sample points from this box and then
        // test whether each point actually falls inside the polygon shape.
        Bounds zoneBounds = spawnZone.bounds;

        // Track the real footprint of every placed item to prevent stacking.
        List<Bounds> occupiedBounds = new List<Bounds>();

        int spawned = 0;
        for (int i = 0; i < spawnCount; i++)
        {
            GameObject prefabToSpawn = specialItemPrefabs[Random.Range(0, specialItemPrefabs.Length)];
            Vector2 itemSize = GetPrefabColliderSize(prefabToSpawn);
            Vector2 checkSize = itemSize + Vector2.one * paddingBetweenItems;

            Vector2 spawnPosition = Vector2.zero;
            bool validPositionFound = false;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                // 1. Pick a random point inside the bounding rectangle.
                float rx = Random.Range(zoneBounds.min.x, zoneBounds.max.x);
                float ry = Random.Range(zoneBounds.min.y, zoneBounds.max.y);
                Vector2 candidate = new Vector2(rx, ry);

                // 2. KEY CHECK: Is this point actually inside the polygon shape?
                //    OverlapPoint returns true only if the point is inside the PolygonCollider2D.
                //    This is what makes custom shapes work correctly.
                if (!spawnZone.OverlapPoint(candidate))
                    continue; // Outside the polygon shape, try again

                // 3. Check that this item's footprint doesn't overlap any already-placed item.
                Bounds candidateBounds = new Bounds(candidate, checkSize);
                bool overlaps = false;
                foreach (Bounds existing in occupiedBounds)
                {
                    if (candidateBounds.Intersects(existing))
                    {
                        overlaps = true;
                        break;
                    }
                }

                if (overlaps) continue;

                // Valid position found!
                spawnPosition = candidate;
                validPositionFound = true;
                break;
            }

            if (!validPositionFound)
            {
                Debug.LogWarning($"[SpecialSpawner] Could not place Special item {i + 1}/{spawnCount} " +
                                 $"after {maxAttempts} attempts. " +
                                 $"Try: reducing spawn count, enlarging the polygon area, or reducing paddingBetweenItems.");
                continue;
            }

            // Register this item's footprint.
            occupiedBounds.Add(new Bounds(spawnPosition, checkSize));

            // Spawn the item.
            GameObject spawned2 = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
            spawned2.transform.parent = this.transform;
            spawned++;
        }

        Debug.Log($"[SpecialSpawner] Spawned {spawned}/{spawnCount} Special items.");
    }

    /// <summary>
    /// Returns the world-space collider size of a prefab.
    /// Falls back to SpriteRenderer bounds, then (1,1).
    /// </summary>
    private Vector2 GetPrefabColliderSize(GameObject prefab)
    {
        BoxCollider2D col = prefab.GetComponent<BoxCollider2D>();
        if (col != null)
        {
            Vector3 scale = prefab.transform.localScale;
            return new Vector2(col.size.x * Mathf.Abs(scale.x), col.size.y * Mathf.Abs(scale.y));
        }

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

    /// <summary>
    /// Draws the spawn zone outline in the Scene view (Editor only).
    /// </summary>
    private void OnDrawGizmos()
    {
        if (spawnZone == null)
            spawnZone = GetComponent<PolygonCollider2D>();

        if (spawnZone == null) return;

        Gizmos.color = new Color(1f, 0.85f, 0f, 0.25f); // Semi-transparent yellow fill
        Vector2[] points = spawnZone.points;
        if (points == null || points.Length < 2) return;

        // Draw filled-ish polygon by connecting all edges
        for (int j = 0; j < points.Length; j++)
        {
            Vector3 a = transform.TransformPoint(points[j]);
            Vector3 b = transform.TransformPoint(points[(j + 1) % points.Length]);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(a, b);

            // Draw a small dot at each vertex
            Gizmos.color = new Color(1f, 0.85f, 0f, 0.8f);
            Gizmos.DrawSphere(a, 0.07f);
        }
    }
}
