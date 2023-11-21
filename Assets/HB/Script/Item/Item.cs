using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int coin;
    public float bounsCount;
    public float ran;

    public enum ItemType { Core,Coin };
    public enum CoreType { Damage, Speed,Health,None };
    public ItemType itemType;
    public CoreType coreType;
    public Vector2 ranVec;

    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    void OnEnable()
    {
        ran = Random.Range(-1f, 1f);
    }
    void Update()
    {
        if (itemType == ItemType.Core && rigid.velocity.y == 0)
            rigid.velocity = Vector2.zero;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag=="Ground")
        {
            if (itemType== ItemType.Coin)
            {
                if (bounsCount < 0)
                    return;
                ranVec = new Vector2(ran, bounsCount);
                rigid.AddForce(ranVec * 2, ForceMode2D.Impulse);

                bounsCount--;
            }
        }
    }
}
