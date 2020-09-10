using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarTile
{
    public WorldTilemap tilemap;

    public Vector2Int position;
    public int distanceFromStart;
    public int distanceToEnd;
    public int estimatedDistance;
    public AStarTile previous;
    public Vector2Int goal;

    public AStarTile(Vector2Int position, Vector2Int goal, AStarTile previous, WorldTilemap tilemap)
    {
        this.tilemap = tilemap;
        this.position = position;
        this.previous = previous;
        this.goal = goal;
        if(previous == null)
        {
            distanceFromStart = 0;
        }
        else
        {
            distanceFromStart = previous.distanceFromStart + 1;
        }
        distanceToEnd = CalculateDistanceCost(position, goal);
        estimatedDistance = distanceToEnd + distanceFromStart;
    }

    public bool IsFree()
    {
        return !tilemap.backgroundTilemap.HasTile(new Vector3Int(position.x, position.y, 0));
    }

    public int CalculateDistanceCost(Vector2Int current, Vector2Int end)
    {
        return (Mathf.Abs(current.x - end.x) + Mathf.Abs(current.y - end.y));
    }

    public List<AStarTile> GetNeighbours()
    {
        List<AStarTile> result = new List<AStarTile>();

        //Up
        result.Add(new AStarTile(new Vector2Int(position.x, position.y + 1), goal, this, tilemap));
        //Right
        result.Add(new AStarTile(new Vector2Int(position.x + 1, position.y), goal, this, tilemap));
        //Down
        result.Add(new AStarTile(new Vector2Int(position.x, position.y - 1), goal, this, tilemap));
        //left
        result.Add(new AStarTile(new Vector2Int(position.x - 1, position.y), goal, this, tilemap));

        return result;
    }
}
