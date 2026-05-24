using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Wajib untuk membaca keyboard

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

    [Header("Referensi Crafting (Hasil Cube)")]
    public GameObject bioCubePrefab;
    public GameObject metalCubePrefab;
    public GameObject plasticCubePrefab;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        // Mendeteksi tombol F ditekan (Sistem Input Baru Unity)
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            TryCraft();
        }
    }

    public void AddItem(ItemData item)
    {
        bool isTrash = (item.itemType == ItemType.Organic || 
                        item.itemType == ItemType.Anorganic || 
                        item.itemType == ItemType.Plastic);

        if (isTrash)
        {
            if (temporaryInventory.Count < maxTrashCapacity)
            {
                temporaryInventory.Add(item);
                InventoryUIManager.Instance.UpdateUI();
                Destroy(item.gameObject);
            } 
            else Debug.Log("Kantong Penuh!");
        }
        else if (item.itemType == ItemType.Special)
        {
            collectionInventory.Add(item);
            InventoryUIManager.Instance.UpdateUI();
            Destroy(item.gameObject);
        }
    }

    public void DropItemFromSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < temporaryInventory.Count)
        {
            ItemType droppedType = temporaryInventory[slotIndex].itemType;
            temporaryInventory.RemoveAt(slotIndex);

            GameObject prefabToDrop = null;
            if (droppedType == ItemType.Organic) prefabToDrop = organicPrefab;
            else if (droppedType == ItemType.Anorganic) prefabToDrop = anorganicPrefab;
            else if (droppedType == ItemType.Plastic) prefabToDrop = plasticPrefab;

            if (prefabToDrop != null) 
            {
                Instantiate(prefabToDrop, playerTransform.position, Quaternion.identity);
            }

            InventoryUIManager.Instance.UpdateUI();
        }
    }

    // --- FUNGSI BARU UNTUK CRAFTING ---
    public void TryCraft()
    {
        // Syarat 1: Kantong harus terisi penuh (5 item)
        if (temporaryInventory.Count < maxTrashCapacity)
        {
            InventoryUIManager.Instance.TriggerErrorAnimation();
            return;
        }

        // Syarat 2: Semua jenis barang harus persis sama
        ItemType firstItemType = temporaryInventory[0].itemType;
        bool isAllSame = true;

        for (int i = 1; i < temporaryInventory.Count; i++)
        {
            if (temporaryInventory[i].itemType != firstItemType)
            {
                isAllSame = false;
                break;
            }
        }

        // Jika ada yang beda, gagalkan dan goyangkan UI
        if (!isAllSame)
        {
            InventoryUIManager.Instance.TriggerErrorAnimation();
            return;
        }

        // Jika lulus ujian, kita rakit Cube-nya!
        GameObject cubeToCraft = null;
        if (firstItemType == ItemType.Organic) cubeToCraft = bioCubePrefab;
        else if (firstItemType == ItemType.Anorganic) cubeToCraft = metalCubePrefab;
        else if (firstItemType == ItemType.Plastic) cubeToCraft = plasticCubePrefab;

        // Kosongkan kantong dari sampah lama
        temporaryInventory.Clear();
        InventoryUIManager.Instance.UpdateUI();

        // Jatuhkan Cube hasil rakitan di depan karakter!
        if (cubeToCraft != null)
        {
            Instantiate(cubeToCraft, playerTransform.position, Quaternion.identity);
            Debug.Log("Sukses membuat " + cubeToCraft.name + "!");
        }
    }
}