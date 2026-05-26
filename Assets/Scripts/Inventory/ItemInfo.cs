using UnityEngine;

// Data class untuk menyimpan info item setelah GameObject aslinya di-destroy.
// Berbeda dari ItemData yang merupakan MonoBehaviour (terikat ke GameObject).
[System.Serializable]
public class ItemInfo
{
    public string itemName;
    public ItemType itemType;
    public Sprite itemIcon;
    public GameObject sourcePrefab; // Prefab asli untuk di-spawn ulang saat drop

    public ItemInfo(ItemData source)
    {
        itemName = source.itemName;
        itemType = source.itemType;
        itemIcon = source.itemIcon;
        sourcePrefab = source.sourcePrefab;

        // Jika itemIcon belum diisi manual di Inspector,
        // coba ambil dari SpriteRenderer di GameObject yang sama
        if (itemIcon == null)
        {
            SpriteRenderer sr = source.GetComponent<SpriteRenderer>();
            if (sr != null) itemIcon = sr.sprite;
        }
    }
}
