using UnityEngine;

public class ItemPickup : MonoBehaviour
{

    public float pickupRange = 1.5f;
    public LayerMask itemLayer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
        
    // }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
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

                Destroy(hit.gameObject);
            }
        }
    }
}
