using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    void OnEnable()
    {
        Invoke("Off", 1f);
    }
    void Off()
    {
        gameObject.SetActive(false);
    }

}
