using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<ItemData> temporaryInventory = new List<ItemData>();
    public List<ItemData> collectionInventory = new List<ItemData>();

    public int maxTrashCapacity = 5;

    [Header("Referensi Drop")]
    public GameObject trashPrefab;    // Menyimpan wujud asli sampah
    public Transform playerTransform; // Posisi tempat jatuhnya sampah

    private void Awake()
    {
        Instance = this;
    }

    public void AddItem(ItemData item)
    {
        if (item.itemType == ItemType.Trash)
        {
            if (temporaryInventory.Count < maxTrashCapacity)
            {
                temporaryInventory.Add(item);
                InventoryUIManager.Instance.UpdateUI(); // Update UI langsung

                Debug.Log(item.itemName + " added to Temporary Inventory");
                Destroy(item.gameObject);
            } 
            else
            {
                Debug.Log("Trash Inventory is full!");
            }
        }
        else if (item.itemType == ItemType.Special)
        {
            collectionInventory.Add(item);
            InventoryUIManager.Instance.UpdateUI(); // Update UI langsung

            Debug.Log(item.itemName + " added to Collection Inventory");
            Destroy(item.gameObject);
        }
    }
    
    // Fungsi lama bawaan dari prototype sebelumnya (bisa diabaikan)
    public void ThrowTrash(ItemData itemName)
    {
        if (temporaryInventory.Contains(itemName))
        {
            temporaryInventory.Remove(itemName);
            Debug.Log("Threw away " + itemName);
        }
    }

    // --- FUNGSI BARU UNTUK DRAG & DROP ---
    public void DropItemFromSlot(int slotIndex)
    {
        // Mengecek apakah kotak yang di-drag benar-benar ada isinya
        if (slotIndex >= 0 && slotIndex < temporaryInventory.Count)
        {
            // 1. Hapus data dari kantong penyimpanan
            temporaryInventory.RemoveAt(slotIndex);

            // 2. Cetak ulang benda fisik di posisi karakter pemain
            Instantiate(trashPrefab, playerTransform.position, Quaternion.identity);

            // 3. Segarkan (Update) layar UI agar kotaknya kembali putih
            InventoryUIManager.Instance.UpdateUI();
        }
    }
}