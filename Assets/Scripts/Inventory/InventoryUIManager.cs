using UnityEngine;
using UnityEngine.UI;

public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager Instance;

    [Header("UI Slots")]
    public Image[] trashSlots;    // Ukuran: 5
    public Image[] specialSlots;  // Ukuran: 3

    private void Awake()
    {
        Instance = this; 
    }

    public void UpdateUI()
    {
        // 1. Update Trash Slots (Kantong Bawah)
        int currentTrashCount = InventoryManager.Instance.temporaryInventory.Count;
        for (int i = 0; i < trashSlots.Length; i++)
        {
            if (i < currentTrashCount)
            {
                // Ambil data tipe barang di kotak ini
                ItemType type = InventoryManager.Instance.temporaryInventory[i].itemType;

                // Warnai sesuai tipe
                if (type == ItemType.Organic)
                    trashSlots[i].color = Color.green; // Hijau
                else if (type == ItemType.Anorganic)
                    trashSlots[i].color = Color.gray;  // Abu-abu
                else if (type == ItemType.Plastic)
                    trashSlots[i].color = Color.blue;  // Biru
            }
            else
            {
                trashSlots[i].color = Color.white; // Kosong = Putih
            }
        }

        // 2. Update Special Slots (Kantong Kanan)
        int currentSpecialCount = InventoryManager.Instance.collectionInventory.Count;
        for (int i = 0; i < specialSlots.Length; i++)
        {
            if (i < currentSpecialCount)
            {
                specialSlots[i].color = Color.yellow; // Special = Kuning
            }
            else
            {
                specialSlots[i].color = Color.white;
            }
        }
    }
}