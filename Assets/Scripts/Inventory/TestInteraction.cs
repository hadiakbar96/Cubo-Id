using UnityEngine.InputSystem;
using UnityEngine;

public class TestInteraction : MonoBehaviour
{
    // These create empty slots in the Unity Inspector so we can link our other objects
    public InventoryManager inventory; 

    public ItemData[] trashItems;
    public ItemData[] specialItems;

    // Update acts like a continuous loop, running once every single frame of the game
    void Update()
    {
       // Check if a keyboard exists, then check if the space key was pressed
    //    if (Input.GetKeyDown(KeyCode.Space))
       if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
       {
            for (int i = 0; i < trashItems.Length; i++)
            {
                if (trashItems[i] != null)
                {
                    inventory.AddItem(trashItems[i]);
                    break;
                }
            }
        //    if (itemToPickUp != null)
        //    {
        //        inventory.AddItem(itemToPickUp);
        //    }
        //    else
        //    {
        //        Debug.Log("There is nothing left to pick up!");
        //    }
       }
       if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
       {
            for (int i = 0; i < specialItems.Length; i++)
            {
                if (specialItems[i] != null)
                {
                    inventory.AddItem(specialItems[i]);
                    break;
                }
            }
        }
   }
}