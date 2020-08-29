using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSheet : MonoBehaviour
{
    public abstract class AttributeContainer<T>
    {
        public T strength, dexterity, intelligence,
            attack, defence;

        public void Foreach<U>(AttributeContainer<U> other, System.Action<T,U> action)
        {
            action(strength, other.strength);
            action(dexterity, other.dexterity);
            action(intelligence, other.intelligence);
            action(attack, other.attack);
            action(defence, other.defence);
        }
    }
    [System.Serializable]
    public class AttributesList : AttributeContainer<Attribute> { }
    [System.Serializable]
    public class AttributesInts : AttributeContainer<int> { }

    public bool IsAlive => 0 < health.GetCurrent;
    public bool IsAnimationOver { get; private set; }    

    public CharacterComponents character;

    [ContextMenuItem("Update Health", "UpdateHealth")]
    public HealthResource health;
    public Resource stamina, mana;

    public AttributesList attributes;

    private List<Effect> effects;

    private void Awake()
    {
        effects = new List<Effect>();
    }

    /// <summary>
    /// updates status effects and other stuff
    /// </summary>
    public void NewTurn()
    {
        StartCoroutine(NewTurnRoutine());
    }

    public void UpdateHealth()
    {
        if (IsAlive == false)
            TurnManager.Current.ReportDead(character);
    }

    public void AddEffect(Effect effect, object source)
    {
        Effect newEffect = effect.Instantiate(character, source);
        effects.Add(newEffect);
        newEffect.Enable();
    }

    public int RemoveEffect(object source) =>
        effects.RemoveAll(x => x.GetSource == source);

    /// <summary>
    /// Removes all effects of type T and from source
    /// </summary>
    public int RemoveEffect<T>(object source) =>
        effects.RemoveAll(x => x is T && x.GetSource == source);

    private IEnumerator NewTurnRoutine()
    {
        IsAnimationOver = false;
        
        //update status effects...
        for(int i = 0; i < effects.Count; i++)
        {
            effects[i].NewTurn();
            if(effects[i].IsAnimationOver == false)
                yield return new WaitUntil(() => effects[i].IsAnimationOver);

            if (effects[i].duration == -1)
                continue;

            effects[i].duration--;
            if(effects[i].duration <= 0)
            {
                effects[i].Disable();
                effects.RemoveAt(i);
                i--;
            }
        }

        IsAnimationOver = true;
    }
}
