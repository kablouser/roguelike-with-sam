using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadRoom : MonoBehaviour
{
    public WorldTilemap tilemap;
    public Room room;
    public Vector2Int offset;

    [ContextMenu("Create Room")]
    public void CreateRoom()
    {
        for(int x = 0; x < room.roomWidth; x++)
        {
            for(int y = 0; y < room.roomHeight; y++)
            {
                tilemap.backgroundTilemap.SetTile(new Vector3Int(x + offset.x, y + offset.y, 0), room.tiles[x * room.roomHeight + y]);
            }
        }
    }
}
