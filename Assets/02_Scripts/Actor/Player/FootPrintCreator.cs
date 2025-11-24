using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Apis;
using Apis;
using Default;
using UnityEngine;
using Random = UnityEngine.Random;

public class FootPrintCreator : MonoBehaviour
{
    private Actor actor;
    private IMovable mover;
    
    private ActorMovement actorMovement => mover?.ActorMovement;
    private void Awake()
    {
        mover = GetComponent<IMovable>();
        actor = GetComponent<Actor>();
    }

    public void SpawnFootPrint()
    {
        if (actorMovement.IsStick)
        {
            var hits = Physics2D.RaycastAll(mover.Position, Vector2.down, 2, LayerMasks.Trigger);
            Dictionary<FootPrintTrigger, RaycastHit2D> triggers = new();
            foreach (var x in hits)
            {
                if (x.collider != null && x.collider.TryGetComponent(out FootPrintTrigger t))
                {
                    triggers.Add(t,x);
                }
            }

            if (triggers.Count == 0) return;
            
            FootPrintTrigger trigger = triggers.Keys.OrderByDescending(x => x.priority).First();
            
            string[] addresses = trigger.groundType switch
            {
                FootPrintTrigger.GroundType.Water => Define.SFXList.WaterFootSteps,
                FootPrintTrigger.GroundType.Grass => Define.SFXList.GrassFootSteps,
                FootPrintTrigger.GroundType.Snow => Define.SFXList.SnowFootSteps,
                FootPrintTrigger.GroundType.Rock => Define.SFXList.RockFootSteps,
                _ => null
            };

            if (addresses == null) return;
            
            int rand = Random.Range(0, addresses.Length);
            var address = addresses[rand];
            GameManager.Sound.Play(address);
        }
    }
}
