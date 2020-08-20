using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Room", menuName = "Room")]
public class Room : ScriptableObject
{
    public int roomWidth;
    public int roomHeight;

    public TileBase[] tiles;

    [ContextMenu("PrintTiles")]
    public void PrintTiles()
    {
        Debug.Log("Length: " + tiles.Length);
    }
}
