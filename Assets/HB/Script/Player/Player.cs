using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //��ȯ��
    public int followCount;
    int maxFollowCount;
    int hpDecrease;

    //���� Ÿ��
    public enum Att_Type { Normal, Power, Sharp, Mystic }
    public Att_Type type;

    //Ư�� �ھ�
    public enum Skill_Core { Roll, Summon, DropAttack, Crouch }
    public Skill_Core skill_Type;

    //�÷��̾� �������ͽ�
    public float curLife;
    public float maxLife;
    public float dmg;
    public float jumpPower;
    public float speed;
    float roll_Speed;

    public float coin;

    //���� ������
    float curAttackDelay;
    float maxAttackDelay;

    public float curSkillDelay;
    public float maxSkillDelay;

    public bool clearMap;       //Ŭ���� �� Ʈ���� �۵�(�̱���)
    public bool isStart;        //���� �� ��Ʈ������

    //�������ͽ� �ھ�(�̱���)
    public bool damageCore;
    public bool speedCore;
    public bool healthCore;

    //����Ÿ�� Ʈ����
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
    bool speedUp;
    bool speedDown;
    bool isCrouch;

    public bool OnSkill;

    //Trigger
    bool isTouchRoom;       //���� �̱���
    public bool isElite;
    public bool isBoss;

    bool OnElite;
    bool OnBoss;

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

    void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

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

        anim.SetBool("isLieDown", true);
        isNormal = true;
    }
    void Update()
    {
        InPut();
        Input_Skill();
        CoolDown();
        if (isCameraMove)
            Stop();
        else
            Move();

        MoveMent();
        Jump();
        LifeCheck();
        StartCoroutine(Attack());
        StartCoroutine(Skill());
        StartCoroutine(Summon());

        StartCoroutine(Attack_Type());

        StartCoroutine(LifeCheat());
    }
    //�÷��̾� �̵�
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

    //���� �� ���� �Ұ�
    void InPut()
    {
        if (!isStart || isCrouch || isCameraMove || isElite || isBoss)
        {
            rigid.velocity = new Vector2(0, rigid.velocity.y);
            return;
        }

        Move_Axis = Input.GetAxisRaw("Horizontal"); //�̵�
        isAtt = Input.GetKeyDown(KeyCode.A);          //����
        isJump = Input.GetKeyDown(KeyCode.S);         //����
        Q_IsSwitch = Input.GetKeyDown(KeyCode.Q);   //���� Ÿ�� ����ü���� ���->�Ŀ�->����->�ź�
        E_IsSwitch = Input.GetKeyDown(KeyCode.E);   //���� Ÿ�� ����ü���� ���->�ź�->����->�Ŀ�
        isCheat = Input.GetKeyDown(KeyCode.L);      //ġƮŰ= ü�� 100����

    }
    void Input_Skill()
    {
        if (!isStart || isCameraMove || isBoss || isElite)
            return;

        if (!OnSkill)
            OnSkill = Input.GetKeyDown(KeyCode.D);      //��ų
        if (Input.GetKeyUp(KeyCode.D))
            OnSkill = false;
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

    //��ų
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
            case Skill_Core.DropAttack:

                break;
            case Skill_Core.Crouch:
                if (OnSkill)
                {
                    isHit = true;
                    isCrouch = true;
                    anim.SetBool("isLieDown", true);
                    Physics2D.IgnoreLayerCollision(6, 7, true);
                }
                else
                {
                    isHit = false;
                    isCrouch = false;
                    anim.SetBool("isLieDown", false);
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

        followCount++;
        gameManager.FollowerSpawn();
        maxLife -= hpDecrease;
        yield return null;
    }


    void CoolDown()
    {
        if (!isStart)
            return;

        if (OnSkill)
        {
            curSkillDelay += Time.deltaTime;

            if (curSkillDelay > maxSkillDelay)
            {
                curSkillDelay = 0;
                OnCoolDown();

            }
        }
        else
            curSkillDelay = 0;
    }
    void OnCoolDown()
    {
        OnSkill = false;
    }

    //ü�� Ȯ��
    void LifeCheck()
    {
        if (curLife > maxLife)
            curLife = maxLife;
    }

    //����Ÿ�� ����
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
        if (E_IsSwitch)
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

    //ġƮ     Life+100
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

    //�ǰ�
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
        spriteRenderer.color = new Color(1, 1, 1, Alpha);
    }
    //���� ���� Ʈ����
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
