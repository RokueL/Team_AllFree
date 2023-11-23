using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteEnemy : Enemy
{
    public Transform[] point;
    public Enemy enemyScript;

    float frontPos;
    float deathHitCount;

    float curSkillDelay;
    float maxSkillDelay;

    bool isStart;
    bool isSkill;

    bool StateCore;
    bool SkillCore;

    public BoxCollider2D attack;
    public BoxCollider2D double_Attack;
    public BoxCollider2D dig;
    public GameObject Trigger;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        speed = 1.5f;
        maxSkillDelay = 10f;

        forward = new Vector2(speed, rigid.velocity.y).normalized;
        maxAttackDelay = Random.Range(1f, 1.5f);
    }
    //체력설정

    void OnEnable()
    {
        StartCoroutine(LevelCheck());
        StartCoroutine(EliteStart());
    }
    void FixedUpdate()
    {
        dist = Vector2.Distance(player.transform.position, transform.position);
        MoveAnim();
        WatchCheck();
        Follow();
        Move();
        Think();
        //GroundCheck();
        if (isDie || isAttack || !isStart||isSkill)
            rigid.velocity = new Vector2(0,rigid.velocity.y);

        if (curSkillDelay > maxSkillDelay)
        {
            StartCoroutine (Skill());
            curSkillDelay = 0;
        }
    }

    //Start Setting
    IEnumerator LevelCheck()
    {
        yield return new WaitForSeconds(0.5f);

        Enemy enemyLogic = enemyScript.GetComponent<Enemy>();
        level = enemyLogic.level;

        yield return null;
        switch (level)
        {
            case Level.Easy:
                dmg = 12;
                maxHealth = 100;
                health = 100;
                isDie = false;
                isFollow = false;
                isAggro = false;
                isHit = false;
                break;

            case Level.Normal:

                break;

            case Level.Hard:
                dmg = 18;
                maxHealth = 100;
                health = 100;
                isDie = false;
                isFollow = false;
                isAggro = false;
                isHit = false;
                break;
        }
    }

    IEnumerator EliteStart()
    {
        Physics2D.IgnoreLayerCollision(8, 9, true);
        yield return new WaitForSeconds(0.8f);

        gameManager.ShakeCam(1f,0.4f);
        yield return new WaitForSeconds(0.5f);

        Physics2D.IgnoreLayerCollision(8, 9, false);
        anim.SetTrigger("doDoubleAttack");
        yield return new WaitForSeconds(1f);
        isStart = true;

    }
    void MoveAnim()
    {
        if (isDie)
            return;
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
        if (!isAttack)
            rigid.velocity = new Vector2(frontPos * speed, rigid.velocity.y);
    }
    void Follow()
    {
        if (player.transform.position.x - transform.position.x < -1.5f)
        {
            frontPos = -speed;
            spriteRenderer.flipX = false;
        }
        else if (player.transform.position.x - transform.position.x >= 1.5f)
        {
            frontPos = speed;
            spriteRenderer.flipX = true;
        }
        else if (player.transform.position.x - transform.position.x >= -1.5f && player.transform.position.x - transform.position.x <= 0)
        {
            frontPos = speed;
            spriteRenderer.flipX = false;
        }
        else if (player.transform.position.x - transform.position.x < 1.5f && player.transform.position.x - transform.position.x > 0)
        {
            frontPos = -speed;
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

            attack.offset = new Vector2(-1.5f, 0);
            double_Attack.offset = new Vector2(-1.5f, 0);
        }
        else if (spriteRenderer.flipX == true)
        {
            forward = new Vector2(speed, rigid.velocity.y).normalized;
            nextMove = 1;

            attack.offset = new Vector2(1.5f, 0);
            double_Attack.offset = new Vector2(1.5f, 0);
        }
    }

    //Attack&Hit
    void Think()
    {
        if (dist < 2)
        {
            curAttackDelay += Time.deltaTime;
            if (isDie)
                rigid.velocity = Vector2.zero;
            if (curAttackDelay > maxAttackDelay)
            {
                StartCoroutine(Attack());
                curAttackDelay = 0;
            }
        }
        else
            curSkillDelay += Time.deltaTime;
    }
    IEnumerator Attack()
    {
        if (isDie)
            yield break;

        int ranPattern = Random.Range(0, 2);
        isAttack = true;
        switch (ranPattern)
        {
            case 0:
                isAttack = true;

                yield return new WaitForSeconds(0.5f);

                attack.enabled = true;
                anim.SetTrigger("doAttack");
                yield return new WaitForSeconds(0.5f);

                isAttack = false;
                attack.enabled = false;

                yield return new WaitForSeconds(0.5f);
                break;
            case 1:
                isAttack = true;

                yield return new WaitForSeconds(0.5f);

                double_Attack.enabled = true;
                anim.SetTrigger("doDoubleAttack");
                yield return new WaitForSeconds(0.5f);

                isAttack = false;
                double_Attack.enabled = false;

                yield return new WaitForSeconds(0.5f);
                break;
        }
    }
    IEnumerator OnHit(float dmg)
    {
        if (isHit)
            yield break;

        isHit = true;

        DamageLogic(dmg);
        ReturnSprite(0.5f);

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
            Vector2 ranVec = new Vector2(Random.Range(-1f, 1f), 0);
            int num = 3;
            if (StateCore||SkillCore)
                num = 2;
            if (StateCore && SkillCore)
                num = 1;

            int ranItem = Random.Range(0, num);
            string itemType=null;
            if ((deathHitCount == 3 || deathHitCount == 4) && !SkillCore)
                ranItem = 1;
            if ((deathHitCount == 3 || deathHitCount == 4) && !StateCore)
                ranItem = 2;

            switch (ranItem)
            {
                case 0:
                    itemType = "gold";
                    break;
                case 1:
                    int ranSkillCore = Random.Range(0, 3);
                    SkillCore = true;
                    switch (ranSkillCore)
                    {
                        case 0:
                            itemType = "coreA";
                            break;
                        case 1:
                            itemType = "coreB";
                            break;
                        case 2:
                            itemType = "coreC";
                            break;
                    }
                    break;
                case 2:
                    int ranstateCore = Random.Range(0, 3);
                    StateCore = true;
                    switch (ranstateCore)
                    {
                        case 0:
                            itemType = "damage_Core";
                            break;
                        case 1:
                            itemType = "speed_Core";
                            break;
                        case 2:
                            itemType = "health_Core";
                            break;
                    }
                    break;
            }
            if (deathHitCount <= 4)
            {
                GameObject Item = objectManager.MakeObj(itemType);
                Item.transform.position = transform.position;

                Rigidbody2D Item_Rigid = Item.GetComponent<Rigidbody2D>();
                Item_Rigid.AddForce(ranVec * 2 + Vector2.up * 8, ForceMode2D.Impulse);
            }
            if (deathHitCount == 4)
            {
                gameObject.SetActive(false);
                gameManager.CreateBoss();
                gameManager.OffEliteRoom();
                yield break;
            }
            else
                yield break;
        }
        yield return new WaitForSeconds(0.1f);

        isHit = false;
        ReturnSprite(1f);
    }
    IEnumerator Skill()
    {
        isSkill = true;
        yield return null;

        rigid.gravityScale = 10;
        rigid.AddForce(Vector2.up * 20,ForceMode2D.Impulse);
        Physics2D.IgnoreLayerCollision(3, 8, true);
        spriteRenderer.sortingOrder = 0;

        yield return new WaitForSeconds(0.4f);
        spriteRenderer.color = new Color(1, 1, 1, 0);

        yield return new WaitForSeconds(1f);

        int ranPoint = Random.Range(0, point.Length);
        transform.position = point[ranPoint].position;

        dig.enabled = true;
        rigid.velocity = Vector2.zero;
        rigid.AddForce(Vector2.up * 40, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.1f);

        spriteRenderer.color = new Color(1, 1, 1, 1);
        yield return new WaitForSeconds(0.4f);

        Physics2D.IgnoreLayerCollision(3, 8, false);
        rigid.gravityScale = 3;
        dig.enabled = false;
        isSkill = false;
    }

    void ReturnSprite(float Alpha)
    {
        spriteRenderer.color = new Color(1f, 1f, 1f, Alpha);
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