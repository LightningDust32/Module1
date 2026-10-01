using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{

    public List<Item> items = new List<Item>();

    public void AddItem(Item item)
    {
        Debug.Log("Added" +  item.itemType);
        items.Add(item);
    }

    public void RemoveItem(Item item)
    {

        if (items.Contains(item))
        {
            Debug.Log("Removed" + item.itemType);
            items.Remove(item);
        }
    }

    public void PrintInventory()
    {
        for (int i = 0; i < items.Count; i++)
        {
            Debug.Log(items[i].itemType);
        }
        
    }
}
