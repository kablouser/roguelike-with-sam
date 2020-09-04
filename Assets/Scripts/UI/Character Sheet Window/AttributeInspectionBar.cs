using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class AttributeInspectionBar : MonoBehaviour
{
    [SerializeField]
    private Scrollbar scrollbar;
    [SerializeField]
    private TextMeshProUGUI leftAligned, rightAligned, centerAligned;
    [SerializeField]
    private RectTransform fillArea, scrollbarRect;
    [SerializeField]
    private float labelBelowY = -3f, labelAboveY = 10f;
    [SerializeField]
    private int finalLine = 9;
    [SerializeField]
    private float heightPerLine = -12f;

    public void SetValues(float scrollbarValue, float scrollbarSize,
        string label, int lineNumber)
    {
        SetScrollbarY(lineNumber);
        scrollbar.value = scrollbarValue;
        scrollbar.size = scrollbarSize;

        float barWidth = scrollbarRect.rect.width;

        leftAligned.SetText(label);
        leftAligned.ForceMeshUpdate(true, true);

        if (barWidth <
            fillArea.anchorMin.x * barWidth + leftAligned.textBounds.size.x)
        {
            rightAligned.SetText(label);
            rightAligned.ForceMeshUpdate(true, true);

            if (fillArea.anchorMax.x * barWidth - leftAligned.textBounds.size.x
                < 0)
            {
                centerAligned.SetText(label);

                SetLabelY(centerAligned, lineNumber, 1);
                leftAligned.gameObject.SetActive(false);
                rightAligned.gameObject.SetActive(false);
                centerAligned.gameObject.SetActive(true);
            }
            else
            {
                SetLabelY(rightAligned, lineNumber);
                leftAligned.gameObject.SetActive(false);
                rightAligned.gameObject.SetActive(true);
                centerAligned.gameObject.SetActive(false);
            }
        }
        else
        {
            SetLabelY(leftAligned, lineNumber);
            leftAligned.gameObject.SetActive(true);
            rightAligned.gameObject.SetActive(false);
            centerAligned.gameObject.SetActive(false);
        }
    }

    public void SetInactive(int lineNumber)
    {
        leftAligned.gameObject.SetActive(false);
        rightAligned.gameObject.SetActive(false);
        SetScrollbarY(lineNumber);
        scrollbar.value = 0;
        scrollbar.size = 1;
    }

    private void SetScrollbarY(int lineNumber)
    {
        Vector2 anchorPosition = scrollbarRect.anchoredPosition;
        anchorPosition.y = heightPerLine * lineNumber;
        scrollbarRect.anchoredPosition = anchorPosition;
    }

    private void SetLabelY(TextMeshProUGUI label, int lineNumber, int offset = 0)
    {
        Vector2 anchorPosition = label.rectTransform.anchoredPosition;
        anchorPosition.y = lineNumber == finalLine ? labelAboveY : labelBelowY;
        anchorPosition.y += offset;
        label.rectTransform.anchoredPosition = anchorPosition;
    }
}