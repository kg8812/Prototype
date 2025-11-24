using System;
using System.Collections;
using System.Collections.Generic;
using Default;
using UnityEngine;


public class ChainObject : MonoBehaviour
{
    private SpriteRenderer render;
    
    private void Awake()
    {
        render = GetComponent<SpriteRenderer>();
        render.color = Color.white;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        Player player = other.transform.GetComponentInParentAndChild<Player>();
        if (player != null)
        { 
            render.color = Color.green;
            player.ActionController.enteredChainObjects.Enqueue(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        Player player = other.transform.GetComponentInParentAndChild<Player>();

        if (player != null)
        {
            render.color = Color.white;
            player.ActionController.enteredChainObjects.Remove(this);
        }
    }

    public void TurnOn()
    {
        render.color = Color.yellow;
    }

    public void TurnOff()
    {
        render.color = Color.white;
    }
    
}
