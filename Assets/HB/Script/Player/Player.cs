using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //소환수
    public int followCount;
    int maxFollowCount;
    int hpDecrease;

    //공격 타입
    public enum Att_Type { Normal,Power,Sharp,Mystic}
    public Att_Type type;

    //특수 코어
    public enum Skill_Core {Roll, Summon,DropAttack,Crouch }
    public Skill_Core skill_Type;

    //플레이어 스테이터스
    public float curLife;
    public float maxLife;
    public float dmg;
    public float jumpPower;
    public float speed;
    float roll_Speed;

    public float coin;

    //공격 딜레이
    float curAttackDelay;
    float maxAttackDelay;

    public float curSkillDelay;
    public float maxSkillDelay;

    public bool clearMap;       //클리어 시 트리거 작동(미구현)
    public bool isStart;        //시작 전 컨트롤제어

    //스테이터스 코어(미구현)
    public bool damageCore;
    public bool speedCore;
    public bool healthCore;

    //공격타입 트리거
    public bool isNormal;
    public bool isPower;
    public bool isSharp;
    public bool isMystic;

    bool Q_IsSwitch;
    bool E_IsSwitch;

    //Controller
    float Move_Axis;

    bool isGround;
    bool isJump;
    bool jumping;

    bool isAtt;
    bool isCrouch;

    bool onSkill;
    bool offSkill;

    public bool startSkill;
    public bool endSkill;

    //Trigger
    bool isTouchRoom;       //투명벽 미구현
    public bool isElite;
    public bool isBoss;

    bool isHit;

    bool isCheat;
    public bool isCameraMove;

    public ObjectManager objectManager;
    public GameManager gameManager;
    public BoxCollider2D meleeAttack;
    public CircleCollider2D rollAttack;

    Animator anim;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;
    PlatformEffector2D effector;

    void Awake()
    {
        maxLife = 100;
        curLife = 100;
        dmg = 15;
        speed = 5;
        roll_Speed = speed;

        maxAttackDelay = 0.5f;
        maxSkillDelay = 2f;

        maxFollowCount = 3;
        followCount = 0;
        hpDecrease = 25;

        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        effector = GetComponent<PlatformEffector2D>();

        isNormal = true;
    }
    void Update()
    {
        InPut();
        Input_Skill();
        SkillOn();
        if (isCameraMove)
            Stop();
        else
            Move();

        MoveMent();
        Jump();
        LifeCheck();
        StartCoroutine(Attack());
        StartCoroutine(Skill());

        StartCoroutine(Attack_Type());

        StartCoroutine(LifeCheat());
    }
    //플레이어 이동
    void MoveMent()
    {
        if (!clearMap&& Move_Axis == 1)
        {
            Move_Axis = 0;
        }
        if (isTouchRoom||isCrouch|| isCameraMove)
            Move_Axis = 0;

        rigid.velocity = new Vector2(Move_Axis * speed, rigid.velocity.y);  
        if (Move_Axis != 0)
        {
            if (Move_Axis > 0)
            {
                spriteRenderer.flipX = false;
                meleeAttack.offset = new Vector2(0.6f, 0);
            }
            else if (Move_Axis < 0)
            {
                spriteRenderer.flipX = true;
                meleeAttack.offset = new Vector2(-0.6f, 0);
            }
        }
    }
    void Stop()
    {
        rigid.velocity = Vector2.zero;
        rigid.gravityScale = 0;
    }
    void Move()
    {
        rigid.gravityScale = 3;
    }

    //시작 전 동작 불가
    void InPut()
    {
        if (!isStart||isCrouch|| isCameraMove)
            return;

        Move_Axis = Input.GetAxisRaw("Horizontal"); //이동
        isAtt=Input.GetKeyDown(KeyCode.A);          //공격
        isJump=Input.GetKeyDown(KeyCode.S);         //점프
        Q_IsSwitch = Input.GetKeyDown(KeyCode.Q);   //공격 타입 슬롯체인지 노멀->파워->정밀->신비
        E_IsSwitch = Input.GetKeyDown(KeyCode.E);   //공격 타입 슬롯체인지 노멀->신비->정밀->파워
        isCheat = Input.GetKeyDown(KeyCode.L);      //치트키= 체력 100증가

    }
    void Input_Skill()
    {
        if (!isStart || isCameraMove)
            return;

        startSkill = Input.GetKeyDown(KeyCode.D);      //스킬
        endSkill = Input.GetKeyUp(KeyCode.D);
    }
    void Jump()
    {
        if (rigid.velocity.y == 0)
        {
            isGround = true;
            jumping = false;
        }
        if (isJump && jumping)
        {
            anim.SetTrigger("doJump");
            rigid.velocity = new Vector2(rigid.velocity.x, 0f);
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            jumping = false;
        }
        if (isJump&&isGround)
        {
            anim.SetTrigger("doJump");
            rigid.velocity = new Vector2(rigid.velocity.x, 0f);
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            isGround = false;
            jumping = true;
        }
    }
    
    IEnumerator Attack()
    {
        curAttackDelay += Time.deltaTime;
        if (curAttackDelay < maxAttackDelay)
            yield break;
        if(isBoss || !isAtt)
            yield break;

        meleeAttack.enabled = true;
        anim.SetTrigger("doAttack");
        yield return new WaitForSeconds(0.1f);

        meleeAttack.enabled = false;
        isAtt = false;
        curAttackDelay = 0;
    }

    //스킬
    IEnumerator Skill()
    {
        if (startSkill|| endSkill)
        {
            switch (skill_Type)
            {
                case Skill_Core.Roll:
                    if (!endSkill)
                    {
                        isHit = true;
                        onSkill = true;
                        offSkill = false;
                        Physics2D.IgnoreLayerCollision(6, 7,true);
                        rollAttack.enabled = true;

                        for (int index = 0; index < 10;)
                        {
                            yield return new WaitForSeconds(0.15f);
                            speed = speed+(roll_Speed * 0.1f);
                            index++;
                        }

                        endSkill = false;
                        startSkill = false;
                    }
                    else
                    {
                        if (offSkill)
                            yield break;

                        isHit = false;
                        onSkill = false;
                        Physics2D.IgnoreLayerCollision(6, 7, false);
                        rollAttack.enabled = false;

                        for (int index = 0; index < 10;)
                        {
                            yield return new WaitForSeconds(0.1f);
                            speed = speed - (roll_Speed * 0.1f);
                            index++;
                        }
                        yield return null;
                        endSkill = false;
                        startSkill = false;
                        offSkill = true;
                    }

                    break;

                case Skill_Core.Summon:
                    
                    if (curLife <= hpDecrease || followCount > maxFollowCount||endSkill)
                        yield break;

                    followCount++;
                    gameManager.FollowerSpawn();
                    maxLife -= hpDecrease;
                    startSkill = false;
                    yield return null;
                    break;

                case Skill_Core.DropAttack:

                    break;
                case Skill_Core.Crouch:
                    if (!endSkill)
                    {
                        isHit = true;
                        onSkill = true;
                        isCrouch = true;
                        offSkill = false;
                        anim.SetBool("isLieDown", true);
                        Physics2D.IgnoreLayerCollision(6, 7, true);

                        endSkill = false;
                        startSkill = false;
                    }
                    else
                    {
                        if (offSkill)
                            yield break;

                        isHit = false;
                        onSkill = false;
                        isCrouch = false;
                        anim.SetBool("isLieDown", false);
                        Physics2D.IgnoreLayerCollision(6, 7, false);

                        yield return null;
                        endSkill = false;
                        startSkill = false;
                        offSkill = true;
                    }
                 break;
            }
        }

        yield return null;
    }
    void SkillOn()
    {
        if (onSkill)
        {
            curSkillDelay += Time.deltaTime;

            if (curSkillDelay > maxSkillDelay)
            {
                SkillOff();
                curSkillDelay = 0;
            }
        }
        else
            curSkillDelay = 0;
    }
    void SkillOff()
    {
        endSkill = true;
    }
    //체력 확인
    void LifeCheck()
    {
        if (curLife > maxLife)
            curLife = maxLife;
    }

    //공격타입 로직
    IEnumerator Attack_Type()
    {
        if (Q_IsSwitch)
        {
            switch (type)
            {
                case Att_Type.Normal:
                    isNormal = false;
                    isPower = true;
                    type = Att_Type.Power;
                    break;
                case Att_Type.Power:
                    isPower = false;
                    isSharp = true;
                    type = Att_Type.Sharp;
                    break;
                case Att_Type.Sharp:
                    isSharp = false;
                    isMystic = true;
                    type = Att_Type.Mystic;
                    break;
                case Att_Type.Mystic:
                    isMystic = false;
                    isNormal = true;
                    type = Att_Type.Normal;
                    break;
            }
            yield return new WaitForSeconds(0.5f);

            Q_IsSwitch = false;
        }
        if(E_IsSwitch)
        {
            switch (type)
            {
                case Att_Type.Normal:
                    isNormal = false;
                    isMystic = true;
                    type = Att_Type.Mystic;
                    break;
                case Att_Type.Power:
                    isPower = false;
                    isNormal = true;
                    type = Att_Type.Normal;
                    break;
                case Att_Type.Sharp:
                    isSharp = false;
                    isPower = true;
                    type = Att_Type.Power;
                    break;
                case Att_Type.Mystic:
                    isMystic = false;
                    isSharp = true;
                    type = Att_Type.Sharp;
                    break;
            }
            yield return new WaitForSeconds(0.5f);

            E_IsSwitch = false;
        }
        yield return null;
    }

    //치트     Life+100
    IEnumerator LifeCheat()
    {
        if (!isCheat)
            yield break;

        yield return null;
        curLife += 100;
        maxLife += 100;

        yield return new WaitForSeconds(0.5f);
        isCheat = false;
    }

    //피격
    IEnumerator OnHit(float dmg)
    {
        if (isHit)
            yield break;

        isHit = true;
        curLife -= dmg;
        ReturnSprite(0.8f);
        anim.SetTrigger("doHit");
        rigid.velocity = Vector2.zero;
        rigid.AddForce(Vector2.up * 4, ForceMode2D.Impulse);

        if (curLife < 0)
        {
            gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(0.3f);

        isHit = false;
        ReturnSprite(1f);
    }
    void ReturnSprite(float Alpha)
    {
        spriteRenderer.color = new Color(0.4f, 0.4f, 0.4f, Alpha);
    }
    //보스 시작 트리거
    IEnumerator EliteStart()
    {
        isElite = true;
        yield return new WaitForSeconds(0.2f);

        isElite = false;
    }
    IEnumerator BossStart()
    {
        isElite = true;
        yield return new WaitForSeconds(0.2f);

        isElite = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag=="Enemy")
        {
            Enemy enemyLogic = collision.gameObject.GetComponentInParent<Enemy>();

            StartCoroutine(OnHit(enemyLogic.dmg));
        }

        //Item
        if (collision.gameObject.tag == "Item")
        {
            Item itemLogic = collision.gameObject.GetComponent<Item>();
            switch (itemLogic.itemType)
            {
                case Item.ItemType.Coin:
                    coin += itemLogic.coin;
                    break;

                case Item.ItemType.Core:
                    switch (itemLogic.coreType)
                    {
                        case Item.CoreType.Damage:
                            damageCore = true;
                            break;
                        case Item.CoreType.Speed:
                            speedCore = true;
                            break;
                        case Item.CoreType.Health:
                            healthCore = true;
                            break;
                    }
                    break;
            }
            collision.gameObject.SetActive(false);
        }
        //Ground
        if(collision.gameObject.tag=="Ground")
        {
            rigid.velocity = new Vector2(rigid.velocity.x, 0);
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "EnemyAttack")
        {
            EnemyObject enemyLogic = collision.gameObject.GetComponent<EnemyObject>();

            StartCoroutine(OnHit(enemyLogic.dmg));
        }
        if (collision.gameObject.tag == "BossAttack")
        {
            BossAttackObject objectLogic = collision.gameObject.GetComponent<BossAttackObject>();
            switch (objectLogic.Att_type)
            {
                case BossAttackObject.Attack_Type.Melee:
                    StartCoroutine(OnHit(objectLogic.dmg));
                    break;
                case BossAttackObject.Attack_Type.Range:
                    StartCoroutine(OnHit(objectLogic.dmg));
                    break;
            }
        }
        if (collision.gameObject.tag == "Boss")
        {
            Boss bossLogic =collision.gameObject.GetComponentInParent<Boss>();

            StartCoroutine(OnHit(bossLogic.dmg));
        }
        if (collision.gameObject.tag == "EliteMonster")
        {
            EliteEnemy eliteLogic = collision.gameObject.GetComponentInParent<EliteEnemy>();

            StartCoroutine(OnHit(eliteLogic.dmg));
        }
        if (collision.gameObject.tag=="TriggerMap")
        {
            isTouchRoom = true;
        }
        if(collision.gameObject.tag=="BossTrigger"&&collision.gameObject.name=="EliteTrigger")
        {
            StartCoroutine(EliteStart());
            collision.gameObject.SetActive(false);
        }
        if (collision.gameObject.tag == "BossTrigger" && collision.gameObject.name == "BossTrigger")
        {
            StartCoroutine(BossStart());
            collision.gameObject.SetActive(false);
        }
        if (collision.gameObject.tag=="Debris")
        {
            Debris debrisLogic = collision.gameObject.GetComponent<Debris>();
            collision.gameObject.SetActive(false);

            StartCoroutine(OnHit(debrisLogic.dmg));
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "TriggerMap")
        {
            isTouchRoom = false;
        }
    }
}
