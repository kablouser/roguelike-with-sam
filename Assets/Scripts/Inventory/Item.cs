using UnityEngine;

using static Inventory;

[CreateAssetMenu(menuName = "Items/Item")]
public class Item : HierarchyAsset
{
    public string displayName;
    [Multiline()]
    public string description;
    public Sprite sprite;
    public ItemType itemType;
}
