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
    public float viewHeight_Y;
    public float viewHeight_X;
    public  float harf_ViewHeaight_Y;
    public float harf_ViewHeaight_X;

    [Header("�����¿� ī�޶� Ÿ��")]
    public Transform[] target;

    Vector3 velocity = Vector3.zero;

    bool isTopMap;
    bool isBottomMap;
    bool isRightMap;
    bool isLeftMap;

    //ShakeCamera
    Vector3 myVec;


    [Header("[���� ����]����Ʈ ���� ��ų")]
    public Transform[] eliteSkill_Spot;

    [Header("[���� ����]{���Ÿ�}{�ٰŸ�}{�����۹ڽ�}{���Ϲ�}{����}")]
    public Transform[] Range_SpawnPoint;
    public Transform[] Shot_SpawnPoint;
    public Transform[] Item_SpawnPoint;
    public Transform[] Drop_SpawnPoint;
    public Transform eliteSpawnPoint;
    public Transform bossSpawnPoint;

    [Header("������ ����")]
    public BoxCollider2D[] elite_Border;
    public BoxCollider2D[] boss_Border;
    [Header("������ Ʈ����")]
    public GameObject eliteMonster_Trigger;
    public GameObject boss_Trigger;

    bool onElite;

    [Header("�÷��̾�")]
    public GameObject player;
    [Header("������Ʈ �Ŵ���")]
    public ObjectManager objectManager;
    [Header("���ͽ�ũ��Ʈ")]
    public Enemy enemyScript;

    void Awake()
    {
        delay = 0.01f;  //�Ÿ��� 0.01���� Ÿ���������� ī�޶� �̵�
        Move_smoothTime = 0.2f; //ī�޶� �ӵ�

        //ī�޶� ������
        harf_ViewHeaight_Y = Camera.main.orthographicSize;
        harf_ViewHeaight_X = Camera.main.orthographicSize * 16 / 9;
        viewHeight_Y = harf_ViewHeaight_Y * 2;
        viewHeight_X = harf_ViewHeaight_X * 2;

        //�̵� ī�޶� ��ǥ
        target[0].transform.localPosition = new Vector3(0,viewHeight_Y+delay,-1);
        target[1].transform.localPosition = new Vector3(0, -viewHeight_Y + delay, -1);
        target[2].transform.localPosition = new Vector3(viewHeight_X+delay,0, -1);
        target[3].transform.localPosition = new Vector3(-viewHeight_X + delay, 0, -1);
        

        SpawnEnemy();//���� ���� ������ �ӽ� ����
    }
    void Update()
    {
        TouchCamera();
        NextMap();

        if (eliteMonster_Trigger.activeSelf == false)
        {
            CreateElite();
            EliteRoom();
            onElite = true;
        }
        //else if (boss_Trigger.activeSelf == false)
        //    BossRoom();
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
            isBottomMap = true;
        }
        else if (player.transform.position.x + harf_ViewHeaight_X > target[2].position.x)
        {
            isRightMap = true;
        }
        if (player.transform.position.x - harf_ViewHeaight_X < target[3].position.x)
        {
            isLeftMap = true;
        }
    }       //ī�޶� Ʈ����
    void NextMap()
    {
        Player playerLogic = player.GetComponent<Player>();
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

            playerLogic.isCameraMove = true;
            if (Vector3.Distance(target[2].position, Camera.main.transform.position) < delay)
            {
                isRightMap = false;
                for (int index = 0; index < 4; index++)
                {
                    playerLogic.isCameraMove = false;

                    target[index].position = target[index].position + new Vector3(viewHeight_X - delay, 0, 0);
                }
            }
        }
        if (isLeftMap)
        {
            Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position,
                target[3].position, ref velocity, Move_smoothTime);

            playerLogic.isCameraMove = true;
            if (Vector3.Distance(target[3].position, Camera.main.transform.position) < delay)
            {
                isLeftMap = false;
                for (int index = 0; index < 4; index++)
                {
                    playerLogic.isCameraMove = false;

                    target[index].position = target[index].position + new Vector3(-(viewHeight_X - delay), 0, 0);
                }
            }
        }
    }           //���������� ī�޶� �̵�

    //ī�޶� ����
    public void ShakeCam(float power, float shakeTime)
    {
        StartCoroutine(CameraShake(power, shakeTime));
    }
    IEnumerator CameraShake(float power, float time)
    {
        myVec = Camera.main.transform.position;
        float timer = 0;
        while (timer < time)
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
    public void CreateBoss()
    {

        GameObject boss = objectManager.MakeObj("bossA");
        boss.transform.position = bossSpawnPoint.position;

        Boss bossLogic = boss.GetComponent<Boss>();
        bossLogic.player = player;
        bossLogic.objectManager = objectManager;
        bossLogic.enemyScript = enemyScript;
        bossLogic.Trigger = boss_Trigger;
        bossLogic.gameManager = this;

    }
    void CreateElite()
    {
        if (onElite)
            return;

        GameObject elite = objectManager.MakeObj("elite");
        elite.transform.position = eliteSpawnPoint.position;

        EliteEnemy eliteLogic = elite.GetComponent<EliteEnemy>();
        eliteLogic.player = player;
        eliteLogic.objectManager = objectManager;
        eliteLogic.enemyScript = enemyScript;
        eliteLogic.Trigger = boss_Trigger;
        eliteLogic.gameManager = this;
        eliteLogic.point = eliteSkill_Spot;

    }
    void CreateMonsterA()
    {
        for (int index = 0; index < Shot_SpawnPoint.Length; index++)
        {
            GameObject mouse = objectManager.MakeObj("mouse");
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

            RangeEnemy snakeLogic = snake.GetComponent<RangeEnemy>();
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
        int ranCount = Random.Range(3, 6);

        for (int index = 0; index < ranCount; index++)
        {
            int ranObj = Random.Range(0, 3);
            int ranPoint = Random.Range(0, Drop_SpawnPoint.Length);

            string type = null;
            switch (ranObj)
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
        followCat.transform.position = playerLogic.transform.position + Vector3.up * 2;
        StartCoroutine(TeleportParticle(followCat.transform));

        FollowCat followCatLogic = followCat.GetComponent<FollowCat>();
        followCatLogic.player = player;
        followCatLogic.gameManager = this;
        followCatLogic.objectManager = objectManager;
        followCatLogic.parent = player.transform;
        followCatLogic.attack = followCat.GetComponentInChildren<BoxCollider2D>();

    }
    public IEnumerator TeleportParticle(Transform target)
    {
        GameObject teleport_Effect = objectManager.MakeObj("teleport");
        ParticleSystem teleportLogic = teleport_Effect.GetComponent<ParticleSystem>();
        teleport_Effect.transform.position = target.position;
        teleportLogic.Play();
        yield return new WaitForSeconds(2f);

        teleport_Effect.SetActive(false);
    }

    //Trigger
    void BossRoom()
    {
        for (int index = 0; index < boss_Border.Length; index++)
        {
            boss_Border[index].enabled = true;
        }

    }
    void EliteRoom()
    {
        for (int index = 0; index < elite_Border.Length; index++)
        {
            elite_Border[index].enabled = true;
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