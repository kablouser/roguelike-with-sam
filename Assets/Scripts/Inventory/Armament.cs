using UnityEngine;
using static Inventory;

[CreateAssetMenu(menuName = "Items/Armament (Weapon,Armour)")]
public class Armament : Item
{
    public EquipmentType equipmentType;
    [ContextMenuItem("Parent Effect", "ParentEffect")]
    [ContextMenuItem("Clear Effect", "ClearChildren")]
    public Effect equippedEffect;
    //add abilities here

    public Armament()
    {
        //when asset is created
        itemType = ItemType.weapon;
    }

    public void Equip(CharacterComponents character)
    {
        character.characterSheet.AddEffect(equippedEffect, this);
    }

    public void Unequip(CharacterComponents character)
    {
        character.characterSheet.RemoveEffect(this);
    }

#if UNITY_EDITOR
    [ContextMenu("Parent Effect")]
    protected virtual void ParentEffect()
    {
        equippedEffect = (Effect)ParentAsset(equippedEffect);
    }

    [ContextMenu("Clear Effect")]
    protected override void ClearChildren()
    {
        base.ClearChildren();
        equippedEffect = null;
    }
#endif
}
