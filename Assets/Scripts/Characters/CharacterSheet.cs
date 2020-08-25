using UnityEngine;

public class CharacterSheet : MonoBehaviour
{
    /// <summary>
    /// These attributes affect your final attributes.
    /// For example strength affects your final attack damage.
    /// But you can have a potion of attack damage that dosen't affect strength.
    /// </summary>
    [System.Serializable]
    public class Attribute
    {
        public int GetBaseValue => baseValue;
        public virtual int GetTotal => baseValue + additional;

        [SerializeField]
        private int baseValue;
        public int additional;
    }

    public abstract class FinalAttribute : Attribute
    {
        /// <summary>
        /// Get the additional from the character's attributes
        /// </summary>
        public abstract int GetCharacterAdditional { get; }
        public override int GetTotal => GetBaseValue + additional + GetCharacterAdditional;

        [SerializeField]
        private CharacterSheet linkedCharacter;
    }

    public bool IsAlive => 0 < health.GetCurrent;
    public bool IsAnimationOver { get; private set; }

    public CharacterComponents character;

    [ContextMenuItem("Update Health", "UpdateHealth")]
    public HealthResource health;
    public Resource mana;

    public Attribute strength, intelligence;
    public AttackAttribute attack;

    /// <summary>
    /// updates status effects and other stuff
    /// </summary>
    public void NewTurn()
    {
        //update status effects...
        IsAnimationOver = true;
    }

    public void UpdateHealth()
    {
        if (IsAlive == false)
            TurnManager.Current.ReportDead(character);
    }
}
