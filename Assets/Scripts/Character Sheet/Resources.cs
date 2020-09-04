using System;
using UnityEngine;

[Serializable]
public class Resource
{
    public int GetCurrent => current;
    public int GetMax => max;

    [SerializeField]
    private int current;
    [SerializeField]
    private int max;

    public virtual void Increase(int increase)
    {
        current += increase;
        if (max < current)
            current = max;
    }

    public virtual void Decrease(int decrease)
    {
        current -= decrease;
        if (current <= 0)
            current = 0;
    }
}

[Serializable]
public class HealthResource : Resource
{
    public event Action<int> OnDecreased;

    [SerializeField]
    private CharacterSheet linkedCharacter;

    public override void Increase(int increase)
    {
        //write heal boosts here!
        base.Increase(increase);
        linkedCharacter.UpdateHealth();
    }

    public override void Decrease(int decrease)
    {
        int rawValue = decrease;
        //write calculations here!
        int defence = linkedCharacter.attributes.armour.GetTotal;
        decrease -= defence;

        if (decrease < 0)
        {
            //maybe play a different animation to show defence
            decrease = 0;
        }

        if(decrease == 0 && 0 < rawValue)
            Console.Current.AddLog(linkedCharacter.name + "'s armour resisted all damage!");
        else
            Console.Current.AddLog(string.Format("{0} took {1} damage.", linkedCharacter.name, decrease));

        base.Decrease(decrease);
        linkedCharacter.UpdateHealth();

        OnDecreased?.Invoke(rawValue);
    }
}
