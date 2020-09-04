using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static InventoryTitlePanel;
using static PlayerController;
using static Inventory;

using TMPro;

public class InventoryWindow : ControlWindow
{    
    [SerializeField]
    private InventoryTitlePanel inventoryTitle;
    [SerializeField]
    private Transform mainBody;
    [Header("Make sure size = 10")]
    [SerializeField]
    private ItemPanel[] itemPanels = new ItemPanel[10];
    [SerializeField]
    private TextMeshProUGUI spaceControlHint;

    private CharacterComponents player;

    /// <summary>
    /// 0 based
    /// </summary>
    private int focusContent = 0;
    private bool lockInput;

    private List<Item> GetTabContent
    {
        get
        {
            if (tabContent == null)
                tabContent = new List<Item>();
            return tabContent;
        }
    }

    public override KeyCode GetActivationKey => KeyCode.E;

    private List<Item> tabContent;

    public override void MoveInput(Vector2Int direction)
    {
        if (lockInput)
            return;

        if (direction.x != 0)
        {
            inventoryTitle.MoveTab(direction.x);
            UpdateAll();
        }
        focusContent -= direction.y;

        if (direction.y != 0)
            UpdatePage();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        lockInput = false;

        if (player == null)
            player = PlayerController.Current.character;

        player.inventory.OnInventoryChange += UpdateAll;
        UpdateAll();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        player.inventory.OnInventoryChange -= UpdateAll;
    }

    private void Update()
    {
        if (lockInput)
            return;

        //detect alpha num keys
        for (int i = 0; i < 10; i++)
        {
            if(Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                int currentPage = focusContent / 10;
                //0 based index
                int selectIndex;
                if (i == 0)
                    selectIndex = (currentPage + 1) * 10 - 1;
                else
                    selectIndex = currentPage * 10 + i - 1;

                List<Item> tabContent = GetTabContent;
                if(selectIndex < tabContent.Count)
                {
                    focusContent = selectIndex;
                    UpdatePage();
                    InteractItem();
                    break;
                }
            }
        }
        //detect submit/jump key
        if(Input.GetButtonDown(Jump))
            InteractItem();
    }

    private void UpdateAll()
    {
        UpdateContent();
        UpdatePage();
    }

    private void UpdatePage()
    {
        if (focusContent < 0)
            focusContent = tabContent.Count - 1;
        else if (tabContent.Count <= focusContent)
            focusContent = 0;

        int currentPage = focusContent / 10;

        const string defaultHint = "use item";
        string useHint = defaultHint;
        if(focusContent < tabContent.Count)
        {
            if (tabContent[focusContent] is Armament armament)
                useHint = armament.IsEquipped ? "unequip" : "equip";
        }
        spaceControlHint.SetText(useHint);

        for (int contentIndex = currentPage * 10, itemPanelIndex = 0;
            contentIndex < (currentPage + 1) * 10;
            contentIndex++, itemPanelIndex++)
        {
            if (contentIndex < tabContent.Count)
            {
                itemPanels[itemPanelIndex].gameObject.SetActive(true);
                itemPanels[itemPanelIndex].SetFocused(contentIndex == focusContent);
                itemPanels[itemPanelIndex].Initialise(
                    contentIndex + 1, //from 0 base index to 1 base counting
                    tabContent[contentIndex]);                
            }
            else
            {
                itemPanels[itemPanelIndex].gameObject.SetActive(false);
            }
        }
    }

    private void UpdateContent()
    {
        List<Item> tabContent = GetTabContent;
        player.inventory.GetItemsFiltered(tabContent, FilterTab);
    }

    private bool FilterTab(Item item)
    {
        if (item == null)
            return true;

        if (inventoryTitle.currentTab == TabType.Inventory)
            //all entries welcome
            return true;
        else
            return (int)inventoryTitle.currentTab == (int)item.GetItemType;
    }

    private void InteractItem()
    {
        List<Item> tabContent = GetTabContent;
        if (focusContent < tabContent.Count &&
            tabContent[focusContent] is Armament armament)
        {
            armament.ToggleEquip(player, AskLeftOrRight);
            if (lockInput == false)
                UpdatePage();
        }
    }

    private void AskLeftOrRight(AnswerLeftOrRight answerCallback)
    {
        StartCoroutine(AskRoutine(answerCallback));
    }

    private IEnumerator AskRoutine(AnswerLeftOrRight answerCallback)
    {
        const string askLeftOrRightQuestion = "press < or > to equip to a hand";
        lockInput = true;
        float x;

        itemPanels[focusContent % 10].SetText(askLeftOrRightQuestion);

        do
        {
            yield return null;
            itemPanels[focusContent % 10].SetText(askLeftOrRightQuestion);
            x = Input.GetAxisRaw(Horizontal);
        }
        while (x == 0);

        answerCallback(x < 0);
        lockInput = false;
        UpdatePage();
    }
}
