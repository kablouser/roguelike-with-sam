using UnityEngine;

public class DroppedItem : MonoBehaviour, WorldTilemap.IOverlay
{
    bool WorldTilemap.IOverlay.IsBlocking => false;

    [ContextMenuItem("Update Sprite", "UpdateSprite")]
    public SpriteRenderer itemSprite;
    public Item item;
    private Vector3Int worldPosition;

    void WorldTilemap.IOverlay.SetDisplay(bool isActive) =>
        itemSprite.gameObject.SetActive(isActive);

    private void Awake()
    {
        UpdateSprite();
        worldPosition = new Vector3Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.y), 0);        
        WorldTilemap.Current.AddForeground(worldPosition, this);
        transform.position = worldPosition;
    }

    private void OnDisable()
    {
        WorldTilemap.Current.RemoveForeground(worldPosition, this);
    }

    [ContextMenu("Update Sprite")]
    private void UpdateSprite()
    {
        if (item == null)
            itemSprite.sprite = null;
        else
            itemSprite.sprite = item.sprite;
    }

    public void Initialise(Item item)
    {
        this.item = item;
        UpdateSprite();
    }

    public void Pickup(Inventory intoInventory)
    {
        intoInventory.AddItem(item);
        enabled = false;
        Destroy(gameObject);
    }
}
