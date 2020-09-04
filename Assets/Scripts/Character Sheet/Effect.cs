using UnityEngine;

public abstract class Effect : ScriptableObject
{
    public bool hideEffect;
    public string displayName;
    
    [Tooltip("Set to -1 for infinite duration")]
    public int duration;

    [HideInInspector]
    [System.NonSerialized]
    public string description;
    [HideInInspector]
    [System.NonSerialized]
    public string sourceName;

    public CharacterComponents GetCharacter { get; private set; }
    public object GetSource { get; private set; }
    public bool IsAnimationOver { get; protected set; }

    /// <summary>
    /// The source could be an item or another object in the scene.
    /// This function returns a copy of itself so it can start affect characters.
    /// </summary>
    public virtual Effect Instantiate(CharacterComponents character, object source, string sourceName)
    {
        Effect copy = Instantiate(this);
        copy.GetCharacter = character;
        copy.GetSource = source;
        copy.sourceName = sourceName;
        copy.description = GenerateDescription();
        return copy;
    }

    public abstract void Enable();
    public abstract void Disable();
    public virtual void NewTurn() => IsAnimationOver = true;

    public abstract string GenerateDescription();

    /// <summary>
    /// Distinct from sourceName of my source. This source name is of this effect.
    /// </summary>
    /// <returns></returns>
    protected string GetEffectSourceName()
    {
        if (hideEffect || displayName.Length == 0)
            return sourceName;
        else
            return displayName;
    }
}
