using UnityEngine;

public class CharacterComponents : MonoBehaviour, WorldTilemap.IOverlay
{
    bool WorldTilemap.IOverlay.IsBlocking => true;

    public Mover mover;
    public CharacterSheet characterSheet;
    public Combatant combatant;
}
