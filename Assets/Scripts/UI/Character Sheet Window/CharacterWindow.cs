using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using System;
using System.Collections.ObjectModel;

public class CharacterWindow : ControlWindow
{
    public override KeyCode GetActivationKey => KeyCode.C;

    private const float defocusedAlpha = 0.5f;
    private const int sheetSectionLength = 10;
    private const int resourcesLength = 3;

    [Header("Sheet Section")]
    [SerializeField]
    private TextMeshProUGUI sheetLeftText;
    [SerializeField]
    private TextMeshProUGUI sheetRightText;
    [SerializeField]
    private Slider healthSlider, staminaSlider, manaSlider;
    [SerializeField]
    private MultiGraphicFader healthFader, staminaFader, manaFader;
    [SerializeField]
    private List<string> leftTextContent = new List<string> {
        "health", "stamina", "mana",
        "strength", "dexterity", "intelligence", "attack", "armour", "critical", "dodge" };
    [SerializeField]
    private AttributeInspectionBar attributeInspectionBar;
    [SerializeField]
    private MultiGraphicFader attributeInspectionBarFader;

    private const int effectsSectionLength = 4;

    [Header("Effects Section")]
    [SerializeField]
    private TextMeshProUGUI effectsText;
    [SerializeField]
    private Scrollbar effectsScrollBar;

    private CharacterSheet playerSheet;
    private Vector2Int currentPosition;
    private StringBuilder stringBuilder;

    private string focusColorTag, defocusColorTag;

    private List<string> rightTextContent;
    private List<string> effectsContent;

