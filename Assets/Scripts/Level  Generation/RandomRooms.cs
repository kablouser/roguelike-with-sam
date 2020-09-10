using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct EntrancePosition
{
    public Vector2Int position;
    //0 - N, 1 - E, 2 - S, 3 - W
    public Direction cardinalDirection;
    public int roomNumber;

    public EntrancePosition(Vector2Int position, Direction cardinalDirection, int roomNumber)
    {
        this.position = position;
        this.cardinalDirection = cardinalDirection;
        this.roomNumber = roomNumber;
    }
}

public enum Direction
{
    north, east, south, west, none
}

public enum HallwayDirection
{
    northSouth, eastWest, northEast, southEast, southWest, northWest
}

public class RandomRooms : MonoBehaviour
{
    [Header("Hallways, NS then EW")]
    public Room[] hallways = new Room[2];
    public TileBase hallwayWall;
    [Header("Possible Rooms")]
    public List<Room> roomPossibilities = new List<Room>();
    [Header("Size of level in room count")]
    public int numberOfRooms;

    public WorldTilemap tilemap;

    private List<EntrancePosition> freeEntrances = new List<EntrancePosition>();
    private List<EntrancePosition> freeEntrancesCopy = new List<EntrancePosition>();

    private List<Room> roomPossibilitiesCopy = new List<Room>();

    private Vector2Int offset;

    private int currentRoomNumber = 0;

    private HallwayAStar pathfinder = new HallwayAStar();

    //Stuff for debugging gizmos
    private List<EntrancePosition> pathForGizmos = new List<EntrancePosition>();
    private Vector2Int newBuildingEntrancePositionForGizmos = new Vector2Int(0, 0);
    private Vector2Int realStart = new Vector2Int(0, 0);
    private Vector2Int realEnd = new Vector2Int(0, 0);
    public List<Vector2Int> allCheckByPathfinding = new List<Vector2Int>();

    [ContextMenu("Create Level")]
    public void GenerateRandomLevel()
    {
        freeEntrances.Clear();

        if(roomPossibilities.Count == 0)
        {
            Debug.Log("No room possibilities");
            return;
        }

        tilemap.backgroundTilemap.ClearAllTiles();

        for(int x = 0; x < numberOfRooms; x++)
        {
            CopyList(roomPossibilities, ref roomPossibilitiesCopy);
            CopyList(freeEntrances, ref freeEntrancesCopy);

            int roomIndex = Random.Range(0, roomPossibilitiesCopy.Count);

            if(x == 0)
            {
                LoadRoom.CreateRoom(roomPossibilitiesCopy[roomIndex], new Vector2Int(0, 0), tilemap);
                AddToFreeEntrances(roomIndex, new Vector2Int(0, 0));
                Debug.Log("freeEntrancesCount: " + freeEntrances.Count);
            }
            else
            {
                Direction direction = Direction.none;
                Vector2Int newBuildingEntrancePosition = new Vector2Int();
                bool whileLoop = true;
                while (whileLoop)
                {
                    int positionIndex = Random.Range(0, freeEntrancesCopy.Count - 1);
                    Debug.Log("PositionIndex: " + positionIndex + "roomIndex: " + roomIndex);
                    Debug.Log("FreeExpansionCountCopy: " + freeEntrancesCopy.Count);

                    if(TryCreateRoom(freeEntrancesCopy[positionIndex], roomPossibilitiesCopy[roomIndex], ref direction, ref newBuildingEntrancePosition))
                    {
                        newBuildingEntrancePositionForGizmos = newBuildingEntrancePosition;
                        if(LoadRoom.CreateRoom(roomPossibilitiesCopy[roomIndex], offset, tilemap))
                        {
                            AddToFreeEntrances(roomIndex, offset);

                            //Create the hallway
                            AddHallway(freeEntrancesCopy[positionIndex], new EntrancePosition(newBuildingEntrancePosition, Direction.none, 0));

                            FreeEntrancesRemoveUsingWorldPosition(freeEntrancesCopy[positionIndex].position);

                            whileLoop = false;
                        }
                        else
                        {
                            freeEntrancesCopy.RemoveAt(positionIndex);
                        }
                    }
                    else
                    {
                        if (freeEntrances.Count == 0)
                        {
                            roomPossibilitiesCopy.RemoveAt(roomIndex);
                            roomIndex = Random.Range(0, roomPossibilitiesCopy.Count - 1);
                            CopyList(freeEntrances, ref freeEntrancesCopy);
                        }
                    }

                    if (freeEntrances.Count == 0 && roomPossibilitiesCopy.Count == 0)
                    {
                        whileLoop = false;
                    }
                }
            }
        }

        //Adding hallways to rooms after generation of all rooms
        bool hallwayTesting = true;
        int hallwayTestingCount = 0;
        while(hallwayTesting)
        {
            //Debugging
            allCheckByPathfinding.Clear();

            int roomIndex1 = Random.Range(0, freeEntrances.Count);
            int roomIndex2 = Random.Range(0, freeEntrances.Count);
            realStart = freeEntrances[roomIndex1].position;
            realEnd = freeEntrances[roomIndex2].position;
            if(!AddHallway(freeEntrances[roomIndex1], freeEntrances[roomIndex2]))
            {
                hallwayTesting = false;
            }
            hallwayTestingCount++;
            if(hallwayTestingCount > 1000)
            {
                break;
            }
        }
    }

