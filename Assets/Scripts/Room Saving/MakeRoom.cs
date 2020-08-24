using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class MakeRoom : MonoBehaviour
{
    [Header("Room Properties")]
    public int roomWidth;
    public int roomHeight;
    [Header("Room Properties - NESW")]
    public Vector2Int[] entrances = new Vector2Int[4];

    [Header("Dependances")]
    public string assetName = "NewRoom";
    public WorldTilemap tilemap;

    //Bottom to up left to right
    [ContextMenu("Make New Room")]
    public void MakeNewRoom()
    {
        Debug.Log("Make new room");

        if(roomWidth <= 0 || roomHeight <= 0)
        {
            Debug.Log("roomWidth and roomHeight need to be bigger than 0");
            return;
        }

        Room room = ScriptableObject.CreateInstance<Room>();
        room.tiles = new TileBase[roomWidth * roomHeight];
        room.roomWidth = roomWidth;
        room.roomHeight = roomHeight;

        for(int x = 0; x < roomWidth; x++)
        {
            for(int y = 0; y < roomHeight; y++)
            {
                room.tiles[x*roomHeight + y] = tilemap.backgroundTilemap.GetTile(new Vector3Int(x, y, 0));
            }
        }

        room.entrances = entrances;

        for(int x = 0; x < entrances.Length; x++)
        {
            if(entrances[x] != new Vector2Int(0, 0))
            {
                room.availableEntranceDirections[x] = true;
            }
            else
            {
                room.availableEntranceDirections[x] = false;
            }
        }

        AssetDatabase.CreateAsset(room, "Assets/Scriptable Objects/Rooms/" + assetName + ".asset");
        AssetDatabase.SaveAssets();
    }
}