    public override void Awake()
    {
        base.Awake();

        if(playerSheet == null)
            playerSheet = PlayerController.Current.character.characterSheet;
        if (stringBuilder == null)
        {
            Color getColor = sheetLeftText.color;
            getColor.a = defocusedAlpha;
            focusColorTag = string.Format("<color=#{0}>", ColorUtility.ToHtmlStringRGB(getColor));
            defocusColorTag = string.Format("<color=#{0}>", ColorUtility.ToHtmlStringRGBA(getColor));

            stringBuilder = new StringBuilder();
            rightTextContent = new List<string>(sheetSectionLength);
            effectsContent = new List<string>();
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        Awake();
        UpdateAll();
        playerSheet.character.inventory.OnInventoryChange += UpdateAll;        
    }

    public override void OnDisable()
    {
        base.OnDisable();
        playerSheet.character.inventory.OnInventoryChange -= UpdateAll;
    }

    public override void MoveInput(Vector2Int direction)
    {
        currentPosition.x += direction.x;
        currentPosition.y -= direction.y;

        if (direction.y != 0)
            currentPosition.x = 0;

        UpdateFocus();
    }

    private void UpdateAll()
    {
        UpdateContent();
        UpdateFocus();
    }

    private void UpdateContent()
    {
        rightTextContent.Clear();
        AppendResource(playerSheet.health, healthSlider);
        AppendResource(playerSheet.stamina, staminaSlider);
        AppendResource(playerSheet.mana, manaSlider);

        attributeIndex = 0;
        foreach (var attribute in playerSheet.attributes)
            AppendAttribute(attribute);

        effectsContent.Clear();
        var getEffects = playerSheet.GetEffects;
        for (int i = 0; i < getEffects.Count; i++)
        {
            if (getEffects[i].hideEffect)
                continue;

            stringBuilder.Clear();

            if (0 < getEffects[i].displayName.Length)
                stringBuilder.AppendFormat("{0}. ", getEffects[i].displayName);

            if (0 < getEffects[i].description.Length)
                stringBuilder.AppendFormat("{0}", getEffects[i].description);

            stringBuilder.AppendLine();

            if (getEffects[i].GetSource != null)
                stringBuilder.AppendFormat("<{0}> ", getEffects[i].sourceName);
            if (getEffects[i].duration != -1)
                stringBuilder.AppendFormat("<{0} turns> ", getEffects[i].duration);

            effectsContent.Add(stringBuilder.ToString());
        }
    }

    private void AppendResource(Resource resource, Slider slider)
    {
        int current = resource.GetCurrent;
        int max = resource.GetMax;
        slider.value = current / (float)max;
        rightTextContent.Add(string.Format("{0}/{1}", current, max));
    }

    int attributeIndex;
    private void AppendAttribute(Attribute attribute)
    {
        //last 2 are percentages
        int additional = attribute.GetTotal - attribute.GetBaseValue;
        char sign = additional < 0 ? '-' : '+';
        additional = Mathf.Abs(additional);

        if (sheetSectionLength - 2 <= resourcesLength + attributeIndex)
            rightTextContent.Add(string.Format("{0} {1}{2}%",
            attribute.GetBaseValue, sign, additional));
        else
            rightTextContent.Add(string.Format("{0} {1}{2}",
            attribute.GetBaseValue, sign, additional));

        attributeIndex++;
    }

    private void UpdateFocus()
    {
        if (currentPosition.y < 0)
            currentPosition.y = sheetSectionLength + effectsContent.Count - 1;
        else if (sheetSectionLength + effectsContent.Count <= currentPosition.y)
            currentPosition.y = 0;

        if (currentPosition.y < sheetSectionLength)
        {
            FocusCharacterSheet(currentPosition.y);
            DefocusEffects();
        }
        else
        {
            DefocusCharacterSheet();
            FocusEffects(currentPosition.y - sheetSectionLength);
        }
    }

    private void FocusCharacterSheet(int focusLine)
    {
        SetFocusedLines(leftTextContent, sheetLeftText, focusLine);
        SetFocusedLines(rightTextContent, sheetRightText, focusLine);

        healthFader.SetAlphas(focusLine == 0 ? 1f : defocusedAlpha);
        staminaFader.SetAlphas(focusLine == 1 ? 1f : defocusedAlpha);
        manaFader.SetAlphas(focusLine == 2 ? 1f : defocusedAlpha);

        if(focusLine < 3)
            attributeInspectionBar.gameObject.SetActive(false);
        else
        {
            attributeInspectionBar.gameObject.SetActive(true);
            FocusAttributeInspectionBar(focusLine);            
        }
    }

    private void DefocusCharacterSheet()
    {
        SetDefocusedLines(leftTextContent, sheetLeftText);
        SetDefocusedLines(rightTextContent, sheetRightText);

        healthFader.SetAlphas(defocusedAlpha);
        staminaFader.SetAlphas(defocusedAlpha);
        manaFader.SetAlphas(defocusedAlpha);

        attributeInspectionBar.gameObject.SetActive(false);
    }

    private void SetFocusedLines(List<string> lines, TextMeshProUGUI text, int focusLine)
    {
        stringBuilder.Clear();
        stringBuilder.Append(defocusColorTag);
        for (int i = 0; i < lines.Count; i++)
        {
            if (i == focusLine)
                stringBuilder.AppendFormat("{0}{1}{2}\n",focusColorTag, lines[i], defocusColorTag);
            else
                stringBuilder.AppendLine(lines[i]);
        }
        text.SetText(stringBuilder.ToString());
    }

    int startLine;
    private void FocusEffects(int focusLine)
    {
        if (focusLine < startLine)
            startLine = focusLine;
        else if (startLine + effectsSectionLength <= focusLine)
            startLine = focusLine - effectsSectionLength + 1;

        stringBuilder.Clear();
        stringBuilder.Append(defocusColorTag);
        for(int line = startLine; line < startLine + effectsSectionLength &&
            line < effectsContent.Count;
            line++)
        {
            if (focusLine == line)
                stringBuilder.AppendFormat("{0}{1}{2}\n",
                    focusColorTag, effectsContent[line], defocusColorTag);
            else
                stringBuilder.AppendLine(effectsContent[line]);
        }
        effectsText.SetText(stringBuilder.ToString());

        if (effectsContent.Count <= effectsSectionLength)
            effectsScrollBar.size = 1;
        else
            effectsScrollBar.size = effectsSectionLength / (float)effectsContent.Count;
        effectsScrollBar.value = startLine / (float)Mathf.Max(1, effectsContent.Count - effectsSectionLength);
    }

    private void DefocusEffects()
    {
        stringBuilder.Clear();
        stringBuilder.Append(defocusColorTag);
        if(0 < effectsContent.Count)
            for (int i = 0; i < effectsContent.Count &&
                i < effectsSectionLength; i++)
                stringBuilder.AppendLine(effectsContent[i]);
        else
            stringBuilder.Append("no effects");
        effectsText.SetText(stringBuilder.ToString());

        if (effectsContent.Count <= effectsSectionLength)
            effectsScrollBar.size = 1;
        else
            effectsScrollBar.size = effectsSectionLength / (float)effectsContent.Count;
        effectsScrollBar.value = 0;
    }

    private void SetDefocusedLines(List<string> lines, TextMeshProUGUI text)
    {
        stringBuilder.Clear();
        stringBuilder.Append(defocusColorTag);
        foreach (string line in lines)
            stringBuilder.AppendLine(line);
        text.SetText(stringBuilder.ToString());
    }

    private void FocusAttributeInspectionBar(int focusLine)
    {
        Attribute attribute = GetAttributeAtLine(focusLine - resourcesLength);
        attribute.SortModifiersDescending();

        var modifiers = RemoveZeroModifiers(attribute.GetModifiers);
        if (attribute.GetBaseValue != 0)
            modifiers.Insert(0, new AttributeSource(attribute.GetBaseValue, "base"));

        if(modifiers.Count == 0)
        {
            //something else
            attributeInspectionBar.SetInactive(focusLine);
            attributeInspectionBarFader.SetAlphas(defocusedAlpha);
            return;
        }
        else
            attributeInspectionBarFader.SetAlphas(1f);

        //clamp input x
        if (modifiers.Count <= currentPosition.x)
            currentPosition.x = 0;
        else if (currentPosition.x < 0)
            currentPosition.x = modifiers.Count - 1;

        //calculate scroll bar value and size
        int focusValue = modifiers[currentPosition.x].increase;
        int scrollBarOffset = 0;

        int absFocusValue = Mathf.Abs(focusValue);
        int totalAbsValue = 0;
        for (int i = 0; i < modifiers.Count; i++)
        {
            int absIncrease = Mathf.Abs(modifiers[i].increase);
            totalAbsValue += absIncrease;
            if (i < currentPosition.x)
                scrollBarOffset += absIncrease;
        }

        float scrollbarValue = scrollBarOffset / (float)Mathf.Max(1, totalAbsValue - absFocusValue);
        float scrollbarSize = absFocusValue / (float)Mathf.Max(1, totalAbsValue);

        //label building
        stringBuilder.Clear();
        stringBuilder.Append(focusValue < 0 ? '-' : '+');
        stringBuilder.Append(absFocusValue);
        stringBuilder.Append(' ');

        if (0 < modifiers[currentPosition.x].sourceName.Length)
            stringBuilder.AppendFormat("<{0}>", modifiers[currentPosition.x].sourceName);

        attributeInspectionBar.SetValues(scrollbarValue, scrollbarSize, stringBuilder.ToString(), focusLine);
    }

    private Attribute GetAttributeAtLine(int line)
    {
        int index = 0;
        foreach (Attribute attribute in playerSheet.attributes)
        {
            if (index == line)
                return attribute;
            else
                index++;
        }

        Debug.LogError("can't find attribute at line " + line, this);
        return default;
    }

    [Serializable]
    private struct AttributeSource
    {
        public int increase;
        public string sourceName;

        public AttributeSource(int increase, string sourceName)
        {
            this.increase = increase;
            this.sourceName = sourceName;
        }
    }
    [SerializeField]
    private List<AttributeSource> normalisedModifiers = new List<AttributeSource>();
    private List<AttributeSource> RemoveZeroModifiers(ReadOnlyCollection<Attribute.AttributeModifier> getModifiers)
    {
        if (normalisedModifiers == null)
            normalisedModifiers = new List<AttributeSource>();
        else
            normalisedModifiers.Clear();

        foreach (var modifier in getModifiers)
            if (modifier.increase != 0)
                normalisedModifiers.Add(
                    new AttributeSource(modifier.increase, modifier.sourceName));

        return normalisedModifiers;
    }
}
