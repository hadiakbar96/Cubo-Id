using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    
    public enum ItemType { Trash, Special }
    public ItemType typeOfItem;

    public string itemName;
}
