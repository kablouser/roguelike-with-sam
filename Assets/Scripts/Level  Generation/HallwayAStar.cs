using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallwayAStar
{
    private List<AStarTile> explored = new List<AStarTile>();
    private List<AStarTile> unExplored = new List<AStarTile>();

    public List<EntrancePosition> FindPath(Vector2Int startPosition, Vector2Int goal, WorldTilemap tilemap, RandomRooms random)
    {
        explored.Clear();
        unExplored.Clear();

        if(tilemap.backgroundTilemap.GetTile(new Vector3Int(startPosition.x, startPosition.y, 0)))
        {
            return new List<EntrancePosition>();
        }

        unExplored.Add(new AStarTile(startPosition, goal, null, tilemap));
        AStarTile currentTile;
        int whileLoopCount = 0;
        while(unExplored.Count > 0)
        {
            currentTile = unExplored[0];

            for(int i = 0; i < unExplored.Count; i++)
            {
                if(unExplored[i].estimatedDistance < currentTile.estimatedDistance || unExplored[i].estimatedDistance == currentTile.estimatedDistance 
                        && unExplored[i].distanceToEnd < currentTile.distanceToEnd)
                {
                    currentTile = unExplored[i];
                }
            }

            unExplored.Remove(currentTile);
            explored.Add(currentTile);

            //Debugging
            random.allCheckByPathfinding.Add(currentTile.position);
            Debug.Log("UNEXPLORED!!!!: " + unExplored.Count);

            if(currentTile.position == goal)
            {
                return GetPath(currentTile);
            }

            foreach(AStarTile neighbour in currentTile.GetNeighbours())
            {
                if(neighbour.position == goal)
                {
                    return GetPath(neighbour);
                }
                if(!neighbour.IsFree() || explored.Find(aStarTile => aStarTile.position == neighbour.position) != null || unExplored.Find(aStarTile => aStarTile.position == neighbour.position) != null)
                {
                    continue;
                }

                unExplored.Add(neighbour);
            }
            whileLoopCount++;
            if(whileLoopCount > 5000)
            {
                Debug.Log("whileLoopCount: " + whileLoopCount);
                break;
            }
        }
        return new List<EntrancePosition>();
    }

    private List<EntrancePosition> GetPath(AStarTile goal)
    {
        List<EntrancePosition> result = new List<EntrancePosition>();
        List<Vector2Int> tempResult = new List<Vector2Int>();

        int pathWhileLoopCount = 0;
        while(true)
        {
            tempResult.Add(goal.position);
            if(goal.previous == null)
            {
                break;
            }
            goal = goal.previous;
            pathWhileLoopCount++;
            if(pathWhileLoopCount > 1000)
            {
                Debug.Log("pathWhileLoopCount: " + pathWhileLoopCount);
                break;
            }
        }

        tempResult.Reverse();

        for(int i = 0; i < tempResult.Count; i++)
        {
            if(i != tempResult.Count - 1)
            {
                //North
                if(tempResult[i+1].y > tempResult[i].y)
                {
                    result.Add(new EntrancePosition(tempResult[i], Direction.north, 0));
                }
                //East
                else if(tempResult[i + 1].x > tempResult[i].x)
                {
                    result.Add(new EntrancePosition(tempResult[i], Direction.east, 0));
                }
                //South
                else if(tempResult[i + 1].y < tempResult[i].y)
                {
                    result.Add(new EntrancePosition(tempResult[i], Direction.south, 0));
                }
                //West
                else
                {
                    result.Add(new EntrancePosition(tempResult[i], Direction.west, 0));
                }
            }
            else
            {
                result.Add(new EntrancePosition(tempResult[i], result[i - 1].cardinalDirection, 0));
            }
        }

        return result;
    }

}