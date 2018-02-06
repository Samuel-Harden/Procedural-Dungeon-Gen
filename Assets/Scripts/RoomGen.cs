using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGen : MonoBehaviour
{
    private int minRoomCount = 10;
    private int maxRoomCount = 1000;

    private int roomCount = 0;

    // Room Sizes
    private int smallWidth;
    private int smallLength;
    private int medWidth;
    private int medLength;
    private int largeWidth;
    private int largeLength;

    // Rooms Amounts
    private int noLargeRooms;
    private int currentLarge = 0;
    private int noMediumRooms;
    private int currentMedium = 0;
    private int noSmallRooms;
    private int currentSmall = 0;

    [SerializeField] GameObject room;

    public void GenerateRooms(List<Room> _rooms, int _roomCount, int _dungeonRadius,
        bool _randNoRooms)
    {
        roomCount = _roomCount;

        SetRoomCount(_randNoRooms);

        GenerateNoRooms(); // For each Size

        Vector3 dungeonCentre = new Vector3((float)_dungeonRadius / 2, 0.0f,
            (float)_dungeonRadius / 2);

        int counter = 0;

        Color color = Color.white;

        string layer = "SmallRoom";

        for (int i = 0; i < roomCount; i++)
        {

            float angle = Random.Range(0, 360);

            Quaternion direction = Quaternion.AngleAxis(angle, Vector3.up);

            Vector3 targetPos = direction * transform.forward *
                Random.Range(0, (_dungeonRadius / 4));

            Vector3 newPos = dungeonCentre + targetPos;

            newPos = new Vector3(Mathf.RoundToInt(newPos.x),
                0.0f, (Mathf.RoundToInt(newPos.z)));

            Vector2 roomSize = GenerateRoomSize(_dungeonRadius);

            var newRoom = Instantiate(room, newPos, Quaternion.identity);

            newRoom.transform.localScale = new Vector3(roomSize.x, 1.0f, roomSize.y);

            _rooms.Add(newRoom.GetComponent<Room>());

            newRoom.GetComponent<Room>().Initialise((int)roomSize.x, (int)roomSize.y, newPos, counter, i);

            newRoom.GetComponent<Renderer>().material.color = color;

            newRoom.layer = LayerMask.NameToLayer(layer);


            if (i == noSmallRooms)
            {
                counter++;
                color = Color.grey;
                layer = "MediumRoom";
            }

            else if (i == noSmallRooms + noMediumRooms)
            {
                counter++;
                color = Color.black;
                layer = "LargeRoom";
            }
        }
    }


    private Vector2 GenerateRoomSize(int _dungeonRadius)
    {
        float smallest_size = (float)_dungeonRadius / 100;
        float largest_size  = (float)_dungeonRadius / 100;

        if (currentSmall <= noSmallRooms)
        {
            smallest_size *= 2;
            largest_size  *= 4;

            currentSmall++;
            goto setSize;
        }

        if (currentMedium <= noMediumRooms)
        {
            smallest_size *= 4;
            largest_size  *= 6;

            currentMedium++;
            goto setSize;
        }

        if (currentLarge <= noLargeRooms)
        {
            smallest_size *= 6;
            largest_size  *= 10;

            currentLarge++;
            goto setSize;
        }

        setSize:

        Vector2 size = new Vector2(Mathf.RoundToInt(Random.Range(smallest_size, largest_size)) * 2 ,
            Mathf.RoundToInt(Random.Range(smallest_size, largest_size)) * 2);

        return size;
    }


    private void GenerateNoRooms()
    {
        float largeRoomDensity  = 4;
        float mediumRoomDensity = 12;
        float smallRoomDensity  = 84;

        float total = largeRoomDensity + mediumRoomDensity + smallRoomDensity;

        noLargeRooms  = Mathf.RoundToInt(largeRoomDensity / total * roomCount);
        noMediumRooms = Mathf.RoundToInt(mediumRoomDensity / total * roomCount);
        noSmallRooms  = Mathf.RoundToInt(smallRoomDensity / total * roomCount);
    }


    private void SetRoomCount(bool _randNoRooms)
    {
        if (!_randNoRooms)
        {
            if (roomCount >= minRoomCount && roomCount <= maxRoomCount)
                return;
        }

        roomCount = Random.Range(minRoomCount, maxRoomCount);
    }
}
