using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayWindow : ControlWindow
{
    [Header("Resources")]
    [SerializeField]
    private Slider healthBar;
    [SerializeField]
    private Slider staminaBar, manaBar;
    [SerializeField]
    private TextMeshProUGUI healthLabel, staminaLabel, manaLabel;
    [SerializeField]
    private string healthLabelStart = "HP",
        staminaLabelStart = "SP",
        manaLabelStart = "MP";

    [Header("Attributes")]
    [SerializeField]
    private TextMeshProUGUI attributesLabel;
    [SerializeField]
    private string attributeTopText = "atk   armor crit  dodge";
    [SerializeField]
    private int attributeLettersMax = 5;

    private StringBuilder stringBuilder;

    private PlayerComponents player;
    private CharacterSheet playerSheet;

    public override void Awake()
    {
        base.Awake();

        stringBuilder = new StringBuilder();

        player = PlayerController.Current.character;
        playerSheet = player.characterSheet;
    }

    private void Update()
    {
        //update resources
        UpdateResource(healthBar, healthLabel, playerSheet.health, healthLabelStart);
        UpdateResource(staminaBar, staminaLabel, playerSheet.stamina, staminaLabelStart);
        UpdateResource(manaBar, manaLabel, playerSheet.mana, manaLabelStart);

        //update attributes
        UpdateAttributes();
    }

    private void UpdateResource(Slider slider, TextMeshProUGUI label, Resource resource, string labelStart)
    {
        int current = resource.GetCurrent;
        float sliderValue = current / (float)resource.GetMax;

        slider.value = sliderValue;

        stringBuilder.Clear();
        stringBuilder.AppendFormat(labelStart);

        string currentString = current.ToString();
        int appendSpaces = 3 - currentString.Length;
        if (0 < appendSpaces)
            stringBuilder.Append(' ', appendSpaces);
        stringBuilder.Append(currentString);

        label.SetText(stringBuilder.ToString());
    }

    private void UpdateAttributes()
    {
        stringBuilder.Clear();
        stringBuilder.AppendLine(attributeTopText);

        //"atk   armor crit  dodge";
        AppendAttribute(playerSheet.attributes.attack);
        AppendAttribute(playerSheet.attributes.armour);
        AppendAttribute(playerSheet.attributes.critical, '%');
        AppendAttribute(playerSheet.attributes.dodge, '%');

        attributesLabel.SetText(stringBuilder.ToString());
    }

    private void AppendAttribute(Attribute attribute, char? append = null)
    {
        string appendString = attribute.GetTotal.ToString();
        stringBuilder.Append(appendString);

        int appendSpaces = attributeLettersMax + 1 - appendString.Length;
        if (append != null)
        {
            appendSpaces--;
            stringBuilder.Append(append);
        }

        if (0 < appendSpaces)
            stringBuilder.Append(' ', appendSpaces);
    }
}
