using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Room", menuName = "Room")]
public class Room : ScriptableObject
{
    public int roomWidth;
    public int roomHeight;
    public Vector2Int[] entrances = new Vector2Int[4];
    public bool[] availableEntranceDirections = new bool[4];

    public TileBase[] tiles;

    [ContextMenu("PrintTiles")]
    public void PrintTiles()
    {
        Debug.Log("Length: " + tiles.Length);
    }
}
