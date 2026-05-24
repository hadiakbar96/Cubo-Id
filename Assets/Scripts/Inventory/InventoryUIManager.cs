using UnityEngine;
using UnityEngine.UI; // Wajib ditambahkan untuk memanipulasi UI

public class InventoryUIManager : MonoBehaviour
{
    // Membuat jalan pintas agar script ini bisa dipanggil dari mana saja
    public static InventoryUIManager Instance;

    [Header("UI Slots")]
    public Image[] trashSlots;    // Array untuk 5 kotak bawah
    public Image[] specialSlots;  // Array untuk 3 kotak kanan

    private void Awake()
    {
        Instance = this; // Menginisialisasi jalan pintas
    }

    // Fungsi ini akan dipanggil setiap kali pemain memungut barang
    public void UpdateUI()
    {
        // 1. Mengubah warna UI Trash (Bawah)
        int currentTrashCount = InventoryManager.Instance.temporaryInventory.Count;
        for (int i = 0; i < trashSlots.Length; i++)
        {
            if (i < currentTrashCount)
            {
                // Jika slot terisi, warnai hitam (atau bisa diganti warna lain)
                trashSlots[i].color = Color.black; 
            }
            else
            {
                // Jika kosong, warnai putih
                trashSlots[i].color = Color.white; 
            }
        }

        // 2. Mengubah warna UI Special (Kanan)
        int currentSpecialCount = InventoryManager.Instance.collectionInventory.Count;
        for (int i = 0; i < specialSlots.Length; i++)
        {
            if (i < currentSpecialCount)
            {
                // Jika slot terisi, warnai kuning (emas)
                specialSlots[i].color = Color.yellow; 
            }
            else
            {
                // Jika kosong, warnai putih
                specialSlots[i].color = Color.white; 
            }
        }
    }
}