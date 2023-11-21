using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum Att_Type { Normal,Power,Sharp,Mystic}
    public Att_Type type;

    public float life;
    public float dmg;
    public float speed;
    public float jumpPower;

    public float coin;
    public float curAttackDelay;
    public float maxAttackDelay;

    public bool rightCheck;
    public bool clearMap;
    public bool isStart;

    //core
    public bool damageCore;
    public bool speedCore;
    public bool healthCore;

    //Attack_Type
    public bool isNormal;
    public bool isPower;
    public bool isSharp;
    public bool isMystic;

    bool Q_IsSwitch;
    bool E_IsSwitch;
    //Controll
    float fAxis;

    bool isGround;
    bool isJump;
    bool jumping;
    bool isAtt;


    //Trigger
    bool isTouchRoom;
    public bool isBoss;

    bool isHit;

    bool isCheat;


    public ObjectManager objectManager;
    public GameManager gameManager;
    public BoxCollider2D attack;

    Animator anim;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        isNormal = true;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            gameManager.ShakeCam();
        InPut();

        MoveMent();
        Jump();
        StartCoroutine(Attack());

        StartCoroutine(Attack_Type());

        StartCoroutine(LifeCheat());
    }
    //KeyControll
    void MoveMent()
    {
        if (isTouchRoom&&!clearMap&& fAxis == 1)
        {
            fAxis = 0;
        }
        if (isBoss)
            fAxis = 0;

        rigid.velocity = new Vector2(fAxis * speed, rigid.velocity.y);
        if (fAxis != 0)
        {
            if (fAxis > 0)
            {
                rightCheck = true;
                spriteRenderer.flipX = false;
                attack.offset = new Vector2(0.6f, 0);
            }
            else if (fAxis < 0)
            {
                rightCheck = false;
                spriteRenderer.flipX = true;
                attack.offset = new Vector2(-0.6f, 0);
            }
        }
    }
    void InPut()
    {
        if (!isStart)
            return;

        fAxis = Input.GetAxisRaw("Horizontal");
        isAtt=Input.GetKey(KeyCode.A);
        isJump=Input.GetKeyDown(KeyCode.S);
        Q_IsSwitch = Input.GetKeyDown(KeyCode.Q);
        E_IsSwitch = Input.GetKeyDown(KeyCode.E);
        isCheat = Input.GetKeyDown(KeyCode.L);

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

        attack.enabled = true;
        anim.SetTrigger("doAttack");
        yield return new WaitForSeconds(0.1f);

        attack.enabled = false;
        isAtt = false;
        curAttackDelay = 0;
    }

    //Attack_Type
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

    //Cheat     Life+100
    IEnumerator LifeCheat()
    {
        if (!isCheat)
            yield break;

        yield return null;
        life += 100;

        yield return new WaitForSeconds(0.5f);
        isCheat = false;
    }

    //Hit
    IEnumerator OnHit(float dmg)
    {
        if (isHit)
            yield break;

        isHit = true;
        life -= dmg;
        ReturnSprite(0.8f);
        anim.SetTrigger("doHit");
        if (life < 0)
        {
            gameManager.GameOver();
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
    //Trigger
    IEnumerator BossStart()
    {
        isBoss = true;
        yield return new WaitForSeconds(0.1f);

        isBoss = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag=="Enemy")
        {
            Enemy enemyLogic = collision.gameObject.GetComponentInParent<Enemy>();

            rigid.velocity = Vector2.zero;
            rigid.AddForce(Vector2.up * 4, ForceMode2D.Impulse);
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
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "EnemyAttack")
        {
            rigid.velocity = Vector2.zero;
            EnemyObject enemyLogic = collision.gameObject.GetComponent<EnemyObject>();

            rigid.AddForce(Vector2.up*2, ForceMode2D.Impulse);
            StartCoroutine(OnHit(enemyLogic.dmg));
        }
        if (collision.gameObject.tag == "BossAttack")
        {
            rigid.velocity = Vector2.zero;
            rigid.AddForce(Vector2.up * 4, ForceMode2D.Impulse);
            BossAttackObject objectLogic = collision.gameObject.GetComponent<BossAttackObject>();
            switch (objectLogic.attackType)
            {
                case "Shot":
                    StartCoroutine(OnHit(objectLogic.dmg));
                    break;
                case "Range":
                    StartCoroutine(OnHit(objectLogic.dmg));
                    break;
            }
        }
        if (collision.gameObject.tag == "Boss")
        {
            rigid.velocity = Vector2.zero;
            Boss bossLogic =collision.gameObject.GetComponentInParent<Boss>();
            rigid.AddForce(Vector2.up * 4, ForceMode2D.Impulse);
            StartCoroutine(OnHit(bossLogic.dmg));
        }

        if(collision.gameObject.tag=="TriggerMap")
        {
            isTouchRoom = true;
        }
        if(collision.gameObject.tag=="BossTrigger")
        {
            StartCoroutine(BossStart());
            collision.gameObject.SetActive(false);
        }

        if(collision.gameObject.tag=="Debris")
        {
            Debris debrisLogic = collision.gameObject.GetComponent<Debris>();
            collision.gameObject.SetActive(false);

            rigid.velocity = Vector2.zero;
            rigid.AddForce(Vector2.up * 4, ForceMode2D.Impulse);

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
