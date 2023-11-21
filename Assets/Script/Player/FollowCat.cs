using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCat : MonoBehaviour
{
    bool nonJump;
    bool jumping;
    bool isGround;
    bool isJump;
    bool isAtt;

    float speed;
    float jumpPower;
    float curAttackDelay;
    float maxAttackDelay;
    float jumpDelay;

    [Header("[팔로우 부모]")]
    public Transform parent;

    int followDelay;
    Vector2 followPos;
    Queue<float> parentPosX;

    Rigidbody2D followRigid;

    public GameObject player;
    public BoxCollider2D attack;
    Animator Followanim;
    SpriteRenderer spriteRenderer;
    void Awake()
    {
        jumpPower = 12;
        maxAttackDelay = 0.5f;
        jumpDelay = 0.35f;

        spriteRenderer = GetComponent<SpriteRenderer>();
        Followanim = GetComponent<Animator>();
        followRigid = GetComponent<Rigidbody2D>();
        parentPosX = new Queue<float>();
    }
    void OnEnable()
    {
        StartCoroutine(Check());
    }
    IEnumerator Check()
    {
        yield return null;
        Player playerLogic = player.GetComponent<Player>();

        switch (playerLogic.followCount)
        {
            case 1:
                followDelay = 10;
                speed = 2.5f;
                break;

            case 2:
                followDelay = 15;
                speed = 2f;
                break;

            case 3:
                followDelay = 20;
                speed = 2f;
                break;
        }
    }
    void Update()
    {
        StartCoroutine(Jump());
        StartCoroutine(Attack());
        Follow();
        Watch();
        InPut();
    }
    void Follow()
    {
        if (isAtt)
            return;
        followRigid.velocity = new Vector2(followPos.x*speed, followRigid.velocity.y);

        if (followPos.x > 0)
        {
            spriteRenderer.flipX = false;
            attack.offset = new Vector2(0.4f, 0);
        }
        if (followPos.x <= 0)
        {
            spriteRenderer.flipX = true;
            attack.offset = new Vector2(-0.4f, 0);
        }
    }
    void Watch()
    {
        parentPosX.Enqueue(parent.position.x-transform.position.x);

        if (parentPosX.Count > followDelay)
            followPos.x = parentPosX.Dequeue();

    }
    void InPut()
    {
        isAtt = Input.GetKeyDown(KeyCode.A);          //공격
        isJump = Input.GetKeyDown(KeyCode.S);         //점프

    }
    IEnumerator Jump()
    {
        if (followRigid.velocity.y == 0)
        {
            isGround = true;
            jumping = false;
        }
        if (isJump && jumping&&!nonJump)
        {
            nonJump = true;
            yield return new WaitForSeconds(jumpDelay);
            Followanim.SetTrigger("doJump");
            followRigid.velocity = new Vector2(followRigid.velocity.x, 0f);
            followRigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            jumping = false;
            nonJump = false;
        }
        if (isJump && isGround&&!nonJump)
        {
            nonJump = true;
            yield return new WaitForSeconds(jumpDelay);

            Followanim.SetTrigger("doJump");
            followRigid.velocity = new Vector2(followRigid.velocity.x, 0f);
            followRigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            isGround = false;
            jumping = true;
            nonJump = false;
        }
    }
    IEnumerator Attack()
    {
        curAttackDelay += Time.deltaTime;
        if (curAttackDelay < maxAttackDelay)
            yield break;
        if (!isAtt)
            yield break;

        attack.enabled = true;
        Followanim.SetTrigger("doAttack");
        yield return new WaitForSeconds(0.1f);

        attack.enabled = false;
        isAtt = false;
        curAttackDelay = 0;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            followRigid.velocity = new Vector2(followRigid.velocity.x, 0);
        }
    }
 }
