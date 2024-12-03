using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room
{
    public GameObject room;
    public Vector3 worldPos = Vector3.zero;
}

public class GameManager : MonoBehaviour
{
    public List<GameObject> roomsPrefab;
    public GameObject baseRoomPrefab;
    public int roomNumber;

    public int row = 10;
    public int col = 10;

    float horizontalDistance = 34;
    float verticalDistance = 22;

    private Room[,] roomGrid;
    private bool[,] isRoomPosDisabled;

    //Ȯ�� ����
    Vector2Int[] directions = new Vector2Int[]
{
            new Vector2Int(0,1),
            new Vector2Int(0,-1),
            new Vector2Int(1,0),
            new Vector2Int(-1,0),
};
    private void Start()
    {
        roomGrid = new Room[row, col];
        isRoomPosDisabled = new bool[row, col];
        //�⺻ ��(��x) ����
        Room baseRoom = new Room();
        baseRoom.worldPos.x = 0;
        baseRoom.worldPos.y = 0;
        baseRoom.room = Instantiate(baseRoomPrefab, baseRoom.worldPos, Quaternion.identity);
        //�迭 ��� ��ǥ ����
        int centerX = row / 2;
        int centerY = col / 2;
        isRoomPosDisabled[centerX, centerY] = true;
        roomGrid[centerX, centerY] = baseRoom;
        //�� ����
        CreateMap(centerX, centerY);
        ConnectDoor();
    }
    public void CreateMap(int startX, int startY)
    {
        //DFS
        Stack<Vector2Int> currentRoomPos = new Stack<Vector2Int>();
        //�⺻ �� �迭 �ε��� �߰�
        currentRoomPos.Push(new Vector2Int(startX, startY));
        //������ �� ����(����Ʈ:�⺻ ��)
        int createdRooms = 1;
        List<Vector2Int> picked = new List<Vector2Int>();

        while (createdRooms < roomNumber) //������ ���� ������ �������� ������
        {
            if (currentRoomPos.Count == 0) break; //���� ������ ���� ������

            Vector2Int currentPos = currentRoomPos.ElementAt(Random.Range(0, currentRoomPos.Count));

            Vector2Int dir;
            //��� ������ Ȯ��������
            if (picked.Count == directions.Length)
            {
                picked.Clear();
                currentRoomPos.Pop();
                continue;
            }
            //Ȯ�� ���� ����
            do
            {
                dir = directions[Random.Range(0, directions.Length)];
            }
            while (picked.Contains(dir));

            int nextX = currentPos.x + dir.x;
            int nextY = currentPos.y + dir.y;
            if (CheckNextRoomPosition(nextX, nextY)) //�� ���� �������� Ȯ��
            {
                GameObject go = roomsPrefab[Random.Range(0, roomsPrefab.Count)];
                CreateRoom(nextX, nextY, go);

                currentRoomPos.Push(new Vector2Int(nextX, nextY)); //�����ϰ� ������ �� ��ǥ Ǫ��
                picked.Clear();
                createdRooms++;
            }
            picked.Add(dir);
        }
    }
    Room CreateRoom(int x, int y, GameObject room)
    {
        float worldPosX = (x - row / 2) * horizontalDistance;
        float worldPosY = (y - col / 2) * verticalDistance;

        Room newRoom = new Room
        {
            worldPos = new Vector3(worldPosX, worldPosY),
            room = Instantiate(room, new Vector3(worldPosX, worldPosY, 0), Quaternion.identity)
        };
        newRoom.room.SetActive(false);
        roomGrid[x, y] = newRoom;
        isRoomPosDisabled[x, y] = true;

        return newRoom;
    }
    bool CheckNextRoomPosition(int x, int y)
    {
        if (x < 0 || x >= row || y < 0 || y >= col) return false;
        if (isRoomPosDisabled[x, y]) return false;
        int count = 0;
        foreach (Vector2Int dir in directions)
        {
            int nextX = x + dir.x;
            int nextY = y + dir.y;
            //�����Ϸ��� ��ǥ ������ ���� 2�� �̻� ������ false(�ʹ� ���� ��ġ�� �� ���� ����)
            if (nextX >= 0 && nextX < row && nextY >= 0 && nextY < col && roomGrid[nextX, nextY] != null)
            {
                count++;
                if (count >= 2)
                {
                    return false;
                }
            }
        }
        return true;
    }
    void ConnectDoor()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (roomGrid[i, j] != null)
                {
                    RoomManager roomManager = roomGrid[i, j].room.GetComponent<RoomManager>();
                    foreach (Vector2Int dir in directions)
                    {
                        int neighborX = i + dir.x;
                        int neighborY = j + dir.y;

                        // ��� ���� Ȯ��
                        if (neighborX >= 0 && neighborX < row && neighborY >= 0 && neighborY < col && roomGrid[neighborX, neighborY] != null)
                        {
                            // ���⿡ ���� ���� ó��
                            if (dir == new Vector2Int(1, 0)) // ������
                            {
                                roomManager.rightDoor.GetComponent<Door>().connectedDoor = roomGrid[neighborX, neighborY].room.GetComponent<RoomManager>().leftDoor;
                            }
                            else if (dir == new Vector2Int(-1, 0)) // ����
                            {
                                roomManager.leftDoor.GetComponent<Door>().connectedDoor = roomGrid[neighborX, neighborY].room.GetComponent<RoomManager>().rightDoor;
                            }
                            else if (dir == new Vector2Int(0, 1)) // ����
                            {
                                roomManager.topDoor.GetComponent<Door>().connectedDoor = roomGrid[neighborX, neighborY].room.GetComponent<RoomManager>().bottomDoor;
                            }
                            else if (dir == new Vector2Int(0, -1)) // �Ʒ���
                            {
                                roomManager.bottomDoor.GetComponent<Door>().connectedDoor = roomGrid[neighborX, neighborY].room.GetComponent<RoomManager>().topDoor; ;
                            }
                        }
                    }
                }
            }
        }
    }
}