using UnityEngine;

using static Inventory;

[CreateAssetMenu(menuName = "Items/Item")]
public class Item : HierarchyAsset
{
    public virtual ItemType GetItemType => ItemType.none;

    public string[] GetStatusText
    {
        get
        {
            if (statuses == null)
                return null;
            else
                return statuses;
        }
    }

    public string displayName;    
    [Multiline()]
    public string description;
    public Sprite sprite;

    protected string[] statuses;

    /// <summary>
    /// If true, each inventory references the same item object.
    /// If false, each inventory references a unique item object.
    /// </summary>
    protected virtual bool IsSingleton => true;
    private bool instanced = false;

    public virtual Item RequestInstance()
    {        
        if (IsSingleton == false && instanced == false)
        {
            Item copy = Instantiate(this);
            copy.instanced = true;
            return copy;
        }
        else
            return this;
    }
}
