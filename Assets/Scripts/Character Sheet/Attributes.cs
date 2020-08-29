using System;
using System.Collections.Generic;
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
        public int increase;

        public AttributeModifier(object source, int increase)
        {
            this.source = source;
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
    public List<AttributeModifier> modifiers;

    public void AddModifier(object source, int increase) =>
        modifiers.Add(new AttributeModifier(source, increase));

    public int RemoveModifier(object source) =>
        modifiers.RemoveAll(x => x.source == source);
}