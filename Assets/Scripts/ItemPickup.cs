using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    // Item Scriptable Object 
    public ItemData itemData;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Add item to inventory
            Inventory inventory = other.GetComponent<Inventory>();

            inventory.AddItem(itemData);
            Destroy(gameObject);
        }
    }
}