    private void AddToFreeEntrances(int roomIndex, Vector2Int offset)
    {
        LoopDirectionArray(roomPossibilitiesCopy[roomIndex].entrancesN, Direction.north, offset);
        LoopDirectionArray(roomPossibilitiesCopy[roomIndex].entrancesE, Direction.east, offset);
        LoopDirectionArray(roomPossibilitiesCopy[roomIndex].entrancesS, Direction.south, offset);
        LoopDirectionArray(roomPossibilitiesCopy[roomIndex].entrancesW, Direction.west, offset);

        currentRoomNumber++;
    }

    private void LoopDirectionArray(Vector2Int[] array, Direction direction, Vector2Int offset)
    {
        for(int i = 0; i < array.Length; i++)
        {
            Debug.Log("For Loop I: " + i);
            if(tilemap.backgroundTilemap.HasTile(new Vector3Int(array[i].x + offset.x, array[i].y + offset.y, 0)))
            {
                Debug.Log("Added to free entrances");
                freeEntrances.Add(new EntrancePosition(new Vector2Int(array[i].x + offset.x, array[i].y + offset.y), direction, currentRoomNumber));
            }
        }
    }

    private void FreeEntrancesRemoveUsingWorldPosition(Vector2Int position)
    {
        for(int i = 0; i < freeEntrances.Count; i++)
        {
            if(freeEntrances[i].position == position)
            {
                freeEntrances.RemoveAt(i);
                break;
            }
        }
    }

    private void CopyList<T>(List<T> original, ref List<T> copy)
    {
        if(copy == null)
        {
            copy = new List<T>(original.Count);
        }
        else
        {
            copy.Clear();
        }

        for (int i = 0; i < original.Count; i++)
        {
            copy.Add(original[i]);
        }
    }

    private List<T> CopyArray<T>(T[] original)
    {
        List<T> newList = new List<T>(original.Length);

        for (int i = 0; i < original.Length; i++)
        {
            newList.Add(original[i]);
        }

        return newList;
    }

