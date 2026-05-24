using System.Collections; // Wajib untuk fitur jeda waktu (IEnumerator)
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager Instance;

    [Header("UI Slots")]
    public Image[] trashSlots;
    public Image[] specialSlots;

    [Header("Animasi Error")]
    // Menyimpan referensi panel pembungkus kotaknya agar bisa digoyang bersamaan
    public RectTransform trashPanel; 

    private Sprite defaultTrashSprite;
    private Sprite defaultSpecialSprite;

    private void Awake()
    {
        Instance = this; 
        if (trashSlots.Length > 0 && trashSlots[0] != null) defaultTrashSprite = trashSlots[0].sprite;
        if (specialSlots.Length > 0 && specialSlots[0] != null) defaultSpecialSprite = specialSlots[0].sprite;
    }

    public void UpdateUI()
    {
        int currentTrashCount = InventoryManager.Instance.temporaryInventory.Count;
        for (int i = 0; i < trashSlots.Length; i++)
        {
            if (i < currentTrashCount)
            {
                ItemData item = InventoryManager.Instance.temporaryInventory[i];
                ItemType type = item.itemType;

                // Jika ada ikon custom, tampilkan gambarnya secara penuh
                if (item.itemIcon != null)
                {
                    trashSlots[i].sprite = item.itemIcon;
                    trashSlots[i].color = Color.white;
                }
                else
                {
                    // Fallback: Warnai kotak default sesuai tipe sampah
                    trashSlots[i].sprite = defaultTrashSprite;
                    if (type == ItemType.Organic) trashSlots[i].color = Color.green;
                    else if (type == ItemType.Anorganic) trashSlots[i].color = Color.gray;
                    else if (type == ItemType.Plastic) trashSlots[i].color = Color.blue;
                }
            }
            else
            {
                // Kembalikan ke tampilan default saat kosong
                trashSlots[i].sprite = defaultTrashSprite;
                trashSlots[i].color = Color.white;
            }
        }

        int currentSpecialCount = InventoryManager.Instance.collectionInventory.Count;
        for (int i = 0; i < specialSlots.Length; i++)
        {
            if (i < currentSpecialCount)
            {
                ItemData item = InventoryManager.Instance.collectionInventory[i];
                if (item.itemIcon != null)
                {
                    specialSlots[i].sprite = item.itemIcon;
                    specialSlots[i].color = Color.white;
                }
                else
                {
                    specialSlots[i].sprite = defaultSpecialSprite;
                    specialSlots[i].color = Color.yellow;
                }
            }
            else
            {
                specialSlots[i].sprite = defaultSpecialSprite;
                specialSlots[i].color = Color.white;
            }
        }
    }

    // --- FUNGSI BARU UNTUK ERROR CRAFTING ---
    public void TriggerErrorAnimation()
    {
        StartCoroutine(ErrorRoutine());
    }

    private IEnumerator ErrorRoutine()
    {
        // 1. Simpan posisi asli panel
        Vector2 originalPos = trashPanel.anchoredPosition;

        // 2. Ubah warna semua kotak menjadi merah seketika
        foreach (Image slot in trashSlots)
        {
            slot.color = Color.red;
        }

        // 3. Efek Goyang (Shake) ke kiri dan kanan dengan cepat
        float elapsed = 0f;
        float duration = 0.3f; // Durasi goyang 0.3 detik
        float magnitude = 15f; // Jarak goyang (15 pixel)

        while (elapsed < duration)
        {
            float xOffset = Random.Range(-1f, 1f) * magnitude;
            trashPanel.anchoredPosition = new Vector2(originalPos.x + xOffset, originalPos.y);
            
            elapsed += Time.deltaTime;
            yield return null; // Tunggu 1 frame
        }

        // 4. Setelah selesai, kembalikan posisi ke semula
        trashPanel.anchoredPosition = originalPos;

        // 5. Kembalikan warnanya ke warna aslinya (hijau/biru/putih)
        UpdateUI();
    }
}