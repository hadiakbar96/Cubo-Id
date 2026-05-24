using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<string> temporaryTrashInventory = new List<string>();
    public List<string> permanentSpecialInventory = new List<string>();

    public int maxTrashCapacity = 5;

    public void CollectItem(CollectibleItem item)
    {
        if (item.typeOfItem == CollectibleItem.ItemType.Trash)
        {
            if (temporaryTrashInventory.Count < maxTrashCapacity)
            {
                temporaryTrashInventory.Add(item.itemName);
                Debug.Log("Added " + item.itemName + " to Trash Inventory.");
                Destroy(item.gameObject); 
            }
            else
            {
                Debug.Log("Trash Inventory is full!");
            }
        }
        else if (item.typeOfItem == CollectibleItem.ItemType.Special)
        {
            permanentSpecialInventory.Add(item.itemName);
            Debug.Log("Added " + item.itemName + " to Special Inventory. Keep it safe!");
            Destroy(item.gameObject);
        }
    }

    public void ThrowAwayTrash(string itemName)
    {
        if (temporaryTrashInventory.Contains(itemName))
        {
            temporaryTrashInventory.Remove(itemName);
            Debug.Log("Threw away " + itemName);
            // Here you could later add logic to spawn the trash back into the world
        }
    }
    
}
