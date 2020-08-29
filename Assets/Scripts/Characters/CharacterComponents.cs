using UnityEngine;

public class CharacterComponents : MonoBehaviour, WorldTilemap.IOverlay
{
    public GameObject display;
    public Mover mover;
    public CharacterSheet characterSheet;
    public Combatant combatant;
    public Inventory inventory;

    bool WorldTilemap.IOverlay.IsBlocking => true;

    void WorldTilemap.IOverlay.SetDisplay(bool isActive) =>
        display.SetActive(isActive);
}
