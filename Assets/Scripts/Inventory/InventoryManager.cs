using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Wajib untuk membaca keyboard

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    // Menggunakan ItemInfo (bukan ItemData) agar data tetap ada setelah GameObject di-destroy
    public List<ItemInfo> temporaryInventory = new List<ItemInfo>();
    public List<ItemInfo> collectionInventory = new List<ItemInfo>();

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

        // Salin data item ke ItemInfo SEBELUM destroy, agar sprite icon tetap tersimpan
        ItemInfo info = new ItemInfo(item);

        if (isTrash)
        {
            if (temporaryInventory.Count < maxTrashCapacity)
            {
                temporaryInventory.Add(info);
                InventoryUIManager.Instance.UpdateUI();
                Destroy(item.gameObject);
            } 
            else Debug.Log("Kantong Penuh!");
        }
        else if (item.itemType == ItemType.Special)
        {
            collectionInventory.Add(info);
            InventoryUIManager.Instance.UpdateUI();
            Destroy(item.gameObject);
        }
    }

    public void DropItemFromSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < temporaryInventory.Count)
        {
            ItemInfo droppedItem = temporaryInventory[slotIndex];
            temporaryInventory.RemoveAt(slotIndex);

            GameObject prefabToDrop = null;

            // Prioritaskan sourcePrefab (prefab asli item)
            if (droppedItem.sourcePrefab != null)
            {
                prefabToDrop = droppedItem.sourcePrefab;
            }
            else
            {
                // Fallback: gunakan prefab generik berdasarkan tipe
                if (droppedItem.itemType == ItemType.Organic) prefabToDrop = organicPrefab;
                else if (droppedItem.itemType == ItemType.Anorganic) prefabToDrop = anorganicPrefab;
                else if (droppedItem.itemType == ItemType.Plastic) prefabToDrop = plasticPrefab;
            }

            if (prefabToDrop != null) 
            {
                GameObject dropped = Instantiate(prefabToDrop, playerTransform.position, Quaternion.identity);

                // Pastikan sprite dan data item sesuai dengan yang dipickup
                SpriteRenderer sr = dropped.GetComponent<SpriteRenderer>();
                if (sr != null && droppedItem.itemIcon != null)
                {
                    sr.sprite = droppedItem.itemIcon;
                }

                ItemData itemData = dropped.GetComponent<ItemData>();
                if (itemData != null)
                {
                    itemData.itemName = droppedItem.itemName;
                    itemData.itemType = droppedItem.itemType;
                    itemData.itemIcon = droppedItem.itemIcon;
                    itemData.sourcePrefab = droppedItem.sourcePrefab;
                }
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
            // Spawn cube sedikit di depan player (berdasarkan arah terakhir)
            Vector3 spawnPos = playerTransform.position + new Vector3(1.5f, 0f, 0f);
            GameObject cubeInstance = Instantiate(cubeToCraft, spawnPos, Quaternion.identity);

            // Bersihkan nama agar tidak ada "(Clone)" — penting untuk deteksi TrashBin
            cubeInstance.name = cubeToCraft.name;

            // Cube hanya bergerak saat ditarik/didorong (Kinematic = tidak terpengaruh tabrakan biasa)
            Rigidbody2D rb = cubeInstance.GetComponent<Rigidbody2D>();
            if (rb == null) rb = cubeInstance.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            // Set layer ke Pushable agar PlayerController bisa deteksi
            int pushableLayer = LayerMask.NameToLayer("Pushable");
            if (pushableLayer != -1) cubeInstance.layer = pushableLayer;

            // Pastikan ada collider untuk fisika push + trigger bin
            if (cubeInstance.GetComponent<BoxCollider2D>() == null)
            {
                cubeInstance.AddComponent<BoxCollider2D>();
            }

            Debug.Log("Sukses membuat " + cubeToCraft.name + "!");
        }
    }
}