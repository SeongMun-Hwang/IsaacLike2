using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour
{
    //RoomControl
    bool isCleared = false;

    //Door Control
    List<GameObject> doors;
    public GameObject topDoor;
    public GameObject bottomDoor;
    public GameObject leftDoor;
    public GameObject rightDoor;
    bool isDoorOpened = false;

    //Enemy Control
    public List<GameObject> enemies;
    public List<GameObject> spawnableEnemyList;

    //Player Reward
    public GameObject rewardCanvas;
    public GameObject horizontalLayoutGroup;
    public List<GameObject> rewardCards;

    List<Button> rewardButtons = new List<Button>();
    Color selectedColor = Color.red;
    Color originColor = Color.white;
    public int buttonIndex = 0;

    //cinemachine camera border
    public PolygonCollider2D border;

    private void Start()
    {
        doors= new List<GameObject>();
        doors.Add(topDoor);
        doors.Add(bottomDoor);
        doors.Add(leftDoor);
        doors.Add(rightDoor);
        if (!isCleared)
        {
            //시작 시 모든 몬스터 스폰에서 몬스터 리스트의 몬스터 생성
            Transform monsterSpawnPosition = transform.Find("MonsterSpawnPosition");
            if (monsterSpawnPosition != null)
            {
                foreach (Transform child in monsterSpawnPosition)
                {
                    int number = UnityEngine.Random.Range(0, spawnableEnemyList.Count);
                    GameObject go = Instantiate(spawnableEnemyList[number], child.transform.position, spawnableEnemyList[number].transform.rotation);
                    enemies.Add(go);
                }
            }
        }
    }
    void Update()
    {
        if (enemies.Count == 0 && !isDoorOpened)
        {
            isDoorOpened = true;
            isCleared = true;
            foreach (GameObject door in doors)
            {
                if (door.GetComponent<Door>().connectedDoor != null)
                {
                    door.GetComponent<Animator>().SetTrigger("Open");
                }
            }
            if (rewardCanvas != null)
            {
                CreateRandomRewardCards();
            }
        }
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            if (enemies[i] == null)
            {
                enemies.RemoveAt(i);
            }
        }
        if (rewardCanvas != null && rewardCanvas.activeSelf)
        {
            ChooseRewards();
        }
    }
    void CreateRandomRewardCards()
    {
        rewardCanvas.SetActive(true);
        Time.timeScale = 0f;
        List<GameObject> buttons = rewardCards;
        for (int i = 0; i < 3; i++)
        {
            int num = UnityEngine.Random.Range(0, buttons.Count);
            GameObject go = Instantiate(buttons[num], horizontalLayoutGroup.transform);
            buttons.RemoveAt(num);
            rewardButtons.Add(go.GetComponent<Button>());
        }
    }
    void ChooseRewards()
    {
        for (int i = 0; i < rewardButtons.Count; i++)
        {
            ColorBlock color = rewardButtons[i].colors;
            if (i == buttonIndex)
            {
                rewardButtons[i].Select();
                color.selectedColor = selectedColor;
            }
            else
            {
                color.selectedColor = originColor;
            }
            rewardButtons[i].colors = color;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            buttonIndex++;
            if (buttonIndex >= rewardButtons.Count)
            {
                buttonIndex = 0;
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            buttonIndex--;
            if (buttonIndex < 0)
            {
                buttonIndex = rewardButtons.Count - 1;
            }
        }
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            rewardButtons[buttonIndex].onClick.Invoke();
        }
    }
}