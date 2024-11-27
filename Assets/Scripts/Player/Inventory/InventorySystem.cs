using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public int maxCapacity = 20;
    private List<Item> items = new List<Item>();

    public void AddItem(Item item)
    {
        if (items.Count < maxCapacity)
        {
            items.Add(item);
            Debug.Log(item.itemName + " added to inventory.");
        }
        else
        {
            Debug.Log("Inventory is full.");
        }
    }

    public void RemoveItem(Item item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            Debug.Log(item.itemName + " removed from inventory.");
        }
        else
        {
            Debug.Log("Item not found in inventory.");
        }
    }

    public List<Item> GetItems()
    {
        return new List<Item>(items); // Return a copy of the inventory list
    }
}
