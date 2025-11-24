using Apis.StageObj;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Apis
{
    public class ArenaDoor : Door
    {
        protected override void Start()
        {
            GameManager.instance.whenArenaStateChanged.AddListener(MoveDoor);
        }
    }
}