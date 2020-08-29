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
    [Header("North Entrances")]
    public Vector2Int[] entrancesN = new Vector2Int[0];
    [Header("East Entrances")]
    public Vector2Int[] entrancesE = new Vector2Int[0];
    [Header("South Entrances")]
    public Vector2Int[] entrancesS = new Vector2Int[0];
    [Header("West Entrances")]
    public Vector2Int[] entrancesW = new Vector2Int[0];

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

        //Setting which directions the room can have entraces from
        if(entrancesN.Length > 0)
        {
            room.entrancesN = entrancesN;
            room.availableEntranceDirections[0] = true;
        }
        if(entrancesE.Length > 0)
        {
            room.entrancesE = entrancesE;
            room.availableEntranceDirections[1] = true;
        }
        if(entrancesS.Length > 0)
        {
            room.entrancesS = entrancesS;
            room.availableEntranceDirections[2] = true;
        }
        if(entrancesW.Length > 0)
        {
            room.entrancesW = entrancesW;
            room.availableEntranceDirections[3] = true;
        }

        AssetDatabase.CreateAsset(room, "Assets/Scriptable Objects/Rooms/" + assetName + ".asset");
        AssetDatabase.SaveAssets();
    }
}
