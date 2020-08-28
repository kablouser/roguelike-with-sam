using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadRoom : MonoBehaviour
{
    public WorldTilemap tilemap;
    public Room room;
    public Vector2Int offset;

    [ContextMenu("Create Room")]
    static public bool CreateRoom(Room room, Vector2Int offset, WorldTilemap tilemap, bool overlay = false)
    {
        if(!overlay)
        {
            if(!CanCreateRoom(room, offset, tilemap))
            {
                return false;
            }
        }

        for(int x = 0; x < room.roomWidth; x++)
        {
            for(int y = 0; y < room.roomHeight; y++)
            {
                tilemap.backgroundTilemap.SetTile(new Vector3Int(x + offset.x, y + offset.y, 0), room.tiles[x * room.roomHeight + y]);
            }
        }
        return true;
    }

    static public bool CanCreateRoom(Room room, Vector2Int offset, WorldTilemap tilemap)
    {
        for(int x = 0; x < room.roomWidth; x++)
        {
            for(int y = 0; y < room.roomHeight; y++)
            {
                if(tilemap.backgroundTilemap.HasTile(new Vector3Int(x + offset.x, y + offset.y, 0)) == true)
                {
                    Debug.Log("Already tile here");
                    return false;
                }
            }
        }
        return true;
    }
}
