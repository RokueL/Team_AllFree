using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("플레이어 상태")]
    public float curLife;
    public float maxLife;
    public float curCoolDown;
    public float maxCoolDown;

    public float dmg;
    public float jumpPower;
    public float speed;
    
    public bool isStart;

    [Header("스테이터스 코어")]
    public bool stateCore;

    [Header("제물 갯수")]
    public float sacrifice;

    [Header("스킬 코어 보유 여부")]
    public bool CrouchCore;
    public bool RollCore;
    public bool DropCore;
    public bool SummonCore;

    public enum Skill_Core { Crouch, Roll, Drop, Summon }
    public Skill_Core skill_Type;

    [Header("현재 사용중인 스킬 코어")]
    public bool isCrouchCore;
    public bool isRollCore;
    public bool isDropCore;
    public bool isSummonCore;

    [Header("현재 장착중인 스킬 코어")]
    public bool equipCrouch;
    public bool equipRoll;
    public bool equipDrop;
    public bool equipSummon;

    public float equipCount;

    [Header("현재 소환수 갯수")]
    public int followCount;
    int maxFollowCount;
    int hpDecrease;

    [Header("공격 타입")]
    public bool isNormal;
    public bool isPower;
    public bool isSharp;
    public bool isMystic;

    public enum Att_Type { Normal, Power, Sharp, Mystic }
    public Att_Type att_Type;


    [Header("죽을 때 이펙트")] 
    public ParticleSystem DeadEffect;
    
    bool Q_IsSwitch;

    float roll_Speed;

    //공격 딜레이
    float curAttackDelay;
    float maxAttackDelay;

    //스킬 지속시간
    float curSkillTime;
    float maxSkillTime;

    float curStopTime;
    float maxStopTime;

    //Controller
    float Move_Axis;

    bool isGround;
    bool isJump;
    bool jumping;

    bool isAtt;
    bool speedUp;
    bool speedDown;
    bool isCrouch;

    public bool OnSkill;

    //Trigger
    bool isTouchRoom;       //투명벽 미구현

    [Header("트리거")]
    public bool isElite;
    public bool isBoss;
    public bool clearMap;       //클리어 시 트리거 작동(미구현)

    public bool isCameraMove;

    bool OnElite;
    bool OnBoss;

    bool isHit;

    bool isCheat;

    [Header("오브젝트")]
    public ObjectManager objectManager;
    public GameManager gameManager;
    public GameObject crouchParticle;
    public BoxCollider2D meleeAttack;
    public BoxCollider2D dropAttack;
    public CircleCollider2D rollAttack;

    Animator anim;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;

    void Awake()
    {

        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        //최대체력&전체체력
        maxLife = 100;
        curLife = 100;

        //쿨타임
        maxCoolDown = 0;

        //공격력&속도&구르기 속도
        dmg = 15;
        speed = 5;
        roll_Speed = speed;

        //공격속도&최대스킬시간
        maxAttackDelay = 0.5f;
        maxSkillTime = 2f;

        //소환수 정보
        maxFollowCount = 3;
        followCount = 0;
        hpDecrease = 25;

        //스킬코어 및 공격타입
        CrouchCore = true;
        skill_Type = Skill_Core.Crouch;
        equipCount = 1;
        isCrouchCore = true;
        equipCrouch = true;
        anim.SetBool("isLieDown", true);
    }
    void Update()
    {
        InPut();

        CoolDown();

        SkillOn();
        if (isCameraMove)
            Stop();
        else
            Move();

        MoveMent();
        Jump();
        StartCoroutine(Attack());

        if (isStart)
        {
            StartCoroutine(Skill());
            StartCoroutine(Summon());
            StartCoroutine(Drop());
        }
        StartCoroutine(Switching_Attack_Type());
        Core_TypeMatch();

        LifeCheck();
        StartCoroutine(LifeCheat());
        JCanvas.Instance.HPBarSet(maxLife,curLife);
    }
    //플레이어 이동
    void MoveMent()
    {
        if (!clearMap && Move_Axis == 1)
        {
            Move_Axis = 0;
        }
        if (isTouchRoom || isCrouch || isCameraMove || isElite || isBoss)
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
        if (!isStart || isCrouch || isCameraMove || isElite || isBoss)
        {
            rigid.velocity = new Vector2(0, rigid.velocity.y);
            return;
        }

        Move_Axis = Input.GetAxisRaw("Horizontal"); //이동
        isAtt = Input.GetKeyDown(KeyCode.A);          //공격
        isJump = Input.GetKeyDown(KeyCode.S);         //점프
        Q_IsSwitch = Input.GetKeyDown(KeyCode.Q);   //공격 타입 슬롯체인지 노멀->파워->정밀->신비
        isCheat = Input.GetKeyDown(KeyCode.L);      //치트키= 체력 100증가

    }
    void Input_Skill()
    {
        if (!isStart || isCameraMove || isBoss || isElite)
            return;

        if (!OnSkill)
        {
            OnSkill = Input.GetKeyDown(KeyCode.D);      //스킬
        }
        else
            curCoolDown = 0;
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
        if (isJump && isGround)
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
        if (isBoss || !isAtt)
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
        switch (skill_Type)
        {
            case Skill_Core.Roll:
                if (OnSkill)
                {
                    isHit = true;
                    Physics2D.IgnoreLayerCollision(6, 7, true);
                    rollAttack.enabled = true;
                    if (!speedUp)
                        StartCoroutine(RollingSpeedUp());
                }
                else
                {
                    isHit = false;
                    Physics2D.IgnoreLayerCollision(6, 7, false);
                    rollAttack.enabled = false;
                    if (!speedDown)
                        StartCoroutine(RollingSpeedDown());
                }

                break;
            case Skill_Core.Crouch:
                if (OnSkill)
                {
                    isHit = true;
                    isCrouch = true;
                    anim.SetBool("isLieDown", true);
                    crouchParticle.SetActive(true);
                    Physics2D.IgnoreLayerCollision(6, 7, true);
                }
                else
                {
                    isHit = false;
                    isCrouch = false;
                    anim.SetBool("isLieDown", false);
                    crouchParticle.SetActive(false);
                    Physics2D.IgnoreLayerCollision(6, 7, false);
                }
                break;
        }
        yield return null;
    }
    IEnumerator RollingSpeedUp()
    {
        if (speed == roll_Speed*1.5f)
            yield break;
        if (speed > roll_Speed * 1.5f)
            speed = roll_Speed * 1.5f;

        speedUp = true;
        yield return new WaitForSeconds(0.15f);

        speed = speed + (roll_Speed * 0.1f);
        speedUp = false;
    }
    IEnumerator RollingSpeedDown()
    {
        if (speed == roll_Speed)
            yield break;
        if (speed < roll_Speed)
            speed = roll_Speed;

        speedDown = true;
        yield return new WaitForSeconds(0.1f);

        speed = speed - (roll_Speed * 0.1f);
        speedDown = false;
    }
    IEnumerator Summon()
    {
        if(skill_Type!=Skill_Core.Summon)
            yield break;

        if (curLife <= hpDecrease || followCount > maxFollowCount)
        yield break;

        if (Input.GetKeyDown(KeyCode.D))
        {
            followCount++;
            gameManager.FollowerSpawn();
            maxLife -= hpDecrease;
        }
        yield return null;
    }
    IEnumerator Drop()
    {
        if (skill_Type != Skill_Core.Drop)
            yield break;

        if (!jumping)
            yield break;

        if (Input.GetKeyDown(KeyCode.D)&& curCoolDown == maxCoolDown)
        {
            rigid.AddForce(Vector2.down * 20, ForceMode2D.Impulse);

            yield return new WaitForSeconds(0.2f);

            dropAttack.enabled = true;
            gameManager.Roar_Effect(transform.position, 0.5f);
            yield return new WaitForSeconds(0.2f);

            dropAttack.enabled = false;
        }
        yield return null;
    }

    void SkillOn()
    {
        if (!isStart)
            return;

        if (OnSkill)
        {
            curSkillTime += Time.deltaTime;

            if (curSkillTime > maxSkillTime)
            {
                curSkillTime = 0;
                SkillOff();
            }
        }
        else
            curSkillTime = 0;
    }
    void SkillOff()
    {
        OnSkill = false;
    }
    void CoolDown()
    {
        if (!isStart)
            return;

        curCoolDown += Time.deltaTime;

        if (curCoolDown > maxCoolDown)
            curCoolDown = maxCoolDown;

        if(curCoolDown==maxCoolDown)
            Input_Skill();

        if (Input.GetKeyUp(KeyCode.D))
            OnSkill = false;
    }

    //체력 확인
    void LifeCheck()
    {
        if (curLife > maxLife)
            curLife = maxLife;
    }

    //공격타입 로직
    IEnumerator Switching_Attack_Type()
    {
        if (Q_IsSwitch)
        {
            int num;
            //3개 장비중
            if (equipCount == 3)
            {
                num = Equip_Core_3();
                switch (num)
                {
                    case 1:
                        switch (skill_Type)
                        {
                            case Skill_Core.Crouch:
                                skill_Type = Skill_Core.Roll;
                                maxCoolDown = 6;
                                break;

                            case Skill_Core.Roll:
                                skill_Type = Skill_Core.Drop;
                                maxCoolDown = 3;
                                break;

                            case Skill_Core.Drop:
                                skill_Type = Skill_Core.Crouch;
                                maxCoolDown = 0;
                                break;
                        }
                        break;
                    case 2:
                        switch (skill_Type)
                        {
                            case Skill_Core.Crouch:
                                skill_Type = Skill_Core.Roll;
                                maxCoolDown = 6;
                                break;

                            case Skill_Core.Roll:
                                skill_Type = Skill_Core.Summon;
                                maxCoolDown = 3;
                                break;

                            case Skill_Core.Summon:
                                skill_Type = Skill_Core.Crouch;
                                maxCoolDown = 0;
                                break;
                        }
                        break;
                    case 3:
                        switch (skill_Type)
                        {
                            case Skill_Core.Crouch:
                                skill_Type = Skill_Core.Drop;
                                maxCoolDown = 6;
                                break;

                            case Skill_Core.Drop:
                                skill_Type = Skill_Core.Summon;
                                maxCoolDown = 0;
                                break;

                            case Skill_Core.Summon:
                                skill_Type = Skill_Core.Crouch;
                                maxCoolDown = 0;
                                break;
                        }
                        break;
                    case 4:
                        switch (skill_Type)
                        {
                            case Skill_Core.Roll:
                                skill_Type = Skill_Core.Drop;
                                maxCoolDown = 3;
                                break;

                            case Skill_Core.Drop:
                                skill_Type = Skill_Core.Summon;
                                maxCoolDown = 0;
                                break;

                            case Skill_Core.Summon:
                                skill_Type = Skill_Core.Roll;
                                maxCoolDown = 0;
                                break;
                        }
                        break;
                }
            }
            else if (equipCount == 2)
            {
                num = Equip_Core_2();
                switch (num)
                {
                    case 1:
                        switch (skill_Type)
                        {
                            case Skill_Core.Crouch:
                                skill_Type = Skill_Core.Roll;
                                maxCoolDown = 6;
                                break;

                            case Skill_Core.Roll:
                                skill_Type = Skill_Core.Crouch;
                                maxCoolDown = 3;
                                break;
                        }
                        break;
                    case 2:
                        switch (skill_Type)
                        {
                            case Skill_Core.Crouch:
                                skill_Type = Skill_Core.Drop;
                                maxCoolDown = 6;
                                break;

                            case Skill_Core.Drop:
                                skill_Type = Skill_Core.Crouch;
                                maxCoolDown = 3;
                                break;
                        }
                        break;
                    case 3:
                        switch (skill_Type)
                        {
                            case Skill_Core.Crouch:
                                skill_Type = Skill_Core.Summon;
                                maxCoolDown = 6;
                                break;

                            case Skill_Core.Summon:
                                skill_Type = Skill_Core.Crouch;
                                maxCoolDown = 0;
                                break;
                        }
                        break;
                    case 4:
                        switch (skill_Type)
                        {
                            case Skill_Core.Roll:
                                skill_Type = Skill_Core.Drop;
                                maxCoolDown = 3;
                                break;

                            case Skill_Core.Drop:
                                skill_Type = Skill_Core.Roll;
                                maxCoolDown = 0;
                                break;
                        }
                        break;
                    case 5:
                        switch (skill_Type)
                        {
                            case Skill_Core.Roll:
                                skill_Type = Skill_Core.Summon;
                                maxCoolDown = 3;
                                break;

                            case Skill_Core.Summon:
                                skill_Type = Skill_Core.Roll;
                                maxCoolDown = 0;
                                break;
                        }
                        break;
                    case 6:
                        switch (skill_Type)
                        {
                            case Skill_Core.Drop:
                                skill_Type = Skill_Core.Summon;
                                maxCoolDown = 3;
                                break;

                            case Skill_Core.Summon:
                                skill_Type = Skill_Core.Drop;
                                maxCoolDown = 0;
                                break;
                        }
                        break;
                }
            }
            else if (equipCount == 1)
                yield break;
            yield return new WaitForSeconds(0.5f);

            Q_IsSwitch = false;
        }
    }
    void Core_TypeMatch()
    {
        if(skill_Type== Skill_Core.Crouch)
        {
            isCrouchCore = true;
            isRollCore = false;
            isDropCore = false;
            isSummonCore = false;

            att_Type = Att_Type.Normal;
            isNormal = true;
            isPower = false;
            isSharp = false;
            isMystic = false;
        }
        if (skill_Type == Skill_Core.Roll)
        {
            isCrouchCore = false;
            isRollCore = true;
            isDropCore = false;
            isSummonCore = false;

            att_Type = Att_Type.Power;
            isNormal = false;
            isPower = true;
            isSharp = false;
            isMystic = false;
        }
        if (skill_Type == Skill_Core.Drop)
        {
            isCrouchCore = false;
            isRollCore = false;
            isDropCore = true;
            isSummonCore = false;

            att_Type = Att_Type.Sharp;
            isNormal = false;
            isPower = false;
            isSharp = true;
            isMystic = false;
        }
        if (skill_Type == Skill_Core.Summon)
        {
            isCrouchCore = false;
            isRollCore = false;
            isDropCore = false;
            isSummonCore = true;

            att_Type = Att_Type.Mystic;
            isNormal = false;
            isPower = false;
            isSharp = false;
            isMystic = true;
        }
    }

    int Equip_Core_3()
    {
        if (equipCrouch && equipRoll && equipDrop)
        {
            return 1;
        }
        if (equipCrouch && equipRoll && equipSummon)
        {
            return 2;
        }
        if (equipCrouch && equipDrop && equipSummon)
        {
            return 3;
        }
        if (equipRoll && equipDrop && equipSummon)
        {
            return 4;
        }
        return 0;
    }
    int Equip_Core_2()
    {
        if (equipCrouch && equipRoll)
        {
            return 1;
        }
        if (equipCrouch && equipDrop)
        {
            return 2;
        }
        if (equipCrouch && equipSummon)
        {
            return 3;
        }
        if (equipRoll && equipDrop)
        {
            return 4;
        }
        if (equipRoll && equipSummon)
        {
            return 5;
        }
        if (equipDrop && equipSummon)
        {
            return 6;
        }
        return 0;
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
            DeadEffect.transform.position = this.transform.position; 
            DeadEffect.Play();
            JCanvas.Instance.DeadCanvas.SetActive(true);
            JCanvas.Instance.InGameHPBar.GetComponent<CanvasGroup>().DOFade(0f, 0.5f);
            JCanvas.Instance.PlayerDeadEvent();
            gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(0.3f);

        isHit = false;
        ReturnSprite(1f);
    }
    void ReturnSprite(float Alpha)
    {
        spriteRenderer.color = new Color(1, 1, 1, Alpha);
    }

    //보스 시작 트리거
    IEnumerator EliteStart()
    {
        isElite = true;
        spriteRenderer.flipX = false;
        if (!OnElite)
        {
            gameManager.CreateElite();
            gameManager.OnEliteRoom();
        }
        OnElite = true;
        yield return new WaitForSeconds(2f);

        isElite = false;
    }
    IEnumerator BossStart()
    {
        isBoss = true;
        spriteRenderer.flipX = false;
        if (!OnBoss)
            gameManager.BossRoom();
        OnBoss = true;
        yield return new WaitForSeconds(4f);

        isBoss = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
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
                    sacrifice += 1;
                    break;

                case Item.ItemType.Core:
                    switch (itemLogic.coreType)
                    {
                        case Item.CoreType.State:
                            gameManager.StateCore_Increase();
                            break;
                        case Item.CoreType.Roll:
                            RollCore = true;

                            break;
                        case Item.CoreType.Summon:
                            SummonCore = true;

                            break;
                        case Item.CoreType.Drop:
                            DropCore = true;

                            break; 
                    }
                    break;
            }
            collision.gameObject.SetActive(false);
        }
        //Ground
        if (collision.gameObject.tag == "Ground")
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
            Boss bossLogic = collision.gameObject.GetComponentInParent<Boss>();

            StartCoroutine(OnHit(bossLogic.dmg));
        }
        if (collision.gameObject.tag == "EliteMonster")
        {
            EliteEnemy eliteLogic = collision.gameObject.GetComponentInParent<EliteEnemy>();

            StartCoroutine(OnHit(eliteLogic.dmg));
        }
        if (collision.gameObject.tag == "TriggerMap")
        {
            isTouchRoom = true;
        }
        if (collision.gameObject.tag == "BossTrigger" && collision.gameObject.name == "EliteTrigger")
        {
            StartCoroutine(EliteStart());
            collision.gameObject.SetActive(false);
        }
        if (collision.gameObject.tag == "BossTrigger" && collision.gameObject.name == "BossTrigger")
        {
            StartCoroutine(BossStart());
            collision.gameObject.SetActive(false);
        }
        if (collision.gameObject.tag == "Debris")
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
