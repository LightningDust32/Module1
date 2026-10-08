using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{

    public List<InventorySlot> items = new List<InventorySlot>();

    public void AddItem(ItemData item)
    {
        Debug.Log("Added" +  item.itemName);
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item == item && items[i].quantity < item.maxStack)
            {
                items[i].quantity++;
                return;
            }
        }
        InventorySlot slot = new InventorySlot();
        slot.item = item;
        slot.quantity = 1;
        items.Add(slot);
    }

    public void RemoveItem(ItemData item)
    {
        for (int i = items.Count - 1; i >= 0; i--)
        {
            if (items[i].item == item)
            {
                Debug.Log("Removed" + item.itemName);
                items[i].quantity--;
                if (items[i].quantity <= 0)
                {
                    items.RemoveAt(i);
                }
            }
        }
    }

    public bool FindItem(ItemData item)
    {
        for (int i = items.Count - 1; i >= 0; i--)
        {
            if (items[i].item == item)
            {
                Debug.Log("Item Found");
                return true;
            }
        }
        
        return false;
    }
}
