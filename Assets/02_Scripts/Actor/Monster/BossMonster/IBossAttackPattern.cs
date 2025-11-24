using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Apis
{
    public interface IBossAttackPattern
    {
        public void OnPatternEnter();
        public void Attack(int index);
        public void SetCollider(int index);
        public void End();
        public void Cancel();
    }
}