    private bool TryCreateRoom(EntrancePosition entrance, Room room, ref Direction direction, ref Vector2Int entrancePosition)
    {
        if (entrance.cardinalDirection == Direction.north && room.GetAvailableEntranceDirection(Direction.south) == true)
        {
            List<Vector2Int> roomEntrancesCopy = CopyArray(room.entrancesS);

            bool whileLoop = true;
            while (whileLoop == true)
            {
                if (roomEntrancesCopy.Count == 0)
                {
                    return false;
                }

                int roomEntranceIndex = Random.Range(0, roomEntrancesCopy.Count - 1);

                int xOffset = entrance.position.x - roomEntrancesCopy[roomEntranceIndex].x;
                int yOffset = entrance.position.y + 3;

                offset = new Vector2Int(xOffset, yOffset);

                if (LoadRoom.CanCreateRoom(room, offset, tilemap))
                {
                    direction = Direction.north;
                    entrancePosition = new Vector2Int(offset.x + roomEntrancesCopy[roomEntranceIndex].x, offset.y + roomEntrancesCopy[roomEntranceIndex].y);
                    return true;
                }

                roomEntrancesCopy.RemoveAt(roomEntranceIndex);
            }
        }
        else if(entrance.cardinalDirection == Direction.east && room.GetAvailableEntranceDirection(Direction.west) == true)
        {
            List<Vector2Int> roomEntrancesCopy = CopyArray(room.entrancesW);

            bool whileLoop = true;
            while (whileLoop == true)
            {
                if(roomEntrancesCopy.Count == 0)
                {
                    return false;
                }

                int roomEntranceIndex = Random.Range(0, roomEntrancesCopy.Count - 1);

                int xOffset = entrance.position.x + 3;
                int yOffset = entrance.position.y - roomEntrancesCopy[roomEntranceIndex].y;

                offset = new Vector2Int(xOffset, yOffset);

                if(LoadRoom.CanCreateRoom(room, offset, tilemap))
                {
                    direction = Direction.east;
                    entrancePosition = new Vector2Int(offset.x + roomEntrancesCopy[roomEntranceIndex].x, offset.y + roomEntrancesCopy[roomEntranceIndex].y);
                    return true;
                }

                roomEntrancesCopy.RemoveAt(roomEntranceIndex);
            }
        }
        else if(entrance.cardinalDirection == Direction.south && room.GetAvailableEntranceDirection(Direction.north) == true)
        {
            List<Vector2Int> roomEntrancesCopy = CopyArray(room.entrancesN);

            bool whileLoop = true;
            while(whileLoop == true)
            {
                if(roomEntrancesCopy.Count == 0)
                {
                    return false;
                }

                int roomEntranceIndex = Random.Range(0, roomEntrancesCopy.Count - 1);

                int xOffset = entrance.position.x - roomEntrancesCopy[roomEntranceIndex].x;
                int yOffset = entrance.position.y - 2 - room.roomHeight;

                offset = new Vector2Int(xOffset, yOffset);

                if(LoadRoom.CanCreateRoom(room, offset, tilemap))
                {
                    direction = Direction.south;
                    entrancePosition = new Vector2Int(offset.x + roomEntrancesCopy[roomEntranceIndex].x, offset.y + roomEntrancesCopy[roomEntranceIndex].y);
                    return true;
                }

                roomEntrancesCopy.RemoveAt(roomEntranceIndex);
            }
        }
        else if(entrance.cardinalDirection == Direction.west && room.GetAvailableEntranceDirection(Direction.east) == true)
        {
            List<Vector2Int> roomEntrancesCopy = CopyArray(room.entrancesE);

            bool whileLoop = true;
            while(whileLoop == true)
            {
                if(roomEntrancesCopy.Count == 0)
                {
                    return false;
                }

                int roomEntranceIndex = Random.Range(0, roomEntrancesCopy.Count - 1);

                int xOffset = entrance.position.x - 2 - room.roomWidth;
                int yOffset = entrance.position.y - roomEntrancesCopy[roomEntranceIndex].y;

                offset = new Vector2Int(xOffset, yOffset);

                if(LoadRoom.CanCreateRoom(room, offset, tilemap))
                {
                    direction = Direction.west;
                    entrancePosition = new Vector2Int(offset.x + roomEntrancesCopy[roomEntranceIndex].x, offset.y + roomEntrancesCopy[roomEntranceIndex].y);
                    return true;
                }

                roomEntrancesCopy.RemoveAt(roomEntranceIndex);
            }
        }
        return false;
    }

