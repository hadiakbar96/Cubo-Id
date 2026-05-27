using System.Collections; // Wajib untuk fitur jeda waktu (IEnumerator)
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager Instance;

    [Header("UI Slots")]
    public Image[] trashSlots;
    public Image[] specialSlots;

    [Header("Slot Sprites (dari Artist)")]
    public Sprite slotDefault;    // Trash bg_0 - Abu-abu (slot kosong)
    public Sprite slotOrganic;    // Trash bg_1 - Hijau (sampah organic)
    public Sprite slotMetal;      // Trash bg_2 - Krim (sampah metal/anorganic)
    public Sprite slotPlastic;    // Trash bg_3 - Biru (sampah plastic)

    [Header("Craft Button")]
    public Image craftButton;          // Image komponen tombol craft (di-assign dari Inspector)
    public Sprite craftButtonDefault;  // Assets v3_16 - tombol nonaktif (abu-abu)
    public Sprite craftButtonActive;   // Assets v3_13 - tombol aktif (biru/menyala)
    public Sprite bioCubeIcon;         // Sprite ikon Bio Cube (overlay di atas button)
    public Sprite metalCubeIcon;       // Sprite ikon Metal Cube
    public Sprite plasticCubeIcon;     // Sprite ikon Plastic Cube

    [Header("Animasi Error")]
    // Menyimpan referensi panel pembungkus kotaknya agar bisa digoyang bersamaan
    public RectTransform trashPanel; 

    private Sprite defaultSpecialSprite;

    // Array untuk menyimpan ikon item yang muncul di atas setiap slot
    private Image[] trashIconImages;

    // Ikon overlay cube di atas craft button
    private Image craftButtonOverlay;

    private void Awake()
    {
        Instance = this; 
        if (specialSlots.Length > 0 && specialSlots[0] != null) defaultSpecialSprite = specialSlots[0].sprite;

        // Otomatis atur slotIndex untuk setiap slot agar tidak perlu diatur manual di Inspector
        for (int i = 0; i < trashSlots.Length; i++)
        {
            InventorySlotUI slotUI = trashSlots[i].GetComponent<InventorySlotUI>();
            if (slotUI != null)
            {
                slotUI.slotIndex = i;
            }
        }

        // Buat child Image untuk menampilkan ikon item di setiap slot
        trashIconImages = new Image[trashSlots.Length];
        for (int i = 0; i < trashSlots.Length; i++)
        {
            GameObject iconObj = new GameObject("ItemIcon");
            iconObj.transform.SetParent(trashSlots[i].transform, false);

            Image iconImage = iconObj.AddComponent<Image>();
            iconImage.raycastTarget = false; // Agar tidak menghalangi drag pada slot
            iconImage.preserveAspect = true; // Menjaga rasio aspek sprite
            iconImage.color = Color.clear;   // Sembunyikan dulu (transparan)

            // Atur ukuran ikon: 70% dari ukuran slot
            RectTransform iconRect = iconObj.GetComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.15f, 0.15f);
            iconRect.anchorMax = new Vector2(0.85f, 0.85f);
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;

            trashIconImages[i] = iconImage;
        }

        // Buat overlay Image di atas craft button untuk menampilkan ikon cube
        if (craftButton != null)
        {
            GameObject overlayObj = new GameObject("CubeOverlay");
            overlayObj.transform.SetParent(craftButton.transform, false);

            craftButtonOverlay = overlayObj.AddComponent<Image>();
            craftButtonOverlay.raycastTarget = false;
            craftButtonOverlay.preserveAspect = true;
            craftButtonOverlay.color = Color.clear; // Sembunyikan dulu

            // Atur ukuran overlay: 70% dari ukuran button
            RectTransform overlayRect = overlayObj.GetComponent<RectTransform>();
            overlayRect.anchorMin = new Vector2(0.15f, 0.15f);
            overlayRect.anchorMax = new Vector2(0.85f, 0.85f);
            overlayRect.offsetMin = Vector2.zero;
            overlayRect.offsetMax = Vector2.zero;

            // Tambahkan Button component dan hubungkan ke fungsi craft
            Button btn = craftButton.GetComponent<Button>();
            if (btn == null) btn = craftButton.gameObject.AddComponent<Button>();
            btn.onClick.AddListener(OnCraftButtonClicked);
        }
    }

    public void UpdateUI()
    {
        int currentTrashCount = InventoryManager.Instance.temporaryInventory.Count;
        for (int i = 0; i < trashSlots.Length; i++)
        {
            if (i < currentTrashCount)
            {
                ItemInfo item = InventoryManager.Instance.temporaryInventory[i];
                ItemType type = item.itemType;

                // Ganti sprite background sesuai tipe sampah (menggunakan asset dari artist)
                if (type == ItemType.Organic) trashSlots[i].sprite = slotOrganic;
                else if (type == ItemType.Anorganic) trashSlots[i].sprite = slotMetal;
                else if (type == ItemType.Plastic) trashSlots[i].sprite = slotPlastic;

                trashSlots[i].color = Color.white; // Selalu putih agar sprite terlihat asli

                // Tampilkan ikon item di atas slot
                if (item.itemIcon != null && trashIconImages[i] != null)
                {
                    trashIconImages[i].sprite = item.itemIcon;
                    trashIconImages[i].color = Color.white; // Tampilkan
                }
                else if (trashIconImages[i] != null)
                {
                    trashIconImages[i].color = Color.clear; // Sembunyikan jika tidak ada ikon
                }
            }
            else
            {
                // Kembalikan ke sprite default (abu-abu) saat kosong
                trashSlots[i].sprite = slotDefault;
                trashSlots[i].color = Color.white;

                // Sembunyikan ikon
                if (trashIconImages[i] != null)
                {
                    trashIconImages[i].sprite = null;
                    trashIconImages[i].color = Color.clear;
                }
            }
        }

        int currentSpecialCount = InventoryManager.Instance.collectionInventory.Count;
        for (int i = 0; i < specialSlots.Length; i++)
        {
            if (specialSlots[i] == null) continue; // Lewati jika slot belum di-assign

            if (i < currentSpecialCount)
            {
                ItemInfo item = InventoryManager.Instance.collectionInventory[i];
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
        // --- UPDATE CRAFT BUTTON ---
        UpdateCraftButton();
    }

    /// <summary>
    /// Cek apakah 5 slot inventory berisi tipe sampah yang sama, lalu update tampilan craft button.
    /// </summary>
    private void UpdateCraftButton()
    {
        if (craftButton == null) return;

        var inventory = InventoryManager.Instance.temporaryInventory;
        bool canCraft = false;
        ItemType craftType = ItemType.Organic; // default

        // Cek apakah penuh (5 item) dan semua tipe sama
        if (inventory.Count >= InventoryManager.Instance.maxTrashCapacity)
        {
            craftType = inventory[0].itemType;
            canCraft = inventory.All(item => item.itemType == craftType);
        }

        if (canCraft)
        {
            // Aktifkan: ganti ke sprite v3_13 dan tampilkan overlay cube
            craftButton.sprite = craftButtonActive;
            craftButton.color = Color.white;

            if (craftButtonOverlay != null)
            {
                Sprite cubeIcon = null;
                if (craftType == ItemType.Organic) cubeIcon = bioCubeIcon;
                else if (craftType == ItemType.Anorganic) cubeIcon = metalCubeIcon;
                else if (craftType == ItemType.Plastic) cubeIcon = plasticCubeIcon;

                if (cubeIcon != null)
                {
                    craftButtonOverlay.sprite = cubeIcon;
                    craftButtonOverlay.color = Color.white;
                }
                else
                {
                    craftButtonOverlay.color = Color.clear;
                }
            }
        }
        else
        {
            // Nonaktifkan: ganti ke sprite v3_16 default dan sembunyikan overlay
            craftButton.sprite = craftButtonDefault;
            craftButton.color = Color.white;

            if (craftButtonOverlay != null)
            {
                craftButtonOverlay.sprite = null;
                craftButtonOverlay.color = Color.clear;
            }
        }
    }

    /// <summary>
    /// Dipanggil saat craft button ditekan.
    /// </summary>
    public void OnCraftButtonClicked()
    {
        InventoryManager.Instance.TryCraft();
    }

    // --- FUNGSI UNTUK ERROR CRAFTING ---
    public void TriggerErrorAnimation()
    {
        StartCoroutine(ErrorRoutine());
    }

    private IEnumerator ErrorRoutine()
    {
        // 1. Simpan posisi asli panel
        Vector2 originalPos = trashPanel.anchoredPosition;

        // 2. Beri tint merah muda ringan pada semua slot (agar sprite tetap terlihat)
        foreach (Image slot in trashSlots)
        {
            slot.color = new Color(1f, 0.5f, 0.5f, 1f);
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

        // 5. Kembalikan sprite dan warnanya ke tampilan normal
        UpdateUI();
    }
}