using UnityEngine;

public class ItemData : MonoBehaviour
{
    public string itemName;
    public ItemType itemType;
    public Sprite itemIcon;         // Ikon gambar untuk UI slot
    public GameObject sourcePrefab; // Prefab asli item ini (untuk di-spawn ulang saat drop)
}
