using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [System.Serializable]
    public struct InventoryEntry
    {
        public uint count;
        public Item item;

        public InventoryEntry(uint count, Item item)
        {
            this.count = count;
            this.item = item;
        }
    }

    public ReadOnlyCollection<InventoryEntry> GetItems
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

    public CharacterComponents character;

    [SerializeField]
    private List<InventoryEntry> items;
    private ReadOnlyCollection<InventoryEntry> readonlyItems;

    private Armament[] equipment;

    private void Awake()
    {
        equipment = new Armament[(int)EquipmentSlot.MAX];
    }

    public void AddItem(uint count, Item item)
    {
        int findIndex = items.FindIndex(entry => entry.item == item);
        if(findIndex == -1)
            items.Add(new InventoryEntry(count, item));
        else
        {
            InventoryEntry getEntry = items[findIndex];
            getEntry.count += count;
            items[findIndex] = getEntry;
        }
    }

    public void RemoveItem(uint count, Item item)
    {
        int findIndex = items.FindIndex(entry => entry.item == item);
        if (findIndex == -1)
            return;
        else
        {
            InventoryEntry getEntry = items[findIndex];
            getEntry.count -= count;
            if (getEntry.count <= 0)
                items.RemoveAt(findIndex);
            else
                items[findIndex] = getEntry;
        }
    }

    public bool Contains(Item item, uint atLeastCount = 1)
    {
        int findIndex = items.FindIndex(entry => entry.item == item);
        if (findIndex == -1)
            return false;
        else
            return atLeastCount <= items[findIndex].count;
    }

    /// <summary>
    /// askIsLeft is called if the armament can be equipped in the left hand or the right hand.
    /// </summary>
    public void EquipItem(Armament armament, System.Func<bool> askIsLeft)
    {
        switch (armament.equipmentType)
        {
            case EquipmentType.oneHanded:
                EquipSingleItem(armament, askIsLeft() ? EquipmentSlot.leftHand : EquipmentSlot.rightHand);
                break;
            case EquipmentType.ring:
                EquipSingleItem(armament, askIsLeft() ? EquipmentSlot.leftRing : EquipmentSlot.rightRing);
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

        armament.Unequip(character);
        equipment[index] = null;
        if (armament.equipmentType == EquipmentType.twoHanded)
            equipment[(int)EquipmentSlot.leftHand] = equipment[(int)EquipmentSlot.rightHand] = null;
    }

    public void GetItemsFiltered(List<InventoryEntry> outputList, System.Func<InventoryEntry, bool> filter)
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

            armament.Equip(character);
            equipment[(int)EquipmentSlot.leftHand] = equipment[(int)EquipmentSlot.rightHand] = armament;
        }
        else
        {
            //clear out item currently equipped
            UnequipItem(intoSlot);

            armament.Equip(character);
            equipment[(int)intoSlot] = armament;
        }        
    }
}
