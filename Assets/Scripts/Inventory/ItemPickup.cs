using UnityEngine;

public class ItemPickup : MonoBehaviour
{

    public float pickupRange = 1.5f;
    public LayerMask itemLayer;

    private PlayerControls playerControls;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    void Update()
    {
        if (playerControls.Movement.Collect.WasPressedThisFrame())
        {
            PickupItem();
        }
    }

    void PickupItem()
    {
        Collider2D hit = Physics2D.OverlapCircle(
            transform.position,
            pickupRange,
            itemLayer
        );

        if (hit != null)
        {
            ItemData item = hit.GetComponent<ItemData>();

            if (item != null)
            {
                InventoryManager.Instance.AddItem(item);

                // Note: InventoryManager already destroys the item object, so we don't need to destroy it again here.
            }
        }
    }
}
