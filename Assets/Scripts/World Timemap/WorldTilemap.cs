using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldTilemap : Singleton<WorldTilemap>
{
    public interface IOverlay
    {
        bool IsBlocking { get; }
        void SetDisplay(bool isActive);
    }

    private struct OverlayTile
    {
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
            overlayTiles = new Dictionary<Vector3Int, OverlayTile>();
            nonWalkableSet = new HashSet<TileBase>(nonWalkableTiles);
        }
    }

    public void AddForeground(Vector3Int position, IOverlay overlay)
    {
        if(overlayTiles.TryGetValue(position, out OverlayTile overlayTile))
        {
            overlayTile.overlays.Add(overlay);
            UpdateOverlays(overlayTile.overlays);
        }
        else
        {
            overlayTiles.Add(position,
                new OverlayTile(overlay, backgroundTilemap.GetTile(position)));
            overlay.SetDisplay(true);
            //remove it from the tilemap
            backgroundTilemap.SetTile(position, null);
        }
    }

    public void RemoveForeground(Vector3Int position, IOverlay overlay)
    {
        if (overlayTiles.TryGetValue(position, out OverlayTile overlayTile))
        {
            overlayTile.overlays.Remove(overlay);
            UpdateOverlays(overlayTile.overlays);

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
    /// Use this function to detect characters and items in the scene
    /// </summary>
    public ReadOnlyCollection<IOverlay> GetOverlays(Vector3Int position)
    {
        if (overlayTiles.TryGetValue(position, out OverlayTile overlayTile))
            return overlayTile.overlays.AsReadOnly();
        else return null;
    }

    /// <summary>
    /// Use this function to detect characters and items in the scene more easily
    /// </summary>
    public List<T> GetOverlays<T>(Vector3Int position)
    {
        if (overlayTiles.TryGetValue(position, out OverlayTile overlayTile))
        {
            List<T> newList = new List<T>(overlayTile.overlays.Count);
            foreach (var overlay in overlayTile.overlays)
                if (overlay is T t)
                    newList.Add(t);

            return newList;
        }
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

    private void UpdateOverlays(List<IOverlay> overlays)
    {
        overlays.Sort(SortOverlay);
        for (int i = 0; i < overlays.Count; i++)
            overlays[i].SetDisplay(i == overlays.Count - 1);
    }

    private int SortOverlay(IOverlay x, IOverlay y)
    {
        if (x.IsBlocking == y.IsBlocking)
            return 0;
        else if (y.IsBlocking)
            return -1;
        else
            return 1;
    }
}
