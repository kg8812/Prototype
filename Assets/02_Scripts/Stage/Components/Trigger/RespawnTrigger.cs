using System;
using System.Collections;
using System.Collections.Generic;
using Default;
using Sirenix.OdinInspector;
using UI;
using UnityEngine;

public class RespawnTrigger : SerializedMonoBehaviour
{
    [LabelText("리스폰 위치")] public List<GameObject> respawnPos;
    [LabelText("받는 데미지 (고정값)")] public float dmgTake;
    [LabelText("받는 데미지 (%)")] public float dmgTakeRatio;
    [LabelText("대기시간")] public float time;

    [HideInInspector] public GameObject lastRespawn;
    private void Awake()
    {
        respawnPos ??= new List<GameObject>();

        if (respawnPos.Count == 0)
        {
            Debug.LogError($"{name}의 리스폰 포인트가 설정되지 않았습니다.");
        }
        lastRespawn = respawnPos[0];
        foreach (var pos in respawnPos)
        {
            var child = pos.GetOrAddComponent<RespawnTriggerChild>();
            child.Init(this);
        }
    }

    IEnumerator Respawn(Player player)
    {
        float dmg = dmgTake + player.MaxHp * dmgTakeRatio / 100f;
        player.CurHp -= dmg;
        player.ControllerOff();
        if (player.IsDead) yield break;
        yield return new WaitForSeconds(time);
        FadeManager.instance.Fading(() =>
        {
            player.transform.position = lastRespawn.transform.position;
            player.CorrectingPlayerPosture();
            player.ControllerOn();
        });
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player _player))
        {
            StartCoroutine(Respawn(_player));
        }
    }
}
