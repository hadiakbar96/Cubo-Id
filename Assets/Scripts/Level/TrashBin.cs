using UnityEngine;

/// <summary>
/// Tempat sampah untuk membuang cube hasil crafting.
/// Attach ke GameObject yang memiliki BoxCollider2D (Is Trigger = true).
/// </summary>
public class TrashBin : MonoBehaviour
{
    [Header("Bin Settings")]
    public ItemType acceptedType; // Organic = Bio Cube, Anorganic = Metal Cube, Plastic = Plastic Cube

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Cek nama objek yang masuk — cube dari TryCraft punya nama bersih (tanpa "(Clone)")
        string cubeName = other.gameObject.name.Replace("(Clone)", "").Trim();

        Debug.Log("TrashBin " + acceptedType + " mendeteksi: " + cubeName);

        // Cocokkan tipe cube dengan tipe bin
        bool accepted = false;

        switch (acceptedType)
        {
            case ItemType.Organic:
                accepted = cubeName.Contains("Bio");
                break;
            case ItemType.Anorganic:
                accepted = cubeName.Contains("Metal");
                break;
            case ItemType.Plastic:
                accepted = cubeName.Contains("Plastic") && cubeName.Contains("Cube");
                break;
        }

        if (accepted)
        {
            // Cube diterima! Update progress dan hancurkan cube
            if (LevelProgressManager.Instance != null)
            {
                LevelProgressManager.Instance.DepositCube(acceptedType);
            }
            Debug.Log("Cube " + cubeName + " berhasil dibuang ke tempat sampah " + acceptedType + "!");
            Destroy(other.gameObject);
        }
    }
}
