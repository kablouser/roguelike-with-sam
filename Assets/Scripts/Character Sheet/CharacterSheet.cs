using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class CharacterSheet : MonoBehaviour
{
    public abstract class AttributeContainer<T> : IEnumerable<T>
    {
        public T strength, dexterity, intelligence,
            attack, armour, critical, dodge;

        private static readonly string[] attributeNames = new string[]
        {
            "strength",
            "dexterity",
            "intelligence",
            "attack",
            "armour",
            "critical",
            "dodge"
        };

        public IEnumerable<Tuple<T,string>> ForeachWithNames()
        {
            foreach (var tuple in GetTwinEnumerator(attributeNames))
                yield return tuple;
        }

        public IEnumerator<T> GetEnumerator()
        {
            yield return strength;
            yield return dexterity;
            yield return intelligence;
            yield return attack;
            yield return armour;
            yield return critical;
            yield return dodge;
        }

        public IEnumerable<Tuple<T,U>> GetTwinEnumerator<U>(IEnumerable<U> other)
        {
            IEnumerator<T> aEnum = GetEnumerator();
            IEnumerator<U> bEnum = other.GetEnumerator();

            while (aEnum.MoveNext() && bEnum.MoveNext())
                yield return new Tuple<T, U>(aEnum.Current, bEnum.Current);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    [Serializable]
    public class AttributesList : AttributeContainer<Attribute> { }
    [Serializable]
    public class AttributesInts : AttributeContainer<int> { }

    public bool IsAlive => 0 < health.GetCurrent;
    public bool IsAnimationOver { get; private set; }
    public ReadOnlyCollection<Effect> GetEffects => effects.AsReadOnly();

    public event System.Action<int> OnAttacked;

    public CharacterComponents character;

    public string displayName = "character";

    [ContextMenuItem("Update Health", "UpdateHealth")]
    public HealthResource health;
    public Resource stamina, mana;

    public AttributesList attributes;

    //TODO REMOVE THIS
    [SerializeField]
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

    public void AddEffect(Effect effect, object source, string sourceName)
    {
        Effect newEffect = effect.Instantiate(character, source, sourceName);
        effects.Add(newEffect);
        newEffect.Enable();
    }

    public int RemoveEffect(object source)
    {
        return effects.RemoveAll(x =>
        {
            bool match = x.GetSource == source;
            if (match)
                x.Disable();
            return match;
        });
    }

    /// <summary>
    /// Removes all effects of type T and from source
    /// </summary>
    public int RemoveEffect<T>(object source)
    {
        return effects.RemoveAll(x =>
        {
            bool match = x is T && x.GetSource == source;
            if (match)
                x.Disable();
            return match;
        });
    }

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
