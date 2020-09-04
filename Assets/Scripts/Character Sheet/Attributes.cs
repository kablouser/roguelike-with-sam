using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

/// <summary>
/// These attributes affect your final attributes.
/// For example strength affects your final attack damage.
/// Examples of attributes : strength, dexterity, intelligence
/// </summary>
[Serializable]
public class Attribute
{
    [Serializable]
    public struct AttributeModifier
    {
        /// <summary>
        /// This can be an item or an object in the scene.
        /// </summary>
        public object source;
        public string sourceName;
        public int increase;

        public AttributeModifier(object source, string sourceName, int increase)
        {
            this.source = source;
            this.sourceName = sourceName;
            this.increase = increase;
        }
    }

    public int GetBaseValue => baseValue;
    public virtual int GetTotal
    {
        get
        {
            int sum = baseValue;
            modifiers.ForEach(x => sum += x.increase);
            return sum;
        }
    }

    [SerializeField]
    private int baseValue;
    [SerializeField]
    private List<AttributeModifier> modifiers;

    public ReadOnlyCollection<AttributeModifier> GetModifiers => modifiers.AsReadOnly();

    public void AddModifier(object source, string sourceName, int increase) =>
        modifiers.Add(new AttributeModifier(source, sourceName, increase));

    public int RemoveModifier(object source) =>
        modifiers.RemoveAll(x => x.source == source);

    public void SortModifiersDescending() =>
        modifiers.Sort(CompareDescending);

    private int CompareDescending(AttributeModifier a, AttributeModifier b) =>
        a.increase - b.increase;
}