using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum Level { Easy, Normal, Hard }
    public Level level;

    public enum Type { Mouse ,Snake };
    public Type enemyType;

    public enum Def_Type { Normal, Resist, Nimble, Solid }
    public Def_Type type;

    public float maxdistance;
    public float mindistance;
    public float speed;
    public float curAttackDelay;
    public float maxAttackDelay;
    public float curAggroTime;
    public float maxAggroTime;
    public  float dmg;

    protected float health;
    protected float maxHealth;

    public GameObject player;
    public ObjectManager objectManager;
    public GameManager gameManager;

    public Rigidbody2D rigid;
    public SpriteRenderer spriteRenderer;
    public Animator anim;

    protected bool isAttack;
    protected bool isHit;
    protected bool isFollow;
    protected bool isAggro;
    protected bool isDie;

    public Vector2 forward;
    protected float dist;

    public void FlipX(SpriteRenderer sprite)
    {
        if (sprite.flipX == false)
            sprite.flipX = true;
        else if (sprite.flipX == true)
            sprite.flipX = false;
    }
    public void Follow(Vector3 Pos,float distance,SpriteRenderer sprite)
    {
        if (player.transform.position.x - Pos.x < -distance)
        {
            forward = new Vector2(-speed, rigid.velocity.y).normalized;
            Watch(Pos, player.transform.position, sprite);
            isFollow = true;
        }
        else if (player.transform.position.x - Pos.x > distance)
        {
            forward = new Vector2(speed, rigid.velocity.y).normalized;
            Watch(Pos, player.transform.position, sprite);
            isFollow = true;
        }
    }
    public void Watch(Vector3 Pos, Vector3 target, SpriteRenderer sprite)
    {
        if (target.x - Pos.x > 0)
            sprite.flipX = true;
        if (target.x - Pos.x < 0)
            sprite.flipX = false;
    }
    public void WatchCheck(bool fol, SpriteRenderer sprite, Vector2 watch)
    {
        if (isFollow)
            return;
        if (spriteRenderer.flipX == false)
        { forward = new Vector2(-speed, rigid.velocity.y).normalized; }
        else if (spriteRenderer.flipX == true)
        { forward = new Vector2(speed, rigid.velocity.y).normalized; }
    }
}
