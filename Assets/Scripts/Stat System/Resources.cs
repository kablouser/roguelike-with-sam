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
        //write calculations here!
        base.Decrease(decrease);
        linkedCharacter.UpdateHealth();
    }
}
