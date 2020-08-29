using System.Text;
using UnityEngine;
using TMPro;

public class InventoryTitlePanel : MonoBehaviour
{
    //Capitalised for displaying on the ui
    public enum TabType { Inventory, Weapon, Armour, Accessory, Scroll, Potion, MAX }

    private const string startFormat = "<color=#00000080>";
    private const string tabFormat = "-{0}- ";
    private const string currentTabFormat = "</color>-{0}- <color=#00000080>";

    [SerializeField]
    private TextMeshProUGUI title;

    [ContextMenuItem("Update Title", "UpdateTitle")]
    public TabType currentTab;

    private StringBuilder GetTitleBuilder
    {
        get
        {
            if (titleBuilder == null)
                titleBuilder = new StringBuilder(128);
            return titleBuilder;
        }
    }

    private StringBuilder titleBuilder;

    private void Awake()
    {
        UpdateTitle();
    }

    public void MoveTab(int direction)
    {
        int sum = (int)currentTab + direction;
        if (sum < 0)
            sum = (int)TabType.MAX - 1;
        else if ((int)TabType.MAX <= sum)
            sum = 0;

        currentTab = (TabType)sum;
        UpdateTitle();
    }

    [ContextMenu("Update Title")]
    private void UpdateTitle()
    {
        StringBuilder titleBuilder = GetTitleBuilder;
        titleBuilder.Clear();
        titleBuilder.Append(startFormat);

        for (int i = 0; i < (int)TabType.MAX; i++)
        {
            TabType itemType = (TabType)i;
            if (currentTab == itemType)
                titleBuilder.AppendFormat(currentTabFormat, itemType);
            else
                titleBuilder.AppendFormat(tabFormat, itemType);            
        }

        title.text = titleBuilder.ToString();
        title.gameObject.SetActive(false);
        title.gameObject.SetActive(true);
    }
}
