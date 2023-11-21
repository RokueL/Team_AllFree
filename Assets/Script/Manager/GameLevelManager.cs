using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevelManager : MonoBehaviour
{
    public Enemy enemy;
    public GameObject meleeEnemy;
    public GameObject RangeEnemy;
    public GameObject Boss;
    public GameObject BossAttck;
    public GameObject enemyAttack;


    public void Easy()
    {
        enemy.level = Enemy.Level.Easy;
    }
    public void Normal()
    {
        enemy.level = Enemy.Level.Easy; //Normal선택시 Easy난이도 설정
    }
    public void Hard()
    {
        enemy.level = Enemy.Level.Hard;
    }
}
