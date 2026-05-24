using UnityEngine.InputSystem;
using UnityEngine;
// If you used Method 2 for the Input System, keep: using UnityEngine.InputSystem;

public class TestInteraction : MonoBehaviour
{
    public InventoryManager inventory; 
    
    // Arrays to hold multiple items instead of just one
    public CollectibleItem[] trashItems; 
    public CollectibleItem[] specialItems; 

    void Update()
    {
        // --- TRASH ITEMS (Spacebar) ---
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) 
        // Or if using New Input: if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            // Loop through the array to find the first available item
            for (int i = 0; i < trashItems.Length; i++)
            {
                if (trashItems[i] != null) // Check if it hasn't been destroyed yet
                {
                    inventory.CollectItem(trashItems[i]);
                    break; // Exit the loop so we only pick up ONE item per key press
                }
            }
        }

        // --- SPECIAL ITEMS (F Key) ---
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        // Or if using New Input: if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            for (int i = 0; i < specialItems.Length; i++)
            {
                if (specialItems[i] != null)
                {
                    inventory.CollectItem(specialItems[i]);
                    break; 
                }
            }
        }
    }
}