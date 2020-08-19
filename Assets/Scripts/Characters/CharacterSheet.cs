using UnityEngine;

public class CharacterSheet : MonoBehaviour
{
    public bool IsAlive => 0 < health;
    public bool IsAnimationOver { get; private set; }

    public CharacterComponents character;

    [SerializeField]
    [ContextMenuItem("Update", "OnHealthChanged")]
    private int health = 1;

    /// <summary>
    /// updates status effects and other stuff
    /// </summary>
    public void NewTurn()
    {
        IsAnimationOver = true;
    }

    public void OnHealthChanged()
    {
        if (IsAlive == false)
            character.deathController.OnDeath();
    }
}
