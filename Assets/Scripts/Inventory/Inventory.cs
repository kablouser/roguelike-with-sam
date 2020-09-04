using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public ReadOnlyCollection<Item> GetItems
    {
        get
        {
            if (readonlyItems == null)
                readonlyItems = items.AsReadOnly();
            return readonlyItems;
        }
    }

    public enum EquipmentSlot { leftHand, rightHand, head, body, feets, hands, necklace, leftRing, rightRing, MAX }
    public enum EquipmentType { oneHanded, twoHanded, head, body, feets, hands, necklace, ring, MAX }
    public enum ItemType { none, weapon, armour, accessory, scroll, potion, MAX }

    public delegate void AskLeftOrRight(AnswerLeftOrRight answerCallback);
    public delegate void AnswerLeftOrRight(bool isLeft);

    public CharacterComponents character;
    public event System.Action OnInventoryChange;

    [SerializeField]
    private List<Item> items;
    private ReadOnlyCollection<Item> readonlyItems;

    private Armament[] equipment;

    private void Awake()
    {
        equipment = new Armament[(int)EquipmentSlot.MAX];
    }

    private void OnValidate()
    {
        OnInventoryChange?.Invoke();
    }

    public void AddItem(Item item)
    {
        item = item.RequestInstance();
        items.Add(item);
    }

    public bool RemoveFirstItem(Item item)
    {
        bool result = items.Remove(item);

        if (item is Armament armament)
            armament.Unequip();

        return result;
    }

    public void RemoveItem(int index)
    {
        if(index < 0 || items.Count <= index)
        {
            Debug.LogWarning($"RemoveItem index {index} out of range [0,{items.Count - 1}]", this);
            return;
        }
        Armament removeItem = items[index] as Armament;
        items.RemoveAt(index);

        if(removeItem != null)
            removeItem.Unequip();
    }

    /// <summary>
    /// askIsLeft is called if the armament can be equipped in the left hand or the right hand.
    /// </summary>
    public void EquipItem(Armament armament, AskLeftOrRight askLeftOrRight)
    {
        switch (armament.equipmentType)
        {
            case EquipmentType.oneHanded:
                askLeftOrRight(isLeft => EquipSingleItem(armament, isLeft ? EquipmentSlot.leftHand : EquipmentSlot.rightHand));                
                break;
            case EquipmentType.ring:
                askLeftOrRight(isLeft => EquipSingleItem(armament, isLeft ? EquipmentSlot.leftRing : EquipmentSlot.rightRing));
                break;
            default:
                EquipSingleItem(armament, (EquipmentSlot)armament.equipmentType);
                break;
        }
    }

    public void UnequipItem(EquipmentSlot fromSlot)
    {
        int index = (int)fromSlot;
        Armament armament = equipment[index];
        if (armament == null)
            return;

        armament.Unequip();
        equipment[index] = null;
        if (armament.equipmentType == EquipmentType.twoHanded)
            equipment[(int)EquipmentSlot.leftHand] = equipment[(int)EquipmentSlot.rightHand] = null;
    }

    public void GetItemsFiltered(List<Item> outputList, System.Func<Item, bool> filter)
    {
        outputList.Clear();
        items.ForEach(x =>
        {
            if (filter(x))
                outputList.Add(x);
        });
    }


    private void EquipSingleItem(Armament armament, EquipmentSlot intoSlot)
    {
        if (armament.equipmentType == EquipmentType.twoHanded)
        {
            UnequipItem(EquipmentSlot.leftHand);
            UnequipItem(EquipmentSlot.rightHand);

            armament.Equip(character, EquipmentSlot.leftHand);
            equipment[(int)EquipmentSlot.leftHand] = equipment[(int)EquipmentSlot.rightHand] = armament;
        }
        else
        {
            //clear out item currently equipped
            UnequipItem(intoSlot);

            armament.Equip(character, intoSlot);
            equipment[(int)intoSlot] = armament;
        }        
    }
}
