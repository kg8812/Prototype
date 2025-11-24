using System;
using System.Collections.Generic;
using Apis.SkillTrees;
using Apis;
using Apis.Managers;
using Default;
using Managers;
using Save.Schema;
using Sirenix.OdinInspector;
using UnityEngine;

public class SkillTreePickUp : DropItem
{
    [InfoBox("1 - 15 사이로 입력. 해당 순서의 스킬트리가 캐릭터 전부 오픈됨")]
    public List<int> skillTreeIndex;

    private void Awake()
    {
        isInteractable = true;
    }

    public override void InvokeInteraction()
    {
        var enums = Enum.GetValues(typeof(PlayerType));
        skillTreeIndex?.ForEach(x =>
        {
            foreach (PlayerType playerType in enums)
            {
                SkillTreeDatas.activatedIndex.Add(
                    ((int)playerType + 1) * 100 + x);
            }
        });
        SystemManager.SystemAlert("스킬트리 활성화",null);
        Destroy(gameObject);
    }
}
