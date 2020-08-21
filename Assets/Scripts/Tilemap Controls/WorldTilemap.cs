using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldTilemap : Singleton<WorldTilemap>
{
    public interface IOverlay
    {
        bool IsBlocking { get; }
    }

    private struct OverlayTile
    {
        /// <summary>
        /// how many foreground items are drawn on top? e.g. 1 player, 1 slash effect = count is 2
        /// </summary>
        public List<IOverlay> overlays;
        public TileBase backgroundTile;

        public OverlayTile(IOverlay overlay, TileBase backgroundTile)
        {
            overlays = new List<IOverlay>() { overlay };
            this.backgroundTile = backgroundTile;
        }
    }

    public Tilemap backgroundTilemap;
    public TileBase[] nonWalkableTiles;

    private Dictionary<Vector3Int, OverlayTile> overlayTiles;
    private HashSet<TileBase> nonWalkableSet;

    public override void Awake()
    {
        base.Awake();
        if (Current == this)
        {
            //initialise other stuff here
            overlayTiles = new Dictionary<Vector3Int, OverlayTile>();
            nonWalkableSet = new HashSet<TileBase>(nonWalkableTiles);
        }
    }

    public void AddForeground(Vector3Int position, IOverlay overlay)
    {
        if(overlayTiles.TryGetValue(position, out OverlayTile overlayTile))
        {
            overlayTile.overlays.Add(overlay);
        }
        else
        {
            overlayTiles.Add(position,
                new OverlayTile(overlay, backgroundTilemap.GetTile(position)));
            //remove it from the tilemap
            backgroundTilemap.SetTile(position, null);
        }
    }

    public void RemoveForeground(Vector3Int position, IOverlay overlay)
    {
        if (overlayTiles.TryGetValue(position, out OverlayTile overlayTile))
        {
            overlayTile.overlays.Remove(overlay);

            if (overlayTile.overlays.Count == 0)
            {
                overlayTiles.Remove(position);
                backgroundTilemap.SetTile(position, overlayTile.backgroundTile);
            }
        }
        else
        {
            Debug.LogWarning("No background information for " + position, this);
        }        
    }

    /// <summary>
    /// Use this function to detect characters in the scene
    /// </summary>
    public ReadOnlyCollection<IOverlay> GetOverlays(Vector3Int position)
    {
        if (overlayTiles.TryGetValue(position, out OverlayTile overlayTile))
            return overlayTile.overlays.AsReadOnly();
        else return null;
    }

    public bool IsWalkable(Vector3Int position)
    {
        if (overlayTiles.TryGetValue(position, out OverlayTile overlayTile))
            //if we find no blocking foreground objects and
            return overlayTile.overlays.Find((x) => x.IsBlocking) == null &&
                //the background tile is walkable
                nonWalkableSet.Contains(overlayTile.backgroundTile) == false;
        else
            return nonWalkableSet.Contains(backgroundTilemap.GetTile(position)) == false;
    }
}
