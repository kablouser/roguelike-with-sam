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

    private void Awake()
    {
        SetFocused(false);
    }

    public void Initialise(int index, Sprite icon, string description)
    {
        if(index < 100)
            indexDisplay.SetText(index.ToString() + ".");
        else
            indexDisplay.SetText(index.ToString());

        if (icon == null)
            itemIcon.enabled = false;
        else
        {
            itemIcon.enabled = true;
            itemIcon.sprite = icon;
        }
        
        itemDescription.SetText(description);
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
