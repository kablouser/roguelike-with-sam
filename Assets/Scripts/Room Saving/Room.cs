using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Room", menuName = "Room")]
public class Room : ScriptableObject
{
    public int roomWidth;
    public int roomHeight;
    public Vector2Int[] entrancesN;
    public Vector2Int[] entrancesE;
    public Vector2Int[] entrancesS;
    public Vector2Int[] entrancesW;
    public bool[] availableEntranceDirections = new bool[4];

    public TileBase[] tiles;

    [ContextMenu("PrintTiles")]
    public void PrintTiles()
    {
        Debug.Log("Length: " + tiles.Length);
    }

    public bool GetAvailableEntranceDirection(Direction direction)
    {
        return availableEntranceDirections[(int)direction];
    }
}
