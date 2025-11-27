using System.Collections.Generic;
using System.Linq;
using Default;
using Sirenix.OdinInspector;
using UnityEngine;

public class SuperJump : PlatformObject
{
    public float jumpPower;
    public float cooldown;
    [LabelText("낙하속도 보정비율 (%)")] public float correctionRatio;

    private float curCd;
    private List<Rigidbody2D> targets = new();

    private Dictionary<Rigidbody2D, float> ySpeeds = new();

    private void Awake()
    {
        curCd = 0;
        targets ??= new List<Rigidbody2D>();
        ySpeeds ??= new Dictionary<Rigidbody2D, float>();
    }

    private void FixedUpdate()
    {
        curCd -= Time.fixedDeltaTime;
        if (curCd <= 0 && targets.Count > 0)
        {
            DoJump();
            curCd = cooldown;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (CheckAvailable(other.gameObject))
        {
            var target = other.transform.GetComponentInParentAndChild<Rigidbody2D>();
            if (target == null) return;

            if (!targets.Contains(target)) targets.Add(target);
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (CheckAvailable(other.gameObject))
        {
            var target = other.transform.GetComponentInParentAndChild<Rigidbody2D>();
            if (target == null) return;

            if (targets.Contains(target)) targets.Remove(target);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (curCd > 0) return;

        if (CheckAvailable(other.gameObject))
        {
            var target = other.transform.GetComponentInParentAndChild<Rigidbody2D>();
            if (target == null) return;

            ySpeeds.TryAdd(target, target.linearVelocity.y);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (CheckAvailable(other.gameObject))
        {
            var target = other.transform.GetComponentInParentAndChild<Rigidbody2D>();
            if (target == null) return;

            ySpeeds.Remove(target);
        }
    }

    private bool IsStandingOnPlatform(Rigidbody2D target)
    {
        var hits = Physics2D.RaycastAll(target.transform.position, Vector2.down, 1);
        foreach (var hit in hits)
            if (hit.collider.gameObject == gameObject) // 현재 플랫폼과 충돌했는지 확인
                return true;

        return false;
    }

    private void DoJump()
    {
        var removes = new List<Rigidbody2D>();

        targets.ForEach(x =>
        {
            if (IsStandingOnPlatform(x))
            {
                float y = 0;
                if (ySpeeds.TryGetValue(x, out var speed)) y = -speed > 0 ? -speed : 0;
                x.linearVelocity = new Vector2(x.linearVelocity.x, y * correctionRatio / 100f);
                x.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
                removes.Add(x);
            }
        });
        targets = targets.Except(removes).ToList();
    }
}