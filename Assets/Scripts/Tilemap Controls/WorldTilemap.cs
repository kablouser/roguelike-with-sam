using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldTilemap : Singleton<WorldTilemap>
{
    private struct HiddenBackground
    {
        //we could hold a list here to debug easier
        /// <summary>
        /// how many foreground items are drawn on top? e.g. 1 player, 1 slash effect = count is 2
        /// </summary>
        public int foregroundCount;
        public int blockingObjects;
        public TileBase tile;

        public HiddenBackground(int foregroundCount, int blockingObjects, TileBase tile)
        {
            this.foregroundCount = foregroundCount;
            this.blockingObjects = blockingObjects;
            this.tile = tile;
        }
    }

    public Tilemap backgroundTilemap;
    public TileBase[] nonWalkableTiles;

    private Dictionary<Vector3Int, HiddenBackground> hiddenTiles;
    private HashSet<TileBase> nonWalkableSet;

    public override void Awake()
    {
        base.Awake();
        if (Current == this)
        {
            //initialise other stuff here
            hiddenTiles = new Dictionary<Vector3Int, HiddenBackground>();
            nonWalkableSet = new HashSet<TileBase>(nonWalkableTiles);
        }
    }

    public void AddForeground(Vector3Int position, bool isBlocking)
    {
        if(hiddenTiles.TryGetValue(position, out HiddenBackground hidden))
        {
            hidden.foregroundCount++;
            if (isBlocking)
                hidden.blockingObjects++;
            //structure is not a reference, so we have to reassign to update the value
            hiddenTiles[position] = hidden;
        }
        else
        {
            hiddenTiles.Add(position,
                new HiddenBackground(1, isBlocking ? 1 : 0, backgroundTilemap.GetTile(position)));
            //remove it from the tilemap
            backgroundTilemap.SetTile(position, null);
        }
    }

    public void RemoveForeground(Vector3Int position, bool isBlocking)
    {
        if (hiddenTiles.TryGetValue(position, out HiddenBackground hidden))
        {
            hidden.foregroundCount--;
            if (isBlocking)
            {
                hidden.blockingObjects--;
                if (hidden.blockingObjects < 0)
                    Debug.LogWarning("You have removed too many foreground blocking objects!", this);
            }

            if (hidden.foregroundCount == 0)
            {
                hiddenTiles.Remove(position);
                backgroundTilemap.SetTile(position, hidden.tile);
            }
            else
            {
                //structure is not a reference, so we have to reassign to update the value
                hiddenTiles[position] = hidden;
            }
        }
        else
        {
            Debug.LogWarning("No background information for " + position, this);
        }        
    }

    public bool IsWalkable(Vector3Int position)
    {
        if (hiddenTiles.TryGetValue(position, out HiddenBackground hidden))
        {
            return hidden.blockingObjects == 0 &&
                nonWalkableSet.Contains(hidden.tile) == false;
        }
        else
            return nonWalkableSet.Contains(backgroundTilemap.GetTile(position)) == false;
    }
}
