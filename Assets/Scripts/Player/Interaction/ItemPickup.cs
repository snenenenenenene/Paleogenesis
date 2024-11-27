using UnityEngine;

public class ItemPickup : Interactable
{
    public Item itemData; // Reference to the item data

    public override void Interact()
    {
        base.Interact();
        
        InventorySystem inventory = FindObjectOfType<InventorySystem>();
        if (inventory != null)
        {
            inventory.AddItem(itemData);
            Debug.Log("Picked up: " + itemData.itemName);
            Destroy(gameObject); // Remove the item from the scene
        }
    }
}
