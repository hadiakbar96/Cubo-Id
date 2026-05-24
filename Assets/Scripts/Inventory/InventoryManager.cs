using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<ItemData> temporaryInventory = new List<ItemData>();
    public List<ItemData> collectionInventory = new List<ItemData>();

    public int maxTrashCapacity = 5;

    [Header("Referensi Drop")]
    public GameObject organicPrefab;
    public GameObject anorganicPrefab;
    public GameObject plasticPrefab;
    public Transform playerTransform;

    private void Awake()
    {
        Instance = this;
    }

    public void AddItem(ItemData item)
    {
        // Mengecek apakah barang ini termasuk salah satu jenis sampah
        bool isTrash = (item.itemType == ItemType.Organic || 
                        item.itemType == ItemType.Anorganic || 
                        item.itemType == ItemType.Plastic);

        if (isTrash)
        {
            if (temporaryInventory.Count < maxTrashCapacity)
            {
                temporaryInventory.Add(item);
                InventoryUIManager.Instance.UpdateUI();

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
            InventoryUIManager.Instance.UpdateUI();

            Debug.Log(item.itemName + " added to Collection Inventory");
            Destroy(item.gameObject);
        }
    }

    public void DropItemFromSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < temporaryInventory.Count)
        {
            // Ambil data tipe barang yang mau dibuang
            ItemType droppedType = temporaryInventory[slotIndex].itemType;
            
            // Hapus dari daftar
            temporaryInventory.RemoveAt(slotIndex);

            // Tentukan wujud (Prefab) mana yang akan jatuh
            GameObject prefabToDrop = null;
            if (droppedType == ItemType.Organic) prefabToDrop = organicPrefab;
            else if (droppedType == ItemType.Anorganic) prefabToDrop = anorganicPrefab;
            else if (droppedType == ItemType.Plastic) prefabToDrop = plasticPrefab;

            // Cetak fisiknya ke tanah
            if (prefabToDrop != null) 
            {
                Instantiate(prefabToDrop, playerTransform.position, Quaternion.identity);
            }

            InventoryUIManager.Instance.UpdateUI();
        }
    }
}