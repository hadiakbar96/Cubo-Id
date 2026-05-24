using UnityEngine;
using UnityEngine.EventSystems; // Wajib untuk interaksi UI (Mouse)
using UnityEngine.UI;

// Perhatikan tambahan tulisan IDragHandler dan IEndDragHandler
public class InventorySlotUI : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public int slotIndex; // Nomor urut kotak (0, 1, 2, 3, 4)

    // Fungsi ini WAJIB ada meskipun kosong, agar OnEndDrag bisa bekerja
    public void OnDrag(PointerEventData eventData)
    {
        // Anda bisa memasang efek ikon mengikuti mouse di sini nanti
    }

    // Fungsi ini otomatis terpanggil saat Anda melepas klik (Drop)
    public void OnEndDrag(PointerEventData eventData)
    {
        Image kotakImage = GetComponent<Image>();
        
        // Kita cek: Apakah kotak ini terisi? (Warna hitam)
        if (kotakImage.color == Color.black) 
        {
            // Panggil manager untuk membuang barang sesuai nomor urut kotak ini
            InventoryManager.Instance.DropItemFromSlot(slotIndex);
        }
    }
}