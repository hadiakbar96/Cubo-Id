using UnityEngine;
using TMPro;

/// <summary>
/// Mengatur todo list progress cube.
/// Attach ke GameObject kosong di scene, lalu assign UI Text dari Inspector.
/// </summary>
public class LevelProgressManager : MonoBehaviour
{
    public static LevelProgressManager Instance;

    [Header("Required Cubes Per Type")]
    public int bioCubeRequired = 2;
    public int metalCubeRequired = 2;
    public int plasticCubeRequired = 2;

    [Header("Todo List UI (TextMeshPro)")]
    public TextMeshProUGUI bioText;
    public TextMeshProUGUI metalText;
    public TextMeshProUGUI plasticText;

    // Counter internal
    private int bioCubeDeposited = 0;
    private int metalCubeDeposited = 0;
    private int plasticCubeDeposited = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateTodoUI();
    }

    /// <summary>
    /// Dipanggil oleh TrashBin saat cube berhasil dibuang.
    /// </summary>
    public void DepositCube(ItemType cubeType)
    {
        switch (cubeType)
        {
            case ItemType.Organic:
                if (bioCubeDeposited < bioCubeRequired) bioCubeDeposited++;
                break;
            case ItemType.Anorganic:
                if (metalCubeDeposited < metalCubeRequired) metalCubeDeposited++;
                break;
            case ItemType.Plastic:
                if (plasticCubeDeposited < plasticCubeRequired) plasticCubeDeposited++;
                break;
        }

        UpdateTodoUI();
    }

    private void UpdateTodoUI()
    {
        // [ ] = belum selesai, [x] = sudah selesai
        if (bioText != null)
        {
            bool done = bioCubeDeposited >= bioCubeRequired;
            string check = done ? "[x]" : "[ ]";
            bioText.text = check + " Bio Cube " + bioCubeDeposited + "/" + bioCubeRequired;
            bioText.color = done ? new Color(0.2f, 0.8f, 0.2f) : Color.white;
        }

        if (metalText != null)
        {
            bool done = metalCubeDeposited >= metalCubeRequired;
            string check = done ? "[x]" : "[ ]";
            metalText.text = check + " Metal Cube " + metalCubeDeposited + "/" + metalCubeRequired;
            metalText.color = done ? new Color(0.2f, 0.8f, 0.2f) : Color.white;
        }

        if (plasticText != null)
        {
            bool done = plasticCubeDeposited >= plasticCubeRequired;
            string check = done ? "[x]" : "[ ]";
            plasticText.text = check + " Plastic Cube " + plasticCubeDeposited + "/" + plasticCubeRequired;
            plasticText.color = done ? new Color(0.2f, 0.8f, 0.2f) : Color.white;
        }
    }
}
