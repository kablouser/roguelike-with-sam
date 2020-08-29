using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class RandomRooms : MonoBehaviour
{
    [Header("Hallways, NS then EW")]
    public Room[] hallways = new Room[2];
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

    [ContextMenu("Create Level")]
    public void GenerateRandomLevel()
    {
        freeEntrances.Clear();

        if (roomPossibilities.Count == 0)
        {
            Debug.Log("No room possibilities");
            return;
        }

        tilemap.backgroundTilemap.ClearAllTiles();

        for (int x = 0; x < numberOfRooms; x++)
        {
            CopyList(roomPossibilities, ref roomPossibilitiesCopy);
            CopyList(freeEntrances, ref freeEntrancesCopy);

            int roomIndex = Random.Range(0, roomPossibilitiesCopy.Count);

            if (x == 0)
            {
                LoadRoom.CreateRoom(roomPossibilitiesCopy[roomIndex], new Vector2Int(0, 0), tilemap);
                AddToFreeExpansions(roomIndex, new Vector2Int(0, 0), Direction.none);
            }
            else
            {
                Direction direction = Direction.none;
                bool whileLoop = true;
                while (whileLoop)
                {
                    int positionIndex = Random.Range(0, freeEntrancesCopy.Count - 1);
                    Debug.Log("PositionIndex: " + positionIndex + "roomIndex: " + roomIndex);
                    Debug.Log("FreeExpansionCount: " + freeEntrancesCopy.Count);

                    if(TryCreateRoom(freeEntrancesCopy[positionIndex], roomPossibilitiesCopy[roomIndex], ref direction))
                    {
                        if(LoadRoom.CreateRoom(roomPossibilitiesCopy[roomIndex], offset, tilemap))
                        {
                            AddToFreeExpansions(roomIndex, offset, Direction.south);

                            //Create the hallway
                            if(direction == Direction.north)
                            {
                                LoadRoom.CreateRoom(hallways[0], new Vector2Int(freeEntrancesCopy[positionIndex].position.x - 1, freeEntrancesCopy[positionIndex].position.y), tilemap, true);
                            }
                            else if(direction == Direction.east)
                            {
                                LoadRoom.CreateRoom(hallways[1], new Vector2Int(freeEntrancesCopy[positionIndex].position.x, freeEntrancesCopy[positionIndex].position.y - 1), tilemap, true);
                            }
                            else if(direction == Direction.south)
                            {
                                LoadRoom.CreateRoom(hallways[0], new Vector2Int(freeEntrancesCopy[positionIndex].position.x - 1, freeEntrancesCopy[positionIndex].position.y - 3), tilemap, true);
                            }
                            else
                            {
                                LoadRoom.CreateRoom(hallways[1], new Vector2Int(freeEntrancesCopy[positionIndex].position.x - 3, freeEntrancesCopy[positionIndex].position.y - 1), tilemap, true);
                            }

                            freeEntrancesCopy.RemoveAt(positionIndex);

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
    }

    private void AddToFreeExpansions(int roomIndex, Vector2Int offset, Direction sideConnected)
    {
        //if(roomPossibilitiesCopy[roomIndex].GetAvailableEntranceDirection(Direction.north) == true && sideConnected != Direction.north)
        //{
            for(int i = 0; i < roomPossibilitiesCopy[roomIndex].entrancesN.Length; i++)
            {
                freeEntrances.Add(new EntrancePosition(new Vector2Int(roomPossibilitiesCopy[roomIndex].entrancesN[i].x + offset.x, roomPossibilitiesCopy[roomIndex].entrancesN[i].y + offset.y), Direction.north, currentRoomNumber));
            }
        //}
        //if(roomPossibilitiesCopy[roomIndex].GetAvailableEntranceDirection(Direction.east) == true && sideConnected != Direction.east)
        //{
            for(int i = 0; i < roomPossibilitiesCopy[roomIndex].entrancesE.Length; i++)
            {
                freeEntrances.Add(new EntrancePosition(new Vector2Int(roomPossibilitiesCopy[roomIndex].entrancesE[i].x + offset.x, roomPossibilitiesCopy[roomIndex].entrancesE[i].y + offset.y), Direction.east, currentRoomNumber));
            }
        //}
        //if(roomPossibilitiesCopy[roomIndex].GetAvailableEntranceDirection(Direction.south) == true && sideConnected != Direction.south)
        //{
            for(int i = 0; i < roomPossibilitiesCopy[roomIndex].entrancesS.Length; i++)
            {
                freeEntrances.Add(new EntrancePosition(new Vector2Int(roomPossibilitiesCopy[roomIndex].entrancesS[i].x + offset.x, roomPossibilitiesCopy[roomIndex].entrancesS[i].y + offset.y), Direction.south, currentRoomNumber));
            }
        //}
        //if(roomPossibilitiesCopy[roomIndex].GetAvailableEntranceDirection(Direction.west) == true && sideConnected != Direction.west)
        //{
            for(int i = 0; i < roomPossibilitiesCopy[roomIndex].entrancesW.Length; i++)
            {
                freeEntrances.Add(new EntrancePosition(new Vector2Int(roomPossibilitiesCopy[roomIndex].entrancesW[i].x + offset.x, roomPossibilitiesCopy[roomIndex].entrancesW[i].y + offset.y), Direction.west, currentRoomNumber));
            }
        //}

        currentRoomNumber++;
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

    private bool TryCreateRoom(EntrancePosition entrance, Room room, ref Direction direction)
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
                    return true;
                }

                roomEntrancesCopy.RemoveAt(roomEntranceIndex);
            }
        }
        else if(entrance.cardinalDirection == Direction.west && room.GetAvailableEntranceDirection(Direction.east) == true)
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

                int xOffset = entrance.position.x - 2 - room.roomWidth;
                int yOffset = entrance.position.y - roomEntrancesCopy[roomEntranceIndex].y + 1;

                offset = new Vector2Int(xOffset, yOffset);

                if(LoadRoom.CanCreateRoom(room, offset, tilemap))
                {
                    direction = Direction.west;
                    return true;
                }

                roomEntrancesCopy.RemoveAt(roomEntranceIndex);
            }
        }
        return false;
    }

    public void AddHallway(int length, Vector2Int Start, Vector2Int End)
    {

    }
}
