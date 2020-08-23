using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct FreeExpansionLocations
{
    public Vector2Int position;
    //0 - N, 1 - E, 2 - S, 3 - W
    public int cardinalDirection;

    public FreeExpansionLocations(Vector2Int position, int cardinalDirection)
    {
        this.position = position;
        this.cardinalDirection = cardinalDirection;
    }
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

    private List<FreeExpansionLocations> freeExpansions = new List<FreeExpansionLocations>();
    private List<FreeExpansionLocations> tempSaveOfFreeExpansions = new List<FreeExpansionLocations>();

    private List<Room> TempRoomPossibilities = new List<Room>();

    private Vector2Int offset;

    [ContextMenu("Create Level")]
    public void GenerateRandomLevel()
    {
        if(roomPossibilities.Count == 0)
        {
            Debug.Log("No room possibilities");
            return;
        }

        tilemap.backgroundTilemap.ClearAllTiles();

        for(int x = 0; x < numberOfRooms; x++)
        {
            TempRoomPossibilities = roomPossibilities;
            tempSaveOfFreeExpansions = freeExpansions;

            int roomIndex = Random.Range(0, TempRoomPossibilities.Count - 1);

            if(x == 0)
            {
                LoadRoom.CreateRoom(TempRoomPossibilities[roomIndex], new Vector2Int(0, 0), tilemap);
                AddToFreeExpansions(roomIndex, new Vector2Int(0, 0), 5);
            }
            else
            {
                bool whileLoop = true;
                while(whileLoop)
                {
                    int positionIndex = Random.Range(0, freeExpansions.Count - 1);
                    Debug.Log("PositionIndex: " + positionIndex + "roomIndex: " + roomIndex);
                    Debug.Log("FreeExpansionCount: " + freeExpansions.Count);

                    if(freeExpansions[positionIndex].cardinalDirection == 0 && TempRoomPossibilities[roomIndex].availableEntranceDirections[2] == true)
                    {
                        int xOffset = freeExpansions[positionIndex].position.x - TempRoomPossibilities[roomIndex].entrances[2].x;
                        int yOffset = freeExpansions[positionIndex].position.y + 3;

                        offset = new Vector2Int(xOffset, yOffset);

                        if(LoadRoom.CreateRoom(TempRoomPossibilities[roomIndex], offset, tilemap))
                        {
                            AddToFreeExpansions(roomIndex, offset, 2);

                            //Create the hallway
                            LoadRoom.CreateRoom(hallways[0], new Vector2Int(freeExpansions[positionIndex].position.x - 1, freeExpansions[positionIndex].position.y), tilemap, true);

                            freeExpansions.RemoveAt(positionIndex);

                            whileLoop = false;
                        }
                        else
                        {
                            freeExpansions.RemoveAt(positionIndex);
                        }
                    }
                    else if(freeExpansions[positionIndex].cardinalDirection == 1 && TempRoomPossibilities[roomIndex].availableEntranceDirections[3] == true)
                    {
                        int xOffset = freeExpansions[positionIndex].position.x + 3;
                        int yOffset = freeExpansions[positionIndex].position.y - TempRoomPossibilities[roomIndex].entrances[3].y;

                        offset = new Vector2Int(xOffset, yOffset);

                        if(LoadRoom.CreateRoom(TempRoomPossibilities[roomIndex], offset, tilemap))
                        {
                            AddToFreeExpansions(roomIndex, offset, 3);

                            //Create the hallway
                            LoadRoom.CreateRoom(hallways[1], new Vector2Int(freeExpansions[positionIndex].position.x, freeExpansions[positionIndex].position.y - 1), tilemap, true);

                            freeExpansions.RemoveAt(positionIndex);

                            whileLoop = false;
                        }
                        else
                        {
                            freeExpansions.RemoveAt(positionIndex);
                        }
                    }
                    else if(freeExpansions[positionIndex].cardinalDirection == 2 && TempRoomPossibilities[roomIndex].availableEntranceDirections[0] == true)
                    {
                        int xOffset = freeExpansions[positionIndex].position.x - TempRoomPossibilities[roomIndex].entrances[0].x;
                        int yOffset = freeExpansions[positionIndex].position.y - 2 - TempRoomPossibilities[roomIndex].roomHeight;

                        offset = new Vector2Int(xOffset, yOffset);

                        if(LoadRoom.CreateRoom(TempRoomPossibilities[roomIndex], offset, tilemap))
                        {
                            AddToFreeExpansions(roomIndex, offset, 0);

                            //Create the hallway
                            LoadRoom.CreateRoom(hallways[0], new Vector2Int(freeExpansions[positionIndex].position.x - 1, freeExpansions[positionIndex].position.y - 3), tilemap, true);

                            freeExpansions.RemoveAt(positionIndex);

                            whileLoop = false;
                        }
                        else
                        {
                            freeExpansions.RemoveAt(positionIndex);
                        }
                    }
                    else if(freeExpansions[positionIndex].cardinalDirection == 3 && TempRoomPossibilities[roomIndex].availableEntranceDirections[1] == true)
                    {
                        int xOffset = freeExpansions[positionIndex].position.x - 2 - TempRoomPossibilities[roomIndex].roomWidth;
                        int yOffset = freeExpansions[positionIndex].position.y - TempRoomPossibilities[roomIndex].entrances[3].y;

                        offset = new Vector2Int(xOffset, yOffset);

                        if(LoadRoom.CreateRoom(TempRoomPossibilities[roomIndex], offset, tilemap))
                        {
                            AddToFreeExpansions(roomIndex, offset, 1);

                            //Create the hallway
                            LoadRoom.CreateRoom(hallways[1], new Vector2Int(freeExpansions[positionIndex].position.x - 3, freeExpansions[positionIndex].position.y - 1), tilemap, true);

                            freeExpansions.RemoveAt(positionIndex);

                            whileLoop = false;
                        }
                        else
                        {
                            freeExpansions.RemoveAt(positionIndex);
                        }
                    }
                    else
                    {
                        if(freeExpansions.Count == 0)
                        {
                            TempRoomPossibilities.RemoveAt(roomIndex);
                            roomIndex = Random.Range(0, TempRoomPossibilities.Count - 1);
                            freeExpansions = tempSaveOfFreeExpansions;
                        }
                    }

                    if(freeExpansions.Count == 0 && TempRoomPossibilities.Count == 0)
                    {
                        whileLoop = false;
                    }
                }
            }
        }

        freeExpansions.Clear();
    }

    private void AddToFreeExpansions(int roomIndex, Vector2Int offset, int sideConnected)
    {
        for(int y = 0; y < 4; y++)
        {
            if(TempRoomPossibilities[roomIndex].availableEntranceDirections[y] == true && sideConnected != y)
            {
                freeExpansions.Add(new FreeExpansionLocations(new Vector2Int(TempRoomPossibilities[roomIndex].entrances[y].x + offset.x, TempRoomPossibilities[roomIndex].entrances[y].y + offset.y), y));
                //Debug.Log("RoomEntranceX: " + (roomPossibilities[roomIndex].entrances[y].x + offset.x) + " RoomEntranceY: " + (roomPossibilities[roomIndex].entrances[y].y + offset.y));
            }
        }
    }
}
