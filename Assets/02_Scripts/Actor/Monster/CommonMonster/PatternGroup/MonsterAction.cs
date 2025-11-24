using UnityEngine;

namespace Apis
{
    public abstract class MonsterAction: ScriptableObject
    {
        protected CommonMonster2.CommonMonster2 _cM;
        protected bool isPlayed;

        public virtual void Action(CommonMonster2.CommonMonster2 monster)
        {
            isPlayed = false;
            _cM = monster;
        }
        public abstract void Update();
        public abstract void FixedUpdate();
        public abstract void OnCancel();

        public virtual void OnEnd()
        {
            
        }
    }
}