using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public GameObject snakeAttackPrb;
    public GameObject flowerAttackPrb;

    public GameObject bossAAttack_0Prb; //Scratch
    public GameObject bossAAttack_1Prb; //Scale

    public GameObject debrisAPrb;
    public GameObject debrisBPrb;
    public GameObject debrisCPrb;

    public GameObject bossAPrb;

    public GameObject mousePrb;
    public GameObject snakePrb;

    public GameObject goldPrb;
    public GameObject silverPrb;
    public GameObject bronzePrb;

    public GameObject coreAPrb;
    public GameObject coreBPrb;
    public GameObject coreCPrb;

    public GameObject damage_CorePrb;
    public GameObject speed_CorePrb;
    public GameObject health_CorePrb;

    public GameObject itemBoxPrb;
    //Attack Object

    GameObject[] snakeAttack;
    GameObject[] flowerAttack;

    GameObject[] bossAAttack_0;
    GameObject[] bossAAttack_1;

    GameObject[] debrisA;
    GameObject[] debrisB;
    GameObject[] debrisC;


    //Enemy
    GameObject[] bossA;

    GameObject[] mouse;
    GameObject[] snake;

    //Item
    GameObject[] gold;
    GameObject[] silver;
    GameObject[] bronze;

    GameObject[] coreA;
    GameObject[] coreB;
    GameObject[] coreC;

    GameObject[] damage_Core;
    GameObject[] speed_Core;
    GameObject[] health_Core;

    GameObject[] itemBox;



    GameObject[] targetPool;

    void Awake()
    {
        snakeAttack = new GameObject[15];
        flowerAttack = new GameObject[30];

        bossAAttack_0 = new GameObject[3];
        bossAAttack_1 = new GameObject[30];

        debrisA = new GameObject[20];
        debrisB= new GameObject[20];
        debrisC = new GameObject[20];

        bossA = new GameObject[2];

        mouse = new GameObject[10];
        snake = new GameObject[10];

        gold = new GameObject[30];
        silver = new GameObject[30];
        bronze = new GameObject[30];

        coreA = new GameObject[1];
        coreB = new GameObject[1];
        coreC = new GameObject[1];

        damage_Core = new GameObject[5];
        speed_Core = new GameObject[5];
        health_Core = new GameObject[5];

        itemBox = new GameObject[15];

        Generate();
    }
    void Generate()
    {
        for (int index=0;index<snakeAttack.Length;index++)
        {
            snakeAttack[index] = Instantiate(snakeAttackPrb);
            snakeAttack[index].SetActive(false);
        }
        for (int index = 0; index < flowerAttack.Length; index++)
        {
            flowerAttack[index] = Instantiate(flowerAttackPrb);
            flowerAttack[index].SetActive(false);
        }

        for (int index = 0; index < bossAAttack_0.Length; index++)
        {
            bossAAttack_0[index] = Instantiate(bossAAttack_0Prb);
            bossAAttack_0[index].SetActive(false);
        }
        for (int index = 0; index < bossAAttack_1.Length; index++)
        {
            bossAAttack_1[index] = Instantiate(bossAAttack_1Prb);
            bossAAttack_1[index].SetActive(false);
        }

        for (int index = 0; index < debrisA.Length; index++)
        {
            debrisA[index] = Instantiate(debrisAPrb);
            debrisA[index].SetActive(false);
        }
        for (int index = 0; index < debrisB.Length; index++)
        {
            debrisB[index] = Instantiate(debrisBPrb);
            debrisB[index].SetActive(false);
        }
        for (int index = 0; index < debrisC.Length; index++)
        {
            debrisC[index] = Instantiate(debrisCPrb);
            debrisC[index].SetActive(false);
        }

        for (int index = 0; index < bossA.Length; index++)
        {
            bossA[index] = Instantiate(bossAPrb);
            bossA[index].SetActive(false);
        }

        for (int index = 0; index < mouse.Length; index++)
        {
            mouse[index] = Instantiate(mousePrb);
            mouse[index].SetActive(false);
        }
        for (int index = 0; index < snake.Length; index++)
        {
            snake[index] = Instantiate(snakePrb);
            snake[index].SetActive(false);
        }

        for (int index = 0; index < gold.Length; index++)
        {
            gold[index] = Instantiate(goldPrb);
            gold[index].SetActive(false);
        }
        for (int index = 0; index < silver.Length; index++)
        {
            silver[index] = Instantiate(silverPrb);
            silver[index].SetActive(false);
        }
        for (int index = 0; index < bronze.Length; index++)
        {
            bronze[index] = Instantiate(bronzePrb);
            bronze[index].SetActive(false);
        }

        for (int index = 0; index < coreA.Length; index++)
        {
            coreA[index] = Instantiate(coreAPrb);
            coreA[index].SetActive(false);
        }
        for (int index = 0; index < coreB.Length; index++)
        {
            coreB[index] = Instantiate(coreBPrb);
            coreB[index].SetActive(false);
        }
        for (int index = 0; index < coreC.Length; index++)
        {
            coreC[index] = Instantiate(coreCPrb);
            coreC[index].SetActive(false);
        }

        for (int index = 0; index < damage_Core.Length; index++)
        {
            damage_Core[index] = Instantiate(damage_CorePrb);
            damage_Core[index].SetActive(false);
        }
        for (int index = 0; index < speed_Core.Length; index++)
        {
            speed_Core[index] = Instantiate(speed_CorePrb);
            speed_Core[index].SetActive(false);
        }
        for (int index = 0; index < health_Core.Length; index++)
        {
            health_Core[index] = Instantiate(health_CorePrb);
            health_Core[index].SetActive(false);
        }

        for (int index = 0; index < itemBox.Length; index++)
        {
            itemBox[index] = Instantiate(itemBoxPrb);
            itemBox[index].SetActive(false);
        }
    }
    public GameObject MakeObj(string type)
    {
        switch(type)
        {
            case "snakeAttack":
                targetPool = snakeAttack;
                break;
            case "flowerAttack":
                targetPool = flowerAttack;
                break;

            case "bossAAttack_0":
                targetPool = bossAAttack_0;
                break;
            case "bossAAttack_1":
                targetPool = bossAAttack_1;
                break;

            case "debrisA":
                targetPool = debrisA;
                break;
            case "debrisB":
                targetPool = debrisB;
                break;
            case "debrisC":
                targetPool = debrisC;
                break;

            case "bossA":
                targetPool = bossA;
                break;

            case "mouse":
                targetPool = mouse;
                break;
            case "snake":
                targetPool = snake;
                break;

            case "gold":
                targetPool = gold;
                break;
            case "silver":
                targetPool = silver;
                break;
            case "bronze":
                targetPool = bronze;
                break;

            case "coreA":
                targetPool = coreA;
                break;
            case "coreB":
                targetPool = coreB;
                break;
            case "coreC":
                targetPool = coreC;
                break;

            case "damage_Core":
                targetPool = damage_Core;
                break;
            case "speed_Core":
                targetPool = speed_Core;
                break;
            case "health_Core":
                targetPool = health_Core;
                break;

            case "itemBox":
                targetPool = itemBox;
                break;
        }
        for(int index=0;index<targetPool.Length;index++)
        {
            if(!targetPool[index].activeSelf)
            {
                targetPool[index].SetActive(true);
                return targetPool[index];
            }
        }
        return null;
    }
}
