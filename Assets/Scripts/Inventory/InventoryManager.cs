using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<ItemData> temporaryInventory = new List<ItemData>();
    public List<ItemData> collectionInventory = new List<ItemData>();

    public int maxTrashCapacity = 5;

    private void Awake()
    {
        Instance = this;
    }

    public void AddItem(ItemData item)
    {
        if (item.itemType == ItemType.Trash)
        {
            if (temporaryInventory.Count < maxTrashCapacity)
            {
                temporaryInventory.Add(item);
                
                // --- PASTE DI SINI UNTUK TRASH ---
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
            
            // --- PASTE DI SINI UNTUK SPECIAL ---
            InventoryUIManager.Instance.UpdateUI(); 

            Debug.Log(item.itemName + " added to Collection Inventory");
            Destroy(item.gameObject);
        }
    }
    
    public void ThrowTrash(ItemData itemName)
    {
        if (temporaryInventory.Contains(itemName))
        {
            temporaryInventory.Remove(itemName);
            Debug.Log("Threw away " + itemName);
        }
    }
}
