
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public InventorySlotUI[] slots;
    public Inventory inventory;

    private void OnEnable()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged += DisplayInventory;
        }
    }

    private void Start()
    {
        DisplayInventory();
    }

    private void OnDisable()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged -= DisplayInventory;
        }
    }

    public void DisplayInventory()
    {
        if (inventory == null)
        {
            Debug.LogError("InventoryUI: Inventory reference is missing!");
            return;
        }

        foreach (InventorySlotUI slot in slots)
        {
            slot.ClearSlot();
        }

        for (int i = 0;
             i < inventory.items.Count && i < slots.Length;
             i++)
        {
            slots[i].SetItem(inventory.items[i]);
        }
    }
}

