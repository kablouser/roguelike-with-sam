using UnityEngine;
using static Inventory;

[CreateAssetMenu(menuName = "Items/Armament (Weapon,Armour)")]
public class Armament : Item
{
    public override ItemType GetItemType
    {
        get
        {
            switch (equipmentType)
            {
                case EquipmentType.oneHanded:
                    return ItemType.weapon;
                case EquipmentType.twoHanded:
                    return ItemType.weapon;
                case EquipmentType.head:
                    return ItemType.armour;
                case EquipmentType.body:
                    return ItemType.armour;
                case EquipmentType.feets:
                    return ItemType.armour;
                case EquipmentType.hands:
                    return ItemType.armour;
                case EquipmentType.necklace:
                    return ItemType.accessory;
                case EquipmentType.ring:
                    return ItemType.accessory;
                default:
                    Debug.LogWarning("Invalid equipment type " + equipmentType, this);
                    return ItemType.none;
            }
        }
    }

    public bool IsEquipped => character != null;

    public EquipmentType equipmentType;
    [ContextMenuItem("Parent Effect", "ParentEffect")]
    [ContextMenuItem("Clear Effect", "ClearChildren")]
    public Effect equippedEffect;

    [Header("Armour only")]
    public int durability;
    public int maxDurability;

    //add abilities here

    protected override bool IsSingleton => false;

    private CharacterComponents character;
    private EquipmentSlot slot;

    private readonly Color leastDamagedColor = new Color(.3f, 1, .3f);
    private readonly Color mostDamagedColor = new Color(1, .3f, .3f);

    private void OnDestroy()
    {
        //prevent memory leaks
        if(IsEquipped && GetItemType == ItemType.armour)
            character.characterSheet.health.OnDecreased -= OnHealthDecrease;
    }

    public override Item RequestInstance()
    {        
        Armament newCopy = (Armament)base.RequestInstance();
        newCopy.statuses = new string[2] { string.Empty, string.Empty };
        if (newCopy.durability <= 0)
            newCopy.durability = 1;
        newCopy.UpdateEquipped();
        newCopy.UpdateDurability();        
        return newCopy;
    }

    /// <summary>
    /// Equips item if not equipped already.
    /// Otherwise unequips.
    /// </summary>
    public void ToggleEquip(CharacterComponents character, AskLeftOrRight askLeftOrRight)
    {
        if (IsEquipped)
            character.inventory.UnequipItem(slot);        
        else
            character.inventory.EquipItem(this, askLeftOrRight);
    }

    /// <summary>
    /// Don't use this function, use Inventory.EquipItem
    /// </summary>
    public void Equip(CharacterComponents character, EquipmentSlot slot)
    {
        if (GetItemType == ItemType.armour)
            character.characterSheet.health.OnDecreased += OnHealthDecrease;

        this.character = character;
        this.slot = slot;
        character.characterSheet.AddEffect(equippedEffect, this, displayName);

        UpdateEquipped();
    }

    /// <summary>
    /// Don't use this function, use Inventory.UnequipItem
    /// </summary>
    public void Unequip()
    {
        if(character == null)
        {
            Debug.LogWarning("Was never equipped, cannot unequip.", this);
            return;
        }

        if (GetItemType == ItemType.armour)
            character.characterSheet.health.OnDecreased -= OnHealthDecrease;

        character.characterSheet.RemoveEffect(this);
        character = null;

        UpdateEquipped();
    }

    private void OnHealthDecrease(int decrease)
    {
        durability -= decrease;
        UpdateDurability();
        if (durability <= 0)
        {
            var saveCharacter = character;
            character.inventory.RemoveFirstItem(this);
            Console.Current.AddLog(string.Format("{0}'s {1} has been broken!", saveCharacter.name, displayName));
        }
    }

    private void UpdateEquipped()
    {
        if(IsEquipped)
        {
            statuses[1] = string.Format("<equipped {0}>",
                equipmentType == EquipmentType.twoHanded ? "both hands" :
                Naming.SpaceOutCamelCase(slot.ToString()));
        }
        else
            statuses[1] = string.Empty;
    }

    private void UpdateDurability()
    {
        const string
            durability75to100 = "pristine",
            durability50to75 = "worn",
            durability25to50 = "scuffed",
            durability0to25 = "damaged";

        if (GetItemType == ItemType.armour)
        {
            float fraction = durability / (float)maxDurability;
            string durabilityWord;
            if (.75f <= fraction)
                durabilityWord = durability75to100;
            else if (.5f <= fraction && fraction < .75f)
                durabilityWord = durability50to75;
            else if (.25f <= fraction && fraction < .5f)
                durabilityWord = durability25to50;
            else
                durabilityWord = durability0to25;

            statuses[0] = string.Format("<<color=#{0}>{1}</color>>",
                ColorUtility.ToHtmlStringRGB(Color.Lerp(mostDamagedColor, leastDamagedColor, fraction)),
                durabilityWord);
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Parent Effect")]
    protected virtual void ParentEffect()
    {
        equippedEffect = (Effect)ParentAsset(equippedEffect);
        equippedEffect.displayName = "equipped";
        equippedEffect.duration = -1;
    }

    [ContextMenu("Clear Effect")]
    protected override void ClearChildren()
    {
        base.ClearChildren();
        equippedEffect = null;
    }
#endif
}
