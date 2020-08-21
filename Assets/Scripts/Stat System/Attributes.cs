using System;
using UnityEngine;

/// <summary>
/// These attributes affect your final attributes.
/// For example strength affects your final attack damage.
/// But you can have a potion of attack damage that dosen't affect strength.
/// Examples of attributes : strength, dexterity, intelligence
/// </summary>
[Serializable]
public class Attribute
{
    public int GetBaseValue => baseValue;
    public virtual int GetTotal => baseValue + additional;

    [SerializeField]
    private int baseValue;
    public int additional;
}

/// <summary>
/// These final attributes are calculated from the character's attributes.
/// But they can buff'd independantly.
/// Examples of final attributes : attack, defence
/// </summary>
public abstract class FinalAttribute : Attribute
{
    /// <summary>
    /// Get the additional from the character's attributes
    /// </summary>
    public abstract int GetCharacterAdditional { get; }
    public override int GetTotal => GetBaseValue + additional + GetCharacterAdditional;

    [SerializeField]
    protected CharacterSheet linkedCharacter;
}

[Serializable]
public class AttackAttribute : FinalAttribute
{
    public override int GetCharacterAdditional => linkedCharacter.strength.GetTotal;
}