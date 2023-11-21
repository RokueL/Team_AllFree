using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteEnemy : Enemy
{
    public Transform point;
    public Enemy enemyScript;

    float frontPos;

    public BoxCollider2D attack;
    public BoxCollider2D double_Attack;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        forward = new Vector2(speed, rigid.velocity.y).normalized;
        maxAttackDelay = 4;
        speed = 2;
    }
    //체력설정

    void OnEnable()
    {
        StartCoroutine(LevelCheck());
    }
    void FixedUpdate()
    {
        curAttackDelay += Time.deltaTime;
        MoveAnim();
        WatchCheck();
        Follow();
        Move();
        //GroundCheck();
        if (isDie)
            rigid.velocity = Vector2.zero;
        if (curAttackDelay > maxAttackDelay)
        {
            StartCoroutine(Attack());
            curAttackDelay = 0;
        }
    }

    //Start Setting
    IEnumerator LevelCheck()
    {
        yield return new WaitForSeconds(0.5f);

        Enemy enemyLogic = enemyScript.GetComponent<Enemy>();
        level = enemyLogic.level;

        yield return null;
        switch (enemyType)
        {
            case Type.Mouse:
                maxHealth = 40;
                health = 40;
                isDie = false;
                isFollow = false;
                isAggro = false;
                isHit = false;

                break;
            case Type.Slime:
                maxHealth = 40;
                health = 40;
                isDie = false;
                isFollow = false;
                isAggro = false;
                isHit = false;

                break;
        }
        switch (level)
        {
            case Level.Easy:
                dmg = 10;
                break;

            case Level.Normal:

                break;

            case Level.Hard:
                dmg = 20;
                break;
        }
    }

    //Move
    void GroundCheck()
    {
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove, rigid.position.y);
        RaycastHit2D hit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Ground"));
        if (hit.collider == null)
        {
            rigid.velocity = new Vector2(0, rigid.velocity.y);
        }
        Debug.DrawRay(frontVec, Vector3.down, Color.red);
    }
    void MoveAnim()
    {
        if (rigid.velocity == Vector2.zero)
        {
            anim.SetBool("isMove", false);
        }
        if (rigid.velocity != Vector2.zero)
        {
            anim.SetBool("isMove", true);
        }
    }
    void Move()
    {
        if(!isAttack)
            rigid.velocity = new Vector2(frontPos * speed, rigid.velocity.y);
    }
    void Follow()
    {
        if (player.transform.position.x - transform.position.x < 0)
        {
            frontPos = -speed;
            attack.offset = new Vector2(-1.5f,0);
            spriteRenderer.flipX = false;
        }
        else if (player.transform.position.x - transform.position.x >= 0)
        {
            frontPos = speed;
            attack.offset = new Vector2(-1.5f, 0);
            spriteRenderer.flipX = true;
        }
    }
    void WatchCheck()
    {
        if (isFollow)
            return;
        if (spriteRenderer.flipX == false)
        {
            forward = new Vector2(-speed, rigid.velocity.y).normalized;
            nextMove = -1;
        }
        else if (spriteRenderer.flipX == true)
        {
            forward = new Vector2(speed, rigid.velocity.y).normalized;
            nextMove = 1;
        }
    }

    //Attack&Hit
    IEnumerator Attack()
    {
        if (isDie)
            yield break;

        int ranPattern = Random.Range(0, 2);

        switch(ranPattern)
        {
            case 0:
                isAttack = true;
                attack.enabled = true;
                anim.SetTrigger("doAttack");
                yield return new WaitForSeconds(1f);

                isAttack = false;
                attack.enabled = false;
               
                break;
            case 1:
                isAttack = true;
                double_Attack.enabled = true;
                anim.SetTrigger("doDoubleAttack");
                yield return new WaitForSeconds(1.5f);

                isAttack = false;
                double_Attack.enabled = false;
                break;
        }
    }
    IEnumerator OnHit(float dmg)
    {
        if (isHit)
            yield break;

        isHit = true;

        DamageLogic(dmg);
        ReturnSprite(0.4f);

        if (health < 0)
        {
            isDie = true;
            //anim.SetTrigger("doDead");
            yield return new WaitForSeconds(1f);
            gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(0.5f);

        isHit = false;
        ReturnSprite(1f);
    }
    void ReturnSprite(float Alpha)
    {
        spriteRenderer.color = new Color(0.4f, 0.4f, 0.4f, Alpha);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        //플레이어의 무기에 공격당했을 때 
        if (collision.gameObject.tag == "PlayerAttack")
        {
            Player playerLogic = player.GetComponent<Player>();
            StartCoroutine(OnHit(playerLogic.dmg));

            Vector2 pos = new Vector2(player.transform.position.x - transform.position.x, 0);
            //X좌표 넉백
            if (collision.gameObject.transform.position.x - transform.position.x < 0)
                rigid.AddForce(-pos * 4, ForceMode2D.Impulse);
            else if (collision.gameObject.transform.position.x - transform.position.x > 0)
                rigid.AddForce(-pos * 4, ForceMode2D.Impulse);
        }
    }
}