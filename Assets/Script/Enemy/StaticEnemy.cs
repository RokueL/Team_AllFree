using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticEnemy : MonoBehaviour
{
    public int Count;
    public int pattern;
    public float Att_Distance;
    public float maxAttackDelay;

    float curAttackDelay;
    float dis;

    public ObjectManager objectManager;
    public GameObject player;
    public Enemy enemyScript;
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }
    void FixedUpdate()
    {
        Distance();
        Attack();
    }
    void Distance()
    {
        dis =Vector2.Distance(transform.position, player.transform.position);
    }
    void Attack()
    {
        curAttackDelay += Time.deltaTime;
        if (dis > Att_Distance)
            return;

        if (curAttackDelay < maxAttackDelay)
            return;
        pattern = pattern % 2 == 0 ? pattern + 1 :0;
        anim.SetTrigger("doAttack");
        if (pattern == 0)
        {
            for (float index = 0; index < Count; index++)
            {
                GameObject flowerAttack = objectManager.MakeObj("flowerAttack");

                flowerAttack.transform.position = transform.position + Vector3.down * 0.5f;
                Rigidbody2D F_rigid = flowerAttack.GetComponent<Rigidbody2D>();
                Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * index / (Count - 1)),
                    Mathf.Sin(-Mathf.PI * index / (Count - 1)));

                F_rigid.transform.Rotate(Vector3.forward * index * 45);
                F_rigid.AddForce(dirVec * 5, ForceMode2D.Impulse);
            }
        }
        else if (pattern == 1)
        {
            for (float index = 0; index < Count+1; index++)
            {
                GameObject flowerAttack = objectManager.MakeObj("flowerAttack");

                flowerAttack.transform.position = transform.position + Vector3.down * 0.5f;
                Rigidbody2D F_rigid = flowerAttack.GetComponent<Rigidbody2D>();
                Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * index / (Count)),
                    Mathf.Sin(-Mathf.PI * index / (Count)));

                F_rigid.transform.Rotate(Vector3.forward * index * 45);
                F_rigid.AddForce(dirVec * 5, ForceMode2D.Impulse);
            }
        }
        curAttackDelay = 0;
    }
}
