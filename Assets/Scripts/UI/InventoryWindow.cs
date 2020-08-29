using System.Collections.Generic;
using UnityEngine;

using static Inventory;
using static InventoryTitlePanel;
using static PlayerController;

public class InventoryWindow : ControlWindow
{
    [SerializeField]
    private CharacterComponents player;
    [SerializeField]
    private InventoryTitlePanel inventoryTitle;
    [SerializeField]
    private Transform mainBody;
    [Header("Make sure size = 10")]
    [SerializeField]
    private ItemPanel[] itemPanels = new ItemPanel[10];

    /// <summary>
    /// 0 based
    /// </summary>
    private int focusContent = 0;

    private List<InventoryEntry> GetTabContent
    {
        get
        {
            if (tabContent == null)
                tabContent = new List<InventoryEntry>();
            return tabContent;
        }
    }

    public override KeyCode GetActivationKey => KeyCode.E;

    private List<InventoryEntry> tabContent;

    public override void MoveInput(Vector2Int direction)
    {
        if (direction.x != 0)
            inventoryTitle.MoveTab(direction.x);
        focusContent -= direction.y;

        UpdatePage();
    }

    private void OnEnable()
    {
        UpdatePage();
    }

    private void Update()
    {
        //detect alpha num keys
        //detect submit/jump key
    }

    private void UpdatePage()
    {
        List<InventoryEntry> tabContent = GetTabContent;
        player.inventory.GetItemsFiltered(tabContent, FilterTab);

        if (focusContent < 0)
            focusContent = tabContent.Count - 1;
        else if (tabContent.Count <= focusContent)
            focusContent = 0;

        int maxPage = (tabContent.Count - 1) / 10;
        int currentPage = focusContent / 10;

        for(int contentIndex = currentPage * 10, itemPanelIndex = 0;
            contentIndex < (currentPage + 1) * 10;
            contentIndex++, itemPanelIndex++)
        {
            if (contentIndex < tabContent.Count)
            {
                itemPanels[itemPanelIndex].gameObject.SetActive(true);
                itemPanels[itemPanelIndex].SetFocused(contentIndex == focusContent);
                itemPanels[itemPanelIndex].Initialise(
                    contentIndex + 1, //from 0 base index to 1 base counting
                    tabContent[contentIndex].item.sprite,
                    tabContent[contentIndex].item.description);
            }
            else
            {
                itemPanels[itemPanelIndex].gameObject.SetActive(false);
            }
        }
    }

    private bool FilterTab(InventoryEntry entry)
    {
        if (inventoryTitle.currentTab == TabType.Inventory)
            //all entries welcome
            return true;

        return (int)inventoryTitle.currentTab == (int)entry.item.itemType;
    }
}
