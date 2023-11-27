using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("플레이어 사운드")]
    public AudioSource walkSound;
    public AudioSource jumpSound;
    public AudioSource HitSound_1;
    public AudioSource HitSound_2;
    public AudioSource HitSound_3;
    public AudioSource meleeAttackSound;
    public AudioSource crouchAttackSound;
    public AudioSource rollAttackSound;
    public AudioSource summonAttackSound;
    public AudioSource healSound;
    public AudioSource weaponChangeSound;

    void Awake()
    {
        walkSound.volume = 0.7f;
        jumpSound.volume = 0.7f;
        HitSound_1.volume = 0.7f;
        HitSound_2.volume = 0.7f;
        HitSound_3.volume = 0.7f;
        meleeAttackSound.volume = 0.7f;
        crouchAttackSound.volume = 0.7f;
        rollAttackSound.volume = 0.7f;
        summonAttackSound.volume = 0.7f;
        healSound.volume = 0.7f;
        weaponChangeSound.volume = 0.7f;

    }
}
