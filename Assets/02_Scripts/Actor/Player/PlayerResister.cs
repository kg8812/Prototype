using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerResister : MonoBehaviour
{
    readonly List<Collider2D> enemyList = new();
    Player _player;
    bool isResist = false;
    float resistForce;
    // float customFactor;

    // const float threshold = 0.3f;
    float threshold;
    const float INF = 1000;

    void Awake()
    {
        GameManager.Scene.WhenSceneLoaded.AddListener((s) =>
        {
            enemyList.Clear();
        });
        _player = gameObject.transform.parent.GetComponent<Player>();
        resistForce = _player.ResistForce;
        threshold = _player.DragFactor;

        var collider = _player.GetComponent<CapsuleCollider2D>();
        Vector2 offsetRef = collider.offset;
        Vector2 sizeRef = collider.size;

        collider = gameObject.GetComponent<CapsuleCollider2D>();
        collider.offset = offsetRef;
        collider.size = sizeRef;
    }

    void Update()
    {
        if(isResist && enemyList.Count == 0)
        {
            isResist = false;
            _player.ActorMovement.dragFactor = 1.0f;
        }
        if(isResist && _player.IsMove)
        {
            if(enemyList.Count == 0) return;

            float distance = _player.Position.x - enemyList[0].transform.position.x;
            int dir = distance > 0 ? 1 : -1;
            if((int)_player.Direction != dir)
            {
                // 현재 dragForce가 threshold 보다 크면 target 거리에 기반해 재설정
                if(_player.ActorMovement.dragFactor > threshold)
                {
                    // _player.actorMovement.dragFactor = Mathf.Lerp(_player.actorMovement.dragFactor, threshold, Time.deltaTime * 5);
                    _player.ActorMovement.dragFactor = threshold;
                }
                // 특정 값 이후부터는 감소 안됨
            }
            else
            {
                _player.ActorMovement.dragFactor = 1.0f;
                // dragForce 1로 초기화
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy"))
        {   
            var otherActor = other.GetComponent<Actor>();

            if(otherActor == null || !otherActor.IsResist) return;

            if(!enemyList.Contains(other))
            {
                enemyList.Add(other);
                isResist = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Enemy"))
        {
            if(enemyList.Contains(other))
            {
                enemyList.Remove(other);
            }
        }
    }

    private bool onPush = false;
    public void Resist()
    {
        if(enemyList.Count > 0 && _player.ActorMovement.IsStick)
        {
            // isResist가 false일때까지 target 반대 방향으로 이동
            // TODO: 밀려나는 중간에 이동 키 누르면?
            // TODO: 여려명 동시 겹쳐있는 경우?
            if(enemyList.Count == 1)
            {
                onPush = true;
                float dir = _player.Position.x - enemyList[0].transform.position.x > 0 ? 1 : -1;
                if(_player.ActorMovement.CheckWall(dir, 0.1f)) return;
                // _player.Rb.velocity = dir * resistForce * Vector2.right;
                EActorDirection d = dir > 0 ? EActorDirection.Right : EActorDirection.Left;
                // _player.MoveComponent.ActorMovement.Move(d, resistForce);
                _player.MoveComponent.ForceActorMovement.Move(d, 1, false, 0.05f, resistForce);
            }
        }
        if(onPush && enemyList.Count == 0)
        {
            onPush = false;
            _player.Stop();
        }
    }
}
