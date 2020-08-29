using UnityEngine;

public abstract class Effect : ScriptableObject
{
    [Tooltip("Set to -1 for infinite duration")]
    public int duration;

    public CharacterComponents GetCharacter { get; private set; }
    public object GetSource { get; private set; }
    public bool IsAnimationOver { get; protected set; }

    /// <summary>
    /// The source could be an item or another object in the scene.
    /// This function returns a copy of itself so it can start affect characters.
    /// </summary>
    public Effect Instantiate(CharacterComponents character, object source)
    {
        Effect copy = Instantiate(this);
        copy.GetCharacter = character;
        copy.GetSource = source;
        return copy;
    }

    public abstract void Enable();
    public abstract void Disable();
    public virtual void NewTurn() => IsAnimationOver = true;
}
