using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Camera
    public float delay;
    public float Move_smoothTime;
    public float Start_smoothTime;
    float viewHeight_Y;
    float viewHeight_X;
    float harf_ViewHeaight_Y;
    float harf_ViewHeaight_X;

    public Transform[] target;
    public Transform StartCameraPos;

    Vector3 velocity = Vector3.zero;

    bool isTopMap;
    bool isBottomMap;
    bool isRightMap;
    bool isLeftMap;

    bool isCameraMoving;
    //ShakeCamera
    Vector3 myVec;

    float shakeAmout = 0.3f;

    float shakeTime = 0.7f;

    //Spawn
    public Transform[] Range_SpawnPoint;
    public Transform[] Shot_SpawnPoint;
    public Transform[] Item_SpawnPoint;
    public Transform[] Drop_SpawnPoint;

    //Start&Trigger
    public BoxCollider2D[] boss_Border;

    public Transform bossSpawnPoint;

    public GameObject reTry;

    bool isStart;

    //UI
    public GameObject gameStart;
    public GameObject[] level;


    //Object&Script
    public GameObject player;
    public ObjectManager objectManager;
    public Enemy enemyScript;
    public GameLevelManager gameLevelManager;

    void Awake()
    {
        harf_ViewHeaight_Y = Camera.main.orthographicSize;
        harf_ViewHeaight_X = Camera.main.orthographicSize * 16 / 9;
        viewHeight_Y = harf_ViewHeaight_Y * 2;
        viewHeight_X = harf_ViewHeaight_X * 2;

        gameLevelManager.enemy = enemyScript;
    }
    void Update()
    {
        StartMove();
        TouchCamera();
        NextMap();

        BossRoom();
    }
    //Camera
    void TouchCamera()
    {
        if (isCameraMoving)
            return;
        if (player.transform.position.y + harf_ViewHeaight_Y > target[0].position.y)
        {
            isTopMap = true;
        }
        else if (player.transform.position.y - harf_ViewHeaight_Y < target[1].position.y)
        {
            PlayerStop();
            isBottomMap = true;
        }
        else if (player.transform.position.x + harf_ViewHeaight_X > target[2].position.x)
        {
            PlayerStop();
            isRightMap = true;
        }
        if (player.transform.position.x - harf_ViewHeaight_X < target[3].position.x)
        {
            PlayerStop();
            isLeftMap = true;
        }
    }
    void NextMap()
    {
        if (isCameraMoving)
            return;
        if (isTopMap)
        {
            Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position,
                target[0].position, ref velocity, Move_smoothTime);

            if (Vector3.Distance(target[0].position, Camera.main.transform.position) < delay)
            {
                Debug.Log("Camera");
                isTopMap = false;
                for (int index = 0; index < 4; index++)
                {
                    target[index].position = target[index].position + new Vector3(0, viewHeight_Y - delay, 0);
                }
            }
        }
        if (isBottomMap)
        {
            Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position,
                target[1].position, ref velocity, Move_smoothTime);

            if (Vector3.Distance(target[1].position, Camera.main.transform.position) < delay)
            {
                Debug.Log("Camera");
                isBottomMap = false;
                for (int index = 0; index < 4; index++)
                {
                    target[index].position = target[index].position + new Vector3(0, -(viewHeight_Y - delay), 0);
                }
            }
        }
        if (isRightMap)
        {
            Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position,
                target[2].position, ref velocity, Move_smoothTime);

            if (Vector3.Distance(target[2].position, Camera.main.transform.position) <delay)
            {
                Debug.Log("Camera");
                isRightMap = false;
                for(int index=0;index<4;index++)
                {
                    target[index].position = target[index].position + new Vector3(viewHeight_X - delay,0,0);
                }
            }
        }
        if (isLeftMap)
        {
            Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position,
                target[3].position, ref velocity, Move_smoothTime);

            if (Vector3.Distance(target[3].position, Camera.main.transform.position) < delay)
            {
                Debug.Log("Camera");
                isLeftMap = false;
                for (int index = 0; index < 4; index++)
                {
                    target[index].position = target[index].position + new Vector3(-(viewHeight_X - delay), 0, 0);
                }
            }
        }
    }
    void StartMove()
    {
        Player playerLogic = player.GetComponent<Player>();

        if (isStart)
        {
            Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position,
                  StartCameraPos.position, ref velocity, Start_smoothTime);
            isCameraMoving = true;
        }
        if (Vector3.Distance(StartCameraPos.position, Camera.main.transform.position) < delay)
        {
            isStart = false;
            isCameraMoving = false;
            playerLogic.isStart = true;
        }
    }
    public void PlayerStop()
    {
        Rigidbody2D p_Rigid = player.GetComponent<Rigidbody2D>();
        p_Rigid.velocity = Vector2.zero;
    }
    public void ShakeCam()
    {
        StartCoroutine(CameraShake(shakeAmout, shakeTime));
    }
    IEnumerator CameraShake(float power,float time)
    {
        myVec = Camera.main.transform.position;
        float timer = 0;
        while(timer<time)
        {
            Camera.main.transform.position = (Vector3)UnityEngine.Random.insideUnitSphere * power + myVec;

            timer += Time.deltaTime;

            yield return null;
        }

        Camera.main.transform.position = myVec;
    }

    //UI
    public void GameStart()
    {
        gameStart.SetActive(false);

        level[0].SetActive(true);
        level[1].SetActive(true);
        level[2].SetActive(true);
    }
    public void Easy()
    {
        gameLevelManager.Easy();
        SelectLevel();
    }
    public void Normal()
    {
        gameLevelManager.Normal();
        SelectLevel();
    }
    public void Hard()
    {
        gameLevelManager.Hard();
        SelectLevel();
    }
    void SelectLevel()
    {
        level[0].SetActive(false);
        level[1].SetActive(false);
        level[2].SetActive(false);
        isStart = true;

        CreateMonsterA();
        CreateMonsterB();
        CreateItemBox();
        CreateBoss();
    }

    public void GameOver()
    {
        reTry.SetActive(true);
    }
    public void GameReStart()
    {
        SceneManager.LoadScene(0);
    }
    
    //Create
    public void CreateBoss()
    {
        if (bossSpawnPoint == null)
            return;
        GameObject boss = objectManager.MakeObj("bossA");
        boss.transform.position = bossSpawnPoint.position;

        Boss bossLogic = boss.GetComponent<Boss>();
        bossLogic.player = player;
        bossLogic.objectManager = objectManager;
        bossLogic.enemyScript = enemyScript;
        bossLogic.gameManager = this;

    }
    public void CreateMonsterA()
    {
        for (int index = 0; index < Shot_SpawnPoint.Length; index++)
        { GameObject mouse = objectManager.MakeObj("mouse");
            mouse.transform.position = Shot_SpawnPoint[index].position;


            ShotRangeEnemy mouseLogic = mouse.GetComponent<ShotRangeEnemy>();

            mouseLogic.point = Shot_SpawnPoint[index];
            mouseLogic.player = player;
            mouseLogic.objectManager = objectManager;
            mouseLogic.enemyScript = enemyScript;
            mouseLogic.gameManager = this;
        }
    }
    public void CreateMonsterB()
    {
        for (int index = 0; index < Range_SpawnPoint.Length; index++)
        {
            GameObject snake = objectManager.MakeObj("snake");
            snake.transform.position = Range_SpawnPoint[index].position;

            LongRangeEnemy snakeLogic = snake.GetComponent<LongRangeEnemy>();
            snakeLogic.player = player;
            snakeLogic.objectManager = objectManager;
            snakeLogic.enemyScript = enemyScript;
            snakeLogic.gameManager = this;
        }
    }
    public void CreateItemBox()
    {
        for (int index = 0; index < Item_SpawnPoint.Length; index++)
        {
            GameObject itemBox = objectManager.MakeObj("itemBox");
            itemBox.transform.position = Item_SpawnPoint[index].position;

            ItemBox ItemBoxLogic = itemBox.GetComponent<ItemBox>();
            SpriteRenderer sprite = itemBox.GetComponent<SpriteRenderer>();
            ItemBoxLogic.objectManager = objectManager;

            if (itemBox.transform.position.x > 0)
                sprite.flipX = true;

        }
    }

    public void DropDebris()
    {
        int ranCount= Random.Range(3, 6);

        for(int index=0;index<ranCount;index++)
        {
            int ranObj = Random.Range(0, 3);
            int ranPoint = Random.Range(0, Drop_SpawnPoint.Length);

            string type=null;
            switch(ranObj)
            {
                case 0:
                    type = "debrisA";
                    break;
                case 1:
                    type = "debrisB";
                    break;
                case 2:
                    type = "debrisC";
                    break;
            }

            GameObject debris = objectManager.MakeObj(type);
            debris.transform.position = Drop_SpawnPoint[ranPoint].position;
        }
    }
    public void SpawnMouse()
    {
        for (int index = 0; index < 2; index++)
        {
            int ranPoint = Random.Range(0, Drop_SpawnPoint.Length);
            GameObject mouse = objectManager.MakeObj("mouse");

            mouse.transform.position = Drop_SpawnPoint[ranPoint].position;
            ShotRangeEnemy mouseLogic = mouse.GetComponent<ShotRangeEnemy>();

            mouseLogic.player = player;
            mouseLogic.objectManager = objectManager;
            mouseLogic.enemyScript = enemyScript;
            mouseLogic.gameManager = this;
        }
    }

    //Trigger
    void BossRoom()
    {
        Player playerLogic = player.GetComponent<Player>();
        if(playerLogic.isBoss)
        {
            for(int index=0;index< boss_Border.Length;index++)
            {
                boss_Border[index].enabled = true;
            }

        }
    }
    public void MonsterKillCheck()
    {
        Player playerLogic = player.GetComponent<Player>();
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
            playerLogic.clearMap = true;
    }

}