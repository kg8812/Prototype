using System;
using System.Collections.Generic;
using UnityEngine;

namespace Apis
{
    [CreateAssetMenu(fileName = "New Pattern", menuName = "Scriptable/Monster/Pattern")]
    [Serializable]
    public class Pattern : ScriptableObject
    {
        public List<MonsterAction> attacks;
        public List<MonsterAction> movements;
        public bool isCliffRun;
        public bool canAtkWithoutTurn;
        public bool haveMoving;

        public void CancelPattern()
        {
            foreach (var atk in attacks) atk.OnCancel();

            foreach (var movement in movements) movement.OnCancel();
        }
    }
}