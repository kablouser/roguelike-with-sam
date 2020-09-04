using System.Text;
using UnityEngine;
using static CharacterSheet;

[CreateAssetMenu(menuName = "Effects/Attribute Effect")]
public class AttributeEffect : Effect
{
    [SerializeField]
    private AttributesInts increases;

    public override void Enable()
    {
        foreach (var tuple in increases.GetTwinEnumerator(GetCharacter.characterSheet.attributes))
            AddModifier(tuple.Item1, tuple.Item2);
    }

    public override void Disable()
    {
        foreach (var tuple in increases.GetTwinEnumerator(GetCharacter.characterSheet.attributes))
            RemoveModifier(tuple.Item1, tuple.Item2);
    }

    private void AddModifier(int increase, Attribute attribute)
    {
        if(increase != 0)
            attribute.AddModifier(this, sourceName, increase);
    }

    private void RemoveModifier(int _, Attribute attribute) => attribute.RemoveModifier(this);

    public override string GenerateDescription()
    {
        StringBuilder stringBuilder = new StringBuilder();

        foreach(var tuple in increases.ForeachWithNames())
        {
            if (tuple.Item1 == 0)
                continue;
            stringBuilder.AppendFormat("{0}{1} {2} ",
                tuple.Item1 < 0 ? '-' : '+', Mathf.Abs(tuple.Item1), tuple.Item2);
        }

        if (0 < stringBuilder.Length)
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
        return stringBuilder.ToString();
    }
}
