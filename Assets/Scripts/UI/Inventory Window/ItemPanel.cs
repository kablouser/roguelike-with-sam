using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemPanel : MonoBehaviour
{
    [SerializeField]
    private Image background;
    [SerializeField]
    private TextMeshProUGUI indexDisplay;
    [SerializeField]
    private Image itemIcon;
    [SerializeField]
    private TextMeshProUGUI itemDescription;
    [SerializeField]
    private float defocusedAlpha = 0.5f;

    private bool isFocused;

    private static StringBuilder GetBuilder
    {
        get
        {
            if (myBuilder == null)
                myBuilder = new StringBuilder();
            else
                myBuilder.Clear();
            return myBuilder;
        }
    }

    private static StringBuilder myBuilder;

    private void Awake()
    {
        SetFocused(false);
    }

    public void Initialise(int index, Item item)
    {
        if(index < 100)
            indexDisplay.SetText(index.ToString() + ".");
        else
            indexDisplay.SetText(index.ToString());

        if (item == null || item.sprite == null)
            itemIcon.enabled = false;
        else
        {
            itemIcon.enabled = true;
            itemIcon.sprite = item.sprite;
        }

        if (item == null)
            itemDescription.SetText(string.Empty);
        else
        {
            StringBuilder builder = GetBuilder;            

            builder.Append(item.displayName + ". ");
            builder.Append(item.description);
            builder.AppendLine();

            var statusText = item.GetStatusText;
            if (statusText != null)
                foreach (string status in statusText)
                {
                    if (status == null || status == string.Empty)
                        continue;
                    else
                        builder.Append(status + " ");
                }

            itemDescription.SetText(builder.ToString());
        }
    }

    public void SetText(string text)
    {
        itemDescription.SetText(text);
    }

    public void SetFocused(bool isFocused)
    {
        this.isFocused = isFocused;
        float setAlpha = isFocused ? 1 : defocusedAlpha;

        SetGraphicAlpha(background, setAlpha);
        SetGraphicAlpha(indexDisplay, setAlpha);
        SetGraphicAlpha(itemIcon, setAlpha);
        SetGraphicAlpha(itemDescription, setAlpha);
    }

    private void SetGraphicAlpha(Graphic graphic, float alpha)
    {
        Color color = graphic.color;
        color.a = alpha;
        graphic.color = color;
    }

    [ContextMenu("Focus")]
    private void SetFocusedTrue() => SetFocused(true);
    [ContextMenu("Defocus")]
    private void SetFocusedFalse() => SetFocused(false);
    [ContextMenu("SetAlpha")]
    private void SetAllAlpha()
    {
        var all = FindObjectsOfType<ItemPanel>();
        foreach (var itemPanel in all)
        {
            itemPanel.defocusedAlpha = defocusedAlpha;
            itemPanel.SetFocused(isFocused);
        }
    }
}
