using UnityEngine;
using static CharacterSheet;

[CreateAssetMenu(menuName = "Effects/Attribute Effect")]
public class AttributeEffect : Effect
{
    [SerializeField]
    private AttributesInts increases;

    public override void Enable() =>
        increases.Foreach(GetCharacter.characterSheet.attributes, AddModifier);

    public override void Disable() =>
        increases.Foreach(GetCharacter.characterSheet.attributes, RemoveModifier);

    private void AddModifier(int increase, Attribute attribute) => attribute.AddModifier(this, increase);
    private void RemoveModifier(int _, Attribute attribute) => attribute.RemoveModifier(this);
}
