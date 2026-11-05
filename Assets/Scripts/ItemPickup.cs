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

            bool canPickup = inventory.items.Count < 6;

            foreach (var item in inventory.items)
            {
                if (item.item == itemData && item.quantity < item.item.maxStack)
                {
                    canPickup = true;
                    break;
                }
            }

            if (canPickup)
            {
                inventory.AddItem(itemData);
                Destroy(gameObject);
            }


        }
    }
}
