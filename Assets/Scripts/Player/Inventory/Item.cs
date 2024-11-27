using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "ScriptableObjects/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite icon; // Optional icon for UI
}
