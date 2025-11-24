using System.Collections;
using System.Collections.Generic;
using Apis;
using Default;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

[HideLabel]
[System.Serializable]
public struct ActionInfo
{
    [LabelText("드릴 공격정보")] public ProjectileInfo drillAtkInfo;
    [LabelText("드릴 공격계수")] public float drillDmg;
    [LabelText("드릴 신체계수")] public float drillBodyRatio;
    [LabelText("드릴 영혼계수")] public float drillSpiritRatio;
    [LabelText("드릴 기량계수")] public float drillFinesseRatio;
    [LabelText("사슬 발사시간")] public float chainSpeed;
    [LabelText("사슬 발사Ease")] public Ease chainEase;
    [LabelText("사슬 발사 후 이동시간")] public float moveSpeed;
}

public class ActionController
{
    private readonly ActionInfo _info;
    private readonly Player _player;

    public readonly CustomQueue<ChainObject> enteredChainObjects = new();
    public ActionController(Player player, ActionInfo info)
    {
        _info = info;
        _player = player;
    }

    public void SpawnIceDrill()
    {
        AttackObject drill = _player.EffectSpawner.Spawn(Define.PlayerEffect.IceDrill, _player.Position, false)
            .GetComponent<AttackObject>();
        drill.Init(_player,new FixedAmount((_player.Body * _info.drillBodyRatio / 100f +
                                            _player.Spirit * _info.drillSpiritRatio / 100f +
                                            _player.Finesse * _info.drillFinesseRatio / 100f) * _info.drillDmg / 100f));
        drill.Init(_info.drillAtkInfo);
        drill.gameObject.SetRadius(0.5f, _player.Direction);
    }

    public void DoChainAction()
    {
        if (enteredChainObjects.Count == 0) return;
        
        var chainObject = enteredChainObjects.Dequeue();
        Vector2 endPos = chainObject.transform.position;
        chainObject.TurnOff();
        enteredChainObjects.Clear();
        
        _player.SetState(EPlayerState.Idle);
        var chain = GameManager.Factory.Get<SpriteRenderer>(FactoryManager.FactoryType.Normal,
            Define.PlayerEffect.Chain, _player.Position);
        chain.size = new Vector2(0, chain.size.y);
        Sequence seq = DOTween.Sequence();
        seq.Append(chain.LaunchTileSprite(endPos,_info.chainSpeed, _info.chainEase));
        seq.Append(chain.ReturnTileSize(_info.moveSpeed, _info.chainEase));
        seq.Join(chain.transform.DOMove(endPos, _info.moveSpeed).SetEase(_info.chainEase).OnUpdate(() =>
        {
            _player.Position = chain.transform.position;
        }).SetUpdate(UpdateType.Fixed));
       
    }
}
