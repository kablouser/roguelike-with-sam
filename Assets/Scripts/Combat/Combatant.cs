using UnityEngine;

/// <summary>
/// uses character sheet to do combat with
/// </summary>
public class Combatant : MonoBehaviour
{
    public CharacterComponents character;
    private WorldTilemap world;

    private void Awake()
    {
        world = WorldTilemap.Current;
    }

    public void Attack(Vector2Int inDirection)
    {
        //we can attack in 1 tile range
        if (1 < inDirection.sqrMagnitude) return;

        Vector3Int attackPosition = character.mover.GetPosition + (Vector3Int) inDirection;
        var overlays = world.GetOverlays(attackPosition);
        if (overlays == null) return;

        foreach(var overlay in overlays)
            if(overlay is CharacterComponents other)
            {
                //decrease other health
                other.characterSheet.health.Decrease(
                    //with my attack
                    character.characterSheet.attack.GetTotal);
                return;
            }
    }
}
