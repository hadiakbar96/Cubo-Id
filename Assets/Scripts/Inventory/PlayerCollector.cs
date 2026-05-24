using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerCollector : MonoBehaviour
{
    private InventoryManager inventory;
    private CollectibleItem itemNearby;

    void Start()
    {
        inventory = GetComponent<InventoryManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CollectibleItem item = collision.GetComponent<CollectibleItem>();

        if (item != null)
        {
            itemNearby = item;
            Debug.Log("Standing near " + item.itemName);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<CollectibleItem>() != null)
        {
            itemNearby = null; 
        }
    }

    void Update()
    {
        if (itemNearby != null)
        {
            if (itemNearby.typeOfItem == CollectibleItem.ItemType.Trash && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                inventory.CollectItem(itemNearby);
                itemNearby = null;
            }
            else if (itemNearby.typeOfItem == CollectibleItem.ItemType.Special && Keyboard.current.fKey.wasPressedThisFrame)
            {
                inventory.CollectItem(itemNearby);
                itemNearby = null;
            }
        }
    }
}
