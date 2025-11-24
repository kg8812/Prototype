using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnTriggerChild : MonoBehaviour
{
    private RespawnTrigger p;
    public void Init(RespawnTrigger p)
    {
        this.p = p;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        p.lastRespawn = gameObject;
    }
    
}
