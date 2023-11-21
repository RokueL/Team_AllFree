using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotRangeEnemy : Enemy
{
    public Transform point;

    bool isDashState;

    public Enemy enemyScript;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        forward = new Vector2(speed, rigid.velocity.y).normalized;
        maxAggroTime = 4;
    }
    //체력설정

    void OnEnable()
    {
        StartCoroutine(LevelCheck());
    }
    void FixedUpdate()
    {
        GroundCheck();
        MoveAnim();
        Aggro();
        WatchCheck();
        Move();
        if (isDie)
            rigid.velocity = Vector2.zero;
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
        }
        switch (level)
        {
            case Level.Easy:
                dmg = 20;
                break;

            case Level.Normal:

                break;

            case Level.Hard:
                dmg = 25;
                break;
        }
    }

    //Move
    void GroundCheck()
    {
        RaycastHit2D hit = Physics2D.Raycast(forward, Vector3.down, 1, LayerMask.GetMask("Ground"));
        if(hit.collider==null)
        {
            rigid.velocity =Vector2.zero;
        }
    }
    void MoveAnim()
    {
        if(rigid.velocity== Vector2.zero)
            anim.SetBool("isMove", false);
        if (rigid.velocity != Vector2.zero)
            anim.SetBool("isMove", true);
    }
    void Move()
    {
        if (isAttack||isDie||isHit)
            return;
        //거리가 maxdistance이하인 경우 플레이어를 따라옴
        if (isAggro)
        {
            Follow(transform.position, mindistance, spriteRenderer);

            rigid.velocity = forward * speed;
            curAttackDelay += Time.deltaTime;
            if (curAttackDelay > maxAttackDelay && !isAttack)
            {
                StartCoroutine(Attack());
            }
        }
        //4초 이후 멈췄다 원래 자리로 돌아감
        else
        {
            curAggroTime += Time.deltaTime;

            if (curAggroTime >= maxAggroTime)
            {
                if (isAggro)
                {
                    curAttackDelay = 0;
                    curAggroTime = 0;
                    return;
                }

                if (transform.position.x - point.position.x <= 0.1 &&
                    transform.position.x - point.position.x > -0.1)
                {
                    rigid.velocity = Vector2.zero;

                    curAggroTime = 0;
                }
                else
                {
                    isFollow = false;
                    rigid.velocity = forward * speed;

                    Watch(transform.position, point.position, spriteRenderer);
                }
            }
        }
    }
    void Aggro()
    {
        dist = Vector2.Distance(player.transform.position, transform.position);
        if (dist < maxdistance)
        {
            isAggro = true;
        }
        else
        {
            isAggro = false;
            curAttackDelay = 0;
        }

    }
    void WatchCheck()
    {
        if (isFollow)
            return;
        if (spriteRenderer.flipX == false)
        { forward = new Vector2(-speed, rigid.velocity.y).normalized; }
        else if (spriteRenderer.flipX == true)
        { forward = new Vector2(speed, rigid.velocity.y).normalized; }
    }


    //Attack&Hit
    IEnumerator Attack()
    {
        if (isDie)
            yield break;

        isAttack = true;
        spriteRenderer.color = new Color(1, 0, 0, 1);
        yield return new WaitForSeconds(0.7f);

        anim.SetTrigger("doAttack");
        yield return new WaitForSeconds(0.3f);

        isDashState = true;
        rigid.velocity = forward * 5*speed+Vector2.up*4;
        yield return new WaitForSeconds(0.3f);

        isDashState = false;

        ReturnSprite(0.5f);
        yield return new WaitForSeconds(3f);

        isAttack = false;
        ReturnSprite(1f);
        curAttackDelay = 0;
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
            anim.SetTrigger("doDead");
            yield return new WaitForSeconds(1f);
            gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(0.5f);

        isHit = false;
        ReturnSprite(1f);
    }
    void DamageLogic(float dmg)
    {
        Player playerLogic = player.GetComponent<Player>();
        switch (type)
        {
            case Def_Type.Normal:
                switch (playerLogic.type)
                {
                    case Player.Att_Type.Normal:
                        health -= dmg;
                        break;
                    case Player.Att_Type.Power:
                        health = health - (dmg * 1.5f);
                        break;
                    case Player.Att_Type.Sharp:
                        health = health - (dmg * 1.5f);
                        break;
                    case Player.Att_Type.Mystic:
                        health = health - (dmg * 1.5f);
                        break;
                }
                break;
            case Def_Type.Nimble:
                switch (playerLogic.type)
                {
                    case Player.Att_Type.Normal:
                        health = health - (dmg * 0.5f);
                        break;
                    case Player.Att_Type.Power:
                        health = health - (dmg * 0.5f);
                        break;
                    case Player.Att_Type.Sharp:
                        health = health - (dmg * 1.3f);
                        break;
                    case Player.Att_Type.Mystic:
                        health = health - (dmg * 0.8f);
                        break;
                }
                break;
            case Def_Type.Resist:
                switch (playerLogic.type)
                {
                    case Player.Att_Type.Normal:
                        health = health - (dmg * 0.5f);
                        break;
                    case Player.Att_Type.Power:
                        health = health - (dmg * 1.5f);
                        break;
                    case Player.Att_Type.Sharp:
                        health = health - (dmg * 0.5f);
                        break;
                    case Player.Att_Type.Mystic:
                        health = health - (dmg * 0.5f);
                        break;
                }
                break;
            case Def_Type.Solid:
                switch (playerLogic.type)
                {
                    case Player.Att_Type.Normal:
                        health = health - (dmg * 0.5f);
                        break;
                    case Player.Att_Type.Power:
                        health = health - (dmg * 0.8f);
                        break;
                    case Player.Att_Type.Sharp:
                        health = health - (dmg * 0.8f);
                        break;
                    case Player.Att_Type.Mystic:
                        health = health - (dmg * 1.5f);
                        break;
                }
                break;
        }
    }
    void ReturnSprite(float Alpha)
    {
        spriteRenderer.color = new Color(0.4f, 0.4f, 0.4f, Alpha);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        //플레이어에게 부딪혔을때
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine( OnHit(0));

            if (isDashState)
            {
                rigid.velocity = Vector2.zero;
                //튕겨 날아가기
                if (collision.gameObject.transform.position.x - transform.position.x < 0)
                    rigid.AddForce(-forward * 5 + Vector2.up * 4, ForceMode2D.Impulse);
                else if (collision.gameObject.transform.position.x - transform.position.x > 0)
                    rigid.AddForce(-forward * 5 + Vector2.up * 4, ForceMode2D.Impulse);
            }
        }
        //벽에 부딪혔을때
        if (collision.gameObject.tag == "Border")
        {
            if (isDashState)                                 //대쉬공격 시
            {
                StartCoroutine(OnHit(0));
                {
                    rigid.velocity = Vector2.zero;
                    //튕겨 날아가기

                    if (collision.gameObject.transform.position.x - transform.position.x < 0)
                        rigid.AddForce(-forward * 6 + Vector2.up * 4, ForceMode2D.Impulse);
                    else if (collision.gameObject.transform.position.x - transform.position.x > 0)
                        rigid.AddForce(-forward * 6 + Vector2.up * 4, ForceMode2D.Impulse);
                }
            }
            else
                rigid.velocity = Vector2.zero;
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        //플레이어의 무기에 공격당했을 때 
        if (collision.gameObject.tag == "PlayerAttack")
        {
            Player playerLogic = player.GetComponent<Player>();
            StartCoroutine(OnHit(playerLogic.dmg));

            Vector2 pos = new Vector2(player.transform.position.x - transform.position.x,0);
            //X좌표 넉백
            if (collision.gameObject.transform.position.x - transform.position.x < 0)
                rigid.AddForce(-pos * 4, ForceMode2D.Impulse);
            else if (collision.gameObject.transform.position.x - transform.position.x > 0)
                rigid.AddForce(-pos * 4 , ForceMode2D.Impulse);
        }
    }
}