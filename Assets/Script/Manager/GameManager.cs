using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Camera
    float delay;
    float Move_smoothTime;
    float viewHeight_Y;
    float viewHeight_X;
    float harf_ViewHeaight_Y;
    float harf_ViewHeaight_X;

    [Header("�����¿� ī�޶� Ÿ��")]
    public Transform[] target;

    Vector3 velocity = Vector3.zero;

    bool isTopMap;
    bool isBottomMap;
    bool isRightMap;
    bool isLeftMap;

    //ShakeCamera
    Vector3 myVec;

    float shakeAmout = 0.3f;

    float shakeTime = 0.7f;

    [Header("[���� ����]{���Ÿ�}{�ٰŸ�}{�����۹ڽ�}{���Ϲ�}{����}")]
    public Transform[] Range_SpawnPoint;
    public Transform[] Shot_SpawnPoint;
    public Transform[] Item_SpawnPoint;
    public Transform[] Drop_SpawnPoint;
    public Transform bossSpawnPoint;

    //Start&Trigger
    [Header("������ ����")]
    public BoxCollider2D[] boss_Border;

    //Object&Script
    [Header("�÷��̾�")]
    public GameObject player;
    [Header("������Ʈ �Ŵ���")]
    public ObjectManager objectManager;
    [Header("���ͽ�ũ��Ʈ")]
    public Enemy enemyScript;

    void Awake()
    {
        //ī�޶� ������
        harf_ViewHeaight_Y = Camera.main.orthographicSize;
        harf_ViewHeaight_X = Camera.main.orthographicSize * 16 / 9;
        viewHeight_Y = harf_ViewHeaight_Y * 2;
        viewHeight_X = harf_ViewHeaight_X * 2;

        delay = 0.01f;  //�Ÿ��� 0.01���� Ÿ���������� ī�޶� �̵�
        Move_smoothTime = 0.2f; //ī�޶� �ӵ�

        SpawnEnemy();//���� ���� ������ �ӽ� ����
    }
    void Update()
    {
        TouchCamera();
        NextMap();

        BossRoom();
    }
    //ī�޶� �̵�
    void TouchCamera()
    {
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
    }       //ī�޶� Ʈ����
    void NextMap()
    {
        if (isTopMap)
        {
            Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position,
                target[0].position, ref velocity, Move_smoothTime);

            if (Vector3.Distance(target[0].position, Camera.main.transform.position) < delay)
            {
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
                isLeftMap = false;
                for (int index = 0; index < 4; index++)
                {
                    target[index].position = target[index].position + new Vector3(-(viewHeight_X - delay), 0, 0);
                }
            }
        }
    }           //���������� ī�޶� �̵�
    public void PlayerStop()
    {
        Rigidbody2D p_Rigid = player.GetComponent<Rigidbody2D>();
        p_Rigid.velocity = Vector2.zero;
    } //ī�޶� �̵��� ����  �ػ���̵��ÿ��� ���� ����

    //ī�޶� ����
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
    public void Easy()
    {
        enemyScript.level = Enemy.Level.Easy;
    }
    public void Normal()
    {
        enemyScript.level = Enemy.Level.Easy; //Normal���ý� Easy���̵� ����
    }
    public void Hard()
    {
        enemyScript.level = Enemy.Level.Hard;
    }
    void SpawnEnemy()           
    {
        CreateMonsterA();
        CreateMonsterB();
        CreateItemBox();
        CreateBoss();
    }

    public void GameReStart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void GameQuit()
    {
        Application.Quit();
    }
    
    //���� �� �����۹ڽ� ���� ����
    void CreateBoss()
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
    void CreateMonsterA()
    {
        for (int index = 0; index < Shot_SpawnPoint.Length; index++)
        { GameObject mouse = objectManager.MakeObj("mouse");
            mouse.transform.position = Shot_SpawnPoint[index].position;


            MeleeEnemy mouseLogic = mouse.GetComponent<MeleeEnemy>();

            mouseLogic.point = Shot_SpawnPoint[index];
            mouseLogic.player = player;
            mouseLogic.objectManager = objectManager;
            mouseLogic.enemyScript = enemyScript;
            mouseLogic.gameManager = this;
        }
    }
    void CreateMonsterB()
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
    void CreateItemBox()
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
    
    //������ ���Ϲ� �� ���� ����
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
            MeleeEnemy mouseLogic = mouse.GetComponent<MeleeEnemy>();

            mouseLogic.player = player;
            mouseLogic.objectManager = objectManager;
            mouseLogic.enemyScript = enemyScript;
            mouseLogic.gameManager = this;
        }
    }

    //��ȯ�� ����
    public void FollowerSpawn()
    {
        Player playerLogic = player.GetComponent<Player>();
        GameObject followCat = objectManager.MakeObj("followCat");
        followCat.transform.position = playerLogic.transform.position+Vector3.up*2;

        FollowCat followCatLogic = followCat.GetComponent<FollowCat>();
        followCatLogic.player = player;
        followCatLogic.parent = player.transform;
        followCatLogic.attack = followCat.GetComponentInChildren<BoxCollider2D>();
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
    //���� �� ������ Ŭ���� ���� �߻�(�̱���)
    public void MonsterKillCheck()
    {
        Player playerLogic = player.GetComponent<Player>();
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
            playerLogic.clearMap = true;
    }

}