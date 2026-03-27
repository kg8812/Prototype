using System.Collections.Generic;
using UnityEngine;

public class PlayerResister : MonoBehaviour
{
    [SerializeField] private string ResistTag;
    private readonly List<Collider2D> enemyList = new();
    private Player _player;
    private bool isResist;

    private bool onPush;

    private float resistForce;
    // float customFactor;

    // const float threshold = 0.3f;
    private float threshold;

    private void Awake()
    {
        GameManager.Scene.WhenSceneLoaded.AddListener(s => { enemyList.Clear(); });
        _player = gameObject.transform.parent.GetComponent<Player>();
        resistForce = _player.ResistForce;
        threshold = _player.DragFactor;

        var _collider = _player.GetComponent<CapsuleCollider2D>();
        var offsetRef = _collider.offset;
        var sizeRef = _collider.size;

        _collider = gameObject.GetComponent<CapsuleCollider2D>();
        _collider.offset = offsetRef;
        _collider.size = sizeRef;
    }

    private void Update()
    {
        if (isResist && enemyList.Count == 0)
        {
            isResist = false;
            _player.ActorMovement.dragFactor = 1.0f;
        }

        if (isResist && _player.IsMove)
        {
            if (enemyList.Count == 0) return;

            var distance = _player.Position.x - enemyList[0].transform.position.x;
            var dir = distance > 0 ? 1 : -1;
            if ((int)_player.Direction != dir)
            {
                // 현재 dragForce가 threshold 보다 크면 target 거리에 기반해 재설정
                if (_player.ActorMovement.dragFactor > threshold)
                    // _player.actorMovement.dragFactor = Mathf.Lerp(_player.actorMovement.dragFactor, threshold, Time.deltaTime * 5);
                    _player.ActorMovement.dragFactor = threshold;
                // 특정 값 이후부터는 감소 안됨
            }
            else
            {
                _player.ActorMovement.dragFactor = 1.0f;
                // dragForce 1로 초기화
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(ResistTag))
        {
            var otherActor = other.GetComponent<Actor>();

            if (otherActor == null || !otherActor.IsResist) return;

            if (!enemyList.Contains(other))
            {
                enemyList.Add(other);
                isResist = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(ResistTag))
            if (enemyList.Contains(other))
                enemyList.Remove(other);
    }

    public void Resist()
    {
        if (enemyList.Count > 0 && _player.ActorMovement.IsStick)
            // isResist가 false일때까지 target 반대 방향으로 이동
            // TODO: 밀려나는 중간에 이동 키 누르면?
            // TODO: 여려명 동시 겹쳐있는 경우?
            if (enemyList.Count == 1)
            {
                onPush = true;
                float dir = _player.Position.x - enemyList[0].transform.position.x > 0 ? 1 : -1;
                if (_player.ActorMovement.CheckWall(dir, 0.1f)) return;
                // _player.Rb.velocity = dir * resistForce * Vector2.right;
                var d = dir > 0 ? EActorDirection.Right : EActorDirection.Left;
                // _player.MoveComponent.ActorMovement.Move(d, resistForce);
                _player.MoveComponent.ForceActorMovement.Move(d, 1, false, 0.05f, resistForce);
            }

        if (onPush && enemyList.Count == 0)
        {
            onPush = false;
            _player.Stop();
        }
    }
}