    //Use cardinal direction to go away from room and not though it
    private bool AddHallway(EntrancePosition start, EntrancePosition end)
    {
        if(start.roomNumber == end.roomNumber)
        {
            return false;
        }

        List<Room> hallwaysToLoad = new List<Room>();

        if(start.position.x == end.position.x && Mathf.Abs(start.position.y - start.position.y) == 1)
        {
            LoadRoom.CreateRoom(hallways[(int)HallwayDirection.northSouth], new Vector2Int(start.position.x, start.position.y), tilemap, true);
            LoadRoom.CreateRoom(hallways[(int)HallwayDirection.northSouth], new Vector2Int(start.position.x, Mathf.Abs(start.position.y - end.position.y) + Mathf.Min(start.position.y, start.position.y)), tilemap, true);
            LoadRoom.CreateRoom(hallways[(int)HallwayDirection.northSouth], new Vector2Int(start.position.x, end.position.y), tilemap, true);
            return true;
        }
        if (start.position.y == end.position.y && Mathf.Abs(start.position.x - start.position.x) == 1)
        {
            LoadRoom.CreateRoom(hallways[(int)HallwayDirection.eastWest], new Vector2Int(start.position.x, start.position.y), tilemap, true);
            LoadRoom.CreateRoom(hallways[(int)HallwayDirection.eastWest], new Vector2Int(Mathf.Abs(start.position.x - end.position.x) + Mathf.Min(start.position.x, start.position.x), start.position.y), tilemap, true);
            LoadRoom.CreateRoom(hallways[(int)HallwayDirection.eastWest], new Vector2Int(end.position.x, start.position.y), tilemap, true);
            return true;
        }

        Vector2Int offset;

        if(start.cardinalDirection == Direction.north)
        {
            offset = new Vector2Int(0, 1);
        }
        else if(start.cardinalDirection == Direction.east)
        {
            offset = new Vector2Int(1, 0);
        }
        else if (start.cardinalDirection == Direction.south)
        {
            offset = new Vector2Int(0, -1);
        }
        else
        {
            offset = new Vector2Int(-1, 0);
        }

        List<EntrancePosition> path = pathfinder.FindPath(start.position + offset, end.position, tilemap, this);
        if(path.Count == 0)
        {
            return false;
        }
        pathForGizmos = path;

        for(int i = 0; i < path.Count; i++)
        {
            //Get all adjacent tiles
            Vector2Int[] adjacent = new Vector2Int[8];
            //Up
            adjacent[0] = new Vector2Int(path[i].position.x, path[i].position.y + 1);
            //Right
            adjacent[1] = new Vector2Int(path[i].position.x + 1, path[i].position.y);
            //Down
            adjacent[2] = new Vector2Int(path[i].position.x, path[i].position.y - 1);
            //left
            adjacent[3] = new Vector2Int(path[i].position.x - 1, path[i].position.y);
            //Up left
            adjacent[4] = new Vector2Int(path[i].position.x - 1, path[i].position.y + 1);
            //Up right
            adjacent[5] = new Vector2Int(path[i].position.x + 1, path[i].position.y + 1);
            //Down right
            adjacent[6] = new Vector2Int(path[i].position.x + 1, path[i].position.y - 1);
            //Down left
            adjacent[7] = new Vector2Int(path[i].position.x - 1, path[i].position.y - 1);

            foreach(Vector2Int neighbour in adjacent)
            {
                if(path.Find(entrancePosition => entrancePosition.position == neighbour) != null)
                {

                }
            }
        }

        //bool skipFirstHallway = false;
        //if(path.Count > 0)
        //{
        //    if(start.cardinalDirection == Direction.north || start.cardinalDirection == Direction.south)
        //    {
        //        if(path[0].cardinalDirection == Direction.east && start.cardinalDirection == Direction.north)
        //        {
        //            LoadRoom.CreateRoom(hallways[(int)HallwayDirection.southWest], new Vector2Int(path[0].position.x, path[0].position.y), tilemap, true);
        //            skipFirstHallway = true;
        //        }
        //        else if(path[0].cardinalDirection == Direction.west && start.cardinalDirection == Direction.north)
        //        {
        //            LoadRoom.CreateRoom(hallways[(int)HallwayDirection.southEast], new Vector2Int(path[0].position.x, path[0].position.y), tilemap, true);
        //            skipFirstHallway = true;
        //        }
        //        else if(path[0].cardinalDirection == Direction.east && start.cardinalDirection == Direction.south)
        //        {
        //            LoadRoom.CreateRoom(hallways[(int)HallwayDirection.northWest], new Vector2Int(path[0].position.x, path[0].position.y), tilemap, true);
        //            skipFirstHallway = true;
        //        }
        //        else if(path[0].cardinalDirection == Direction.west && start.cardinalDirection == Direction.south)
        //        {
        //            LoadRoom.CreateRoom(hallways[(int)HallwayDirection.northEast], new Vector2Int(path[0].position.x, path[0].position.y), tilemap, true);
        //            skipFirstHallway = true;
        //        }
        //        else
        //        {
        //            LoadRoom.CreateRoom(hallways[(int)HallwayDirection.northSouth], new Vector2Int(start.position.x, start.position.y), tilemap, true);
        //        }
        //    }
        //    else
        //    {
        //        if(start.cardinalDirection == Direction.east && path[0].cardinalDirection == Direction.north)
        //        {
        //            LoadRoom.CreateRoom(hallways[(int)HallwayDirection.northEast], new Vector2Int(path[0].position.x, path[0].position.y), tilemap, true);
        //            skipFirstHallway = true;
        //        }
        //        else if(start.cardinalDirection == Direction.west && path[0].cardinalDirection == Direction.north)
        //        {
        //            LoadRoom.CreateRoom(hallways[(int)HallwayDirection.northWest], new Vector2Int(path[0].position.x, path[0].position.y), tilemap, true);
        //            skipFirstHallway = true;
        //        }
        //        else if(start.cardinalDirection == Direction.east && path[0].cardinalDirection == Direction.south)
        //        {
        //            LoadRoom.CreateRoom(hallways[(int)HallwayDirection.southEast], new Vector2Int(path[0].position.x, path[0].position.y), tilemap, true);
        //            skipFirstHallway = true;
        //        }
        //        else if(start.cardinalDirection == Direction.west && path[0].cardinalDirection == Direction.south)
        //        {
        //            LoadRoom.CreateRoom(hallways[(int)HallwayDirection.southWest], new Vector2Int(path[0].position.x, path[0].position.y), tilemap, true);
        //            skipFirstHallway = true;
        //        }
        //        else
        //        {
        //            LoadRoom.CreateRoom(hallways[(int)HallwayDirection.eastWest], new Vector2Int(start.position.x, start.position.y), tilemap, true);
        //        }
        //    }

        //    for (int i = 0; i < path.Count; i++)
        //    {
        //        if(skipFirstHallway)
        //        {
        //            skipFirstHallway = false;
        //            continue;
        //        }
        //        if(i != 0 && path[i].cardinalDirection != path[i - 1].cardinalDirection)
        //        {
        //            if (path[i].cardinalDirection == Direction.east && path[i - 1].cardinalDirection == Direction.north)
        //            {
        //                LoadRoom.CreateRoom(hallways[(int)HallwayDirection.southEast], new Vector2Int(path[i].position.x, path[i].position.y), tilemap, true);
        //            }
        //            else if (path[i].cardinalDirection == Direction.west && path[i - 1].cardinalDirection == Direction.north)
        //            {
        //                LoadRoom.CreateRoom(hallways[(int)HallwayDirection.southWest], new Vector2Int(path[i].position.x, path[i].position.y), tilemap, true);
        //            }
        //            else if (path[i].cardinalDirection == Direction.east && path[i - 1].cardinalDirection == Direction.south)
        //            {
        //                LoadRoom.CreateRoom(hallways[(int)HallwayDirection.northEast], new Vector2Int(path[i].position.x, path[i].position.y), tilemap, true);
        //            }
        //            else if (path[i].cardinalDirection == Direction.west && path[i - 1].cardinalDirection == Direction.south)
        //            {
        //                LoadRoom.CreateRoom(hallways[(int)HallwayDirection.northWest], new Vector2Int(path[i].position.x, path[i].position.y), tilemap, true);
        //            }
        //            else if (path[i].cardinalDirection == Direction.north && path[i - 1].cardinalDirection == Direction.east)
        //            {
        //                LoadRoom.CreateRoom(hallways[(int)HallwayDirection.northWest], new Vector2Int(path[i].position.x, path[i].position.y), tilemap, true);
        //            }
        //            else if (path[i].cardinalDirection == Direction.north && path[i - 1].cardinalDirection == Direction.west)
        //            {
        //                LoadRoom.CreateRoom(hallways[(int)HallwayDirection.northEast], new Vector2Int(path[i].position.x, path[i].position.y), tilemap, true);
        //            }
        //            else if (path[i].cardinalDirection == Direction.south && path[i - 1].cardinalDirection == Direction.east)
        //            {
        //                LoadRoom.CreateRoom(hallways[(int)HallwayDirection.southWest], new Vector2Int(path[i].position.x, path[i].position.y), tilemap, true);
        //            }
        //            else if (path[i].cardinalDirection == Direction.south && path[i - 1].cardinalDirection == Direction.west)
        //            {
        //                LoadRoom.CreateRoom(hallways[(int)HallwayDirection.southEast], new Vector2Int(path[i].position.x, path[i].position.y), tilemap, true);
        //            }
        //            continue;
        //        }
        //        if (path[i].cardinalDirection == Direction.north || path[i].cardinalDirection == Direction.south)
        //        {
        //            LoadRoom.CreateRoom(hallways[(int)HallwayDirection.northSouth], new Vector2Int(path[i].position.x, path[i].position.y), tilemap, true);
        //        }
        //        else
        //        {
        //            LoadRoom.CreateRoom(hallways[(int)HallwayDirection.eastWest], new Vector2Int(path[i].position.x, path[i].position.y), tilemap, true);
        //        }
        //    }
        //}

        return false;
    }

    private void OnDrawGizmos()
    {
        for(int i = 0; i < allCheckByPathfinding.Count; i++)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(new Vector3(allCheckByPathfinding[i].x + 0.5f, allCheckByPathfinding[i].y + 0.5f), new Vector3(1, 1));
        }

        for(int i = 0; i < pathForGizmos.Count; i++)
        {
            if(i == 0)
            {
                Gizmos.color = Color.blue;
            }
            else if(i < (pathForGizmos.Count - 1))
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.magenta;
            }
            Gizmos.DrawWireCube(new Vector3(pathForGizmos[i].position.x + 0.5f, pathForGizmos[i].position.y + 0.5f), new Vector3(1, 1));
        }
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3(realStart.x + 0.5f, realStart.y + 0.5f), new Vector3(1, 1));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(realEnd.x + 0.5f, realEnd.y + 0.5f), new Vector3(1, 1));

        //Gizmos.color = Color.green;
        //Gizmos.DrawWireCube(new Vector3(newBuildingEntrancePositionForGizmos.x + 0.5f, newBuildingEntrancePositionForGizmos.y + 0.5f), new Vector3(1, 1));
    }
}
