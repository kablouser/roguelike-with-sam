using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct EntrancePosition
{
    public Vector2Int position;
    //0 - N, 1 - E, 2 - S, 3 - W
    public Direction cardinalDirection;

    public EntrancePosition(Vector2Int position, Direction cardinalDirection)
    {
        this.position = position;
        this.cardinalDirection = cardinalDirection;
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
                AddToFreeExpansions(roomIndex, new Vector2Int(0, 0), Direction.none);
            }
            else
            {
                bool whileLoop = true;
                while(whileLoop)
                {
                    int positionIndex = Random.Range(0, freeEntrancesCopy.Count - 1);
                    Debug.Log("PositionIndex: " + positionIndex + "roomIndex: " + roomIndex);
                    Debug.Log("FreeExpansionCount: " + freeEntrancesCopy.Count);

                    if(freeEntrancesCopy[positionIndex].cardinalDirection == Direction.north && roomPossibilitiesCopy[roomIndex].GetAvailableEntranceDirection(Direction.south) == true)
                    {
                        int xOffset = freeEntrancesCopy[positionIndex].position.x - roomPossibilitiesCopy[roomIndex].entrances[2].x;
                        int yOffset = freeEntrancesCopy[positionIndex].position.y + 3;

                        offset = new Vector2Int(xOffset, yOffset);

                        if(LoadRoom.CreateRoom(roomPossibilitiesCopy[roomIndex], offset, tilemap))
                        {
                            AddToFreeExpansions(roomIndex, offset, Direction.south);

                            //Create the hallway
                            LoadRoom.CreateRoom(hallways[0], new Vector2Int(freeEntrancesCopy[positionIndex].position.x - 1, freeEntrancesCopy[positionIndex].position.y), tilemap, true);

                            freeEntrancesCopy.RemoveAt(positionIndex);

                            whileLoop = false;
                        }
                        else
                        {
                            freeEntrancesCopy.RemoveAt(positionIndex);
                        }
                    }
                    else if(freeEntrancesCopy[positionIndex].cardinalDirection == Direction.east && roomPossibilitiesCopy[roomIndex].GetAvailableEntranceDirection(Direction.west) == true)
                    {
                        int xOffset = freeEntrancesCopy[positionIndex].position.x + 3;
                        int yOffset = freeEntrancesCopy[positionIndex].position.y - roomPossibilitiesCopy[roomIndex].entrances[3].y;

                        offset = new Vector2Int(xOffset, yOffset);

                        if(LoadRoom.CreateRoom(roomPossibilitiesCopy[roomIndex], offset, tilemap))
                        {
                            AddToFreeExpansions(roomIndex, offset, Direction.west);

                            //Create the hallway
                            LoadRoom.CreateRoom(hallways[1], new Vector2Int(freeEntrancesCopy[positionIndex].position.x, freeEntrancesCopy[positionIndex].position.y - 1), tilemap, true);

                            freeEntrancesCopy.RemoveAt(positionIndex);

                            whileLoop = false;
                        }
                        else
                        {
                            freeEntrancesCopy.RemoveAt(positionIndex);
                        }
                    }
                    else if(freeEntrancesCopy[positionIndex].cardinalDirection == Direction.south && roomPossibilitiesCopy[roomIndex].GetAvailableEntranceDirection(Direction.north) == true)
                    {
                        int xOffset = freeEntrancesCopy[positionIndex].position.x - roomPossibilitiesCopy[roomIndex].entrances[0].x;
                        int yOffset = freeEntrancesCopy[positionIndex].position.y - 2 - roomPossibilitiesCopy[roomIndex].roomHeight;

                        offset = new Vector2Int(xOffset, yOffset);

                        if(LoadRoom.CreateRoom(roomPossibilitiesCopy[roomIndex], offset, tilemap))
                        {
                            AddToFreeExpansions(roomIndex, offset, Direction.north);

                            //Create the hallway
                            LoadRoom.CreateRoom(hallways[0], new Vector2Int(freeEntrancesCopy[positionIndex].position.x - 1, freeEntrancesCopy[positionIndex].position.y - 3), tilemap, true);

                            freeEntrancesCopy.RemoveAt(positionIndex);

                            whileLoop = false;
                        }
                        else
                        {
                            freeEntrancesCopy.RemoveAt(positionIndex);
                        }
                    }
                    else if(freeEntrancesCopy[positionIndex].cardinalDirection == Direction.west && roomPossibilitiesCopy[roomIndex].GetAvailableEntranceDirection(Direction.east) == true)
                    {
                        int xOffset = freeEntrancesCopy[positionIndex].position.x - 2 - roomPossibilitiesCopy[roomIndex].roomWidth;
                        int yOffset = freeEntrancesCopy[positionIndex].position.y - roomPossibilitiesCopy[roomIndex].entrances[1].y;

                        offset = new Vector2Int(xOffset, yOffset);

                        if(LoadRoom.CreateRoom(roomPossibilitiesCopy[roomIndex], offset, tilemap))
                        {
                            AddToFreeExpansions(roomIndex, offset, Direction.east);

                            //Create the hallway
                            LoadRoom.CreateRoom(hallways[1], new Vector2Int(freeEntrancesCopy[positionIndex].position.x - 3, freeEntrancesCopy[positionIndex].position.y - 1), tilemap, true);

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
                        if(freeEntrances.Count == 0)
                        {
                            roomPossibilitiesCopy.RemoveAt(roomIndex);
                            roomIndex = Random.Range(0, roomPossibilitiesCopy.Count - 1);
                            CopyList(freeEntrances, ref freeEntrancesCopy);
                        }
                    }

                    if(freeEntrances.Count == 0 && roomPossibilitiesCopy.Count == 0)
                    {
                        whileLoop = false;
                    }
                }
            }
        }
    }

    private void AddToFreeExpansions(int roomIndex, Vector2Int offset, Direction sideConnected)
    {
        for(int i = 0; i < 4; i++)
        {
            if(roomPossibilitiesCopy[roomIndex].GetAvailableEntranceDirection((Direction)i) == true && sideConnected != (Direction)i)
            {
                freeEntrances.Add(new EntrancePosition(new Vector2Int(roomPossibilitiesCopy[roomIndex].entrances[i].x + offset.x, roomPossibilitiesCopy[roomIndex].entrances[i].y + offset.y), (Direction)i));
                //Debug.Log("RoomEntranceX: " + (roomPossibilities[roomIndex].entrances[y].x + offset.x) + " RoomEntranceY: " + (roomPossibilities[roomIndex].entrances[y].y + offset.y));
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

        for(int i = 0; i < original.Count; i++)
        {
            copy.Add(original[i]);
        }
    }
}
