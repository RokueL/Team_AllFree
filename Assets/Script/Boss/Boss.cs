using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    public enum Boss_Type { BossA, BossB, BossC }
    public Boss_Type bossType;

    public int patternIndex;
    public int deathHitCount;
    public int attackDamage;

    float rageGage;

    float frontPos;

    //BossA
    public float rollingSpeed;
    public float PatternDelay;
    public float curShotDelay;
    public float maxShotDelay;

    float normalScatchRatio;

    float rageScatchRatio;
    float rageRollingRatio;

    float doReadyRoll;

    bool isRolling;

    bool isRolling_1;
    bool isRolling_2;

    int scaleCount;
    bool isScalesFire;

    public bool isAppear;
    bool rageState;

    public CircleCollider2D Rolling;


    public Enemy enemyScript;

    void Awake()
    {
        normalScatchRatio = 7;
        rageScatchRatio = 4;
        rageRollingRatio = 7;
        doReadyRoll = 5;

        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    void OnEnable()
    {
        StartCoroutine(LevelCheck());
    }
    void FixedUpdate()
    {
        Player playerLogic = player.GetComponent<Player>();
        if (playerLogic.isBoss&&isAppear)
            Appear();

        if (!isDie)
        {
            dist = Vector2.Distance(player.transform.position, transform.position);

            Follow();
            //BossA
            {
                //Moving
                if (!isAttack && !isAppear)
                {
                    Move();
                    anim.SetBool("isWalk", true);
                }
                //ScaleShot
                else if (isScalesFire)
                {
                    curShotDelay += Time.deltaTime;
                    ScalesAttack();
                }

                if (rageState)
                    RageSprite();

            }
        }
        else
        {
            DieSprite();
        }


    }

    //Start Setting
    public void Appear()
    {
        isAppear = false;
        StartCoroutine(Think());
    }
    IEnumerator LevelCheck()
    {
        yield return new WaitForSeconds(0.5f);

        Enemy enemyLogic = enemyScript.GetComponent<Enemy>();
        level = enemyLogic.level;

        switch (bossType)
        {
            case Boss_Type.BossA:
                isAppear = true;
                break;
            case Boss_Type.BossB:

                break;
            case Boss_Type.BossC:

                break;
        }
        switch (level)
        {
            case Level.Easy:
                health = 100;
                maxHealth = 100;
                dmg = 15;
                rageGage = -1;
                scaleCount = 5;
                break;

            case Level.Normal:

                break;

            case Level.Hard:
                health = 100;
                maxHealth = 100;
                dmg = 20;
                rageGage = 30;
                scaleCount = 9;
                break;

        }
    }

    //Move
    void Move()
    {
        rigid.velocity = new Vector2(frontPos * speed, rigid.velocity.y);
    }
    void Follow()
    {
        if (player.transform.position.x - transform.position.x < 0)
        {
            frontPos = -speed;
            spriteRenderer.flipX = false;
        }
        else if (player.transform.position.x - transform.position.x >= 0)
        {
            frontPos = speed;
            spriteRenderer.flipX = true;
        }
    }
    void RandomPattern()
    {
        int ranPattern = Random.Range(0, 10);
        if (!rageState)
        {
            if (ranPattern < normalScatchRatio) //70%
                patternIndex = 0;
            else                                //30%
                patternIndex = 1;
        }
        else
        {
            if (ranPattern < rageScatchRatio)         //40%
                patternIndex = 0;
            else if(ranPattern< rageRollingRatio)       //30%
                patternIndex = 1;
            else                        //30%
                patternIndex = 2;
        }
    }
    IEnumerator Think()
    {
        if(isDie)
            yield break;

        isAppear = false;
        isAttack = false;

        rigid.velocity = Vector2.zero;

        if (!rageState)
            ReturnSprite(1f);
        //Rage Mode
        if (health <= maxHealth * (rageGage / 100f) && !rageState)
        {
            isAttack = true;
            rageState = true;
            yield return new WaitForSeconds(3);

            Rage();
            StartCoroutine(Think());
            yield break;
        }
        RandomPattern();
        switch(patternIndex)
        {
            case 0:     //Scratch&ReThink
                if(dist<5)
                {
                    yield return new WaitForSeconds(PatternDelay);
                    StartCoroutine(ScratchAttack());
                }
                else
                {
                    yield return new WaitForSeconds(PatternDelay);
                    StartCoroutine(Think());
                }
                break;
            case 1:     //Roll
                StartCoroutine(RollAttack());
                break;
            case 2:     //Jump
                StartCoroutine(JumpAttack());
                break;
            default:
                StartCoroutine(Think());
                break;
        }
    }

    //Rolling Attack
    IEnumerator RollAttack()
    {
        float rollDmg = dmg;
        yield return null;

        dmg = rollDmg * 1.2f;
        isRolling = true;
        isAttack = true;
        anim.SetTrigger("StartRoll");
        rigid.AddForce(Vector2.right * frontPos * -doReadyRoll + Vector2.up * doReadyRoll, ForceMode2D.Impulse);

        int ranRoll = Random.Range(0, 2);
        yield return new WaitForSeconds(PatternDelay);

        Rolling.enabled = true;
        if (ranRoll == 0)
            isRolling_1 = true;

        if (ranRoll == 1)
            isRolling_2 = true;

        rigid.AddForce(Vector2.right * frontPos * rollingSpeed, ForceMode2D.Impulse);
        yield return new WaitForSeconds(PatternDelay);

        anim.SetTrigger("EndRoll");
        dmg = rollDmg;
        Stun();
        Rolling.enabled = false;
        yield return new WaitForSeconds(PatternDelay * 2);

        StartCoroutine(Think());
    }

    //Scratch Attack
    IEnumerator ScratchAttack()
    {
        isAttack = true;
        anim.SetBool("isWalk", false);
        yield return null;

        GameObject scratch = objectManager.MakeObj("bossAAttack_0");
        BossAttackObject scratchLogic = scratch.GetComponent<BossAttackObject>();
        scratchLogic.enemyScript = enemyScript;
        scratchLogic.dmg = dmg;

        scratch.transform.position = transform.position + Vector3.right * frontPos*2+Vector3.down*0.2f;

        yield return new WaitForSeconds(PatternDelay);
        StartCoroutine(Think());
    }

    //Jump Attack
    IEnumerator JumpAttack()
    {
        isAttack = true;
        yield return new WaitForSeconds(1f);

        rigid.AddForce(Vector2.up * 15, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.3f);

        rigid.AddForce(Vector2.down * 20, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f);

        gameManager.DropDebris();

        StartCoroutine(Think());
    }

    //Scales Attack
    void ScalesAttack()
    {
        if (curShotDelay < maxShotDelay)
            return;

        int ran = Random.Range(0, 10);

        if (ran < 3)   //30%
        {
            for (float index = 0; index < scaleCount; index++)
            {
                GameObject scales = objectManager.MakeObj("bossAAttack_1");
                BossAttackObject scaleLogic = scales.GetComponent<BossAttackObject>();
                scaleLogic.enemyScript = enemyScript;
                scaleLogic.dmg = dmg * 0.7f;

                scales.transform.position = transform.position+Vector3.down*1f;
                Rigidbody2D S_rigid = scales.GetComponent<Rigidbody2D>();
                Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * index / (scaleCount-1)),
                    Mathf.Sin(Mathf.PI * index / (scaleCount - 1)));

                S_rigid.transform.Rotate(Vector3.forward * index * 45);
                S_rigid.AddForce(dirVec * 5, ForceMode2D.Impulse);
            }
        }
        curShotDelay = 0;
    }

    //Speed*1.25
    void Rage()
    {
        isScalesFire = true;
        isAttack = false;
        rageState = true;
        PatternDelay = 1.2f;
        speed *= 1.25f ;
    }

    //Rolling Stun
    void Stun()
    {
        isRolling = false;

        isRolling_1 = false;
        isRolling_2 = false;

        rigid.velocity = Vector2.zero;
        transform.rotation = Quaternion.identity;

        ReturnSprite(0.7f);
    }

    //Hit
    IEnumerator OnHit(float dmg)
    {
        if (isHit)
            yield break;

        if (health > 0)
        {
            isHit = true;
            DamageLogic(dmg);

            ReturnSprite(0.4f);

            //Rolling Animation
            if (!isRolling)
            {
                anim.SetTrigger("doHit");
            }
            yield return new WaitForSeconds(0.2f);
            isHit = false;
            ReturnSprite(1f);
        }
        if (health <= 0)
        {
            //Dead Animation
            if (!isDie)
            {
                anim.SetTrigger("doDie");
                isDie = true;
                yield return new WaitForSeconds(2.2f);
            }

            ReturnSprite(0.3f);
            deathHitCount++;
            
            yield return new WaitForSeconds(0.2f);

            isHit = false;
            //Item
            for (int index = 0; index < 5; index++)
            {
                Vector2 ranVec = new Vector2(Random.Range(-1f, 1f), 0);
                if (deathHitCount < 2)
                {
                    GameObject b_Coin = objectManager.MakeObj("bronze");
                    b_Coin.transform.position = transform.position;

                    Rigidbody2D b_Rigid = b_Coin.GetComponent<Rigidbody2D>();
                    b_Rigid.AddForce(ranVec * 2 + Vector2.up * 8, ForceMode2D.Impulse);
                }
                else if (deathHitCount < 3)
                {
                    GameObject s_Coin = objectManager.MakeObj("silver");
                    s_Coin.transform.position = transform.position;

                    Rigidbody2D s_Rigid = s_Coin.GetComponent<Rigidbody2D>();
                    s_Rigid.AddForce(ranVec * 2 + Vector2.up * 8, ForceMode2D.Impulse);
                }
                else if (deathHitCount < 4)
                {
                    GameObject g_Coin = objectManager.MakeObj("gold");
                    g_Coin.transform.position = transform.position;

                    Rigidbody2D g_Rigid = g_Coin.GetComponent<Rigidbody2D>();
                    g_Rigid.AddForce(ranVec * 2 + Vector2.up * 8, ForceMode2D.Impulse);
                }
                else if(deathHitCount==4)
                {
                    GameObject coreA = objectManager.MakeObj("coreA");
                    coreA.transform.position = transform.position;

                    Rigidbody2D rigid = coreA.GetComponent<Rigidbody2D>();
                    rigid.AddForce(Vector2.up * 8, ForceMode2D.Impulse);
                    anim.SetTrigger("doDie");
                    yield return new WaitForSeconds(2.2f);

                    gameObject.SetActive(false);
                    yield break;
                }
                else
                    yield break;
            }
        }
    }
    void ReturnSprite(float Alpha)
    {
        spriteRenderer.color = new Color(0.4f, 0.4f, 0.4f, Alpha);
    }

    //Sprite
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
    void RageSprite()
    {
        spriteRenderer.color = new Color(1, 0.5f, 0.5f, 1);
    }
    void DieSprite()
    {
        ReturnSprite(0.5f);
    }

    //Tigger
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDie)
            return;
        if(collision.gameObject.tag=="Border")
        {
            if (isRolling_1)
            {
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                gameManager.SpawnMouse();
            }
            if (isRolling_2)
            {
                rigid.velocity = Vector2.zero;
                rigid.AddForce(Vector2.right * frontPos * rollingSpeed, ForceMode2D.Impulse);
                gameManager.SpawnMouse();
            }
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        //플레이어의 무기에 공격당했을 때 
        if (collision.gameObject.tag == "PlayerAttack")
        {
            Player playerLogic = player.GetComponent<Player>();
            StartCoroutine(OnHit(playerLogic.dmg));

        }
    }
}