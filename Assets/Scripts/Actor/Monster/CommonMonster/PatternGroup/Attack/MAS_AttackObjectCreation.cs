using System;
using System.Collections;
using System.Collections.Generic;
using Default;
using UnityEngine;

namespace Apis.CommonMonster2
{
    [CreateAssetMenu(fileName = "New AttackObjectCreation",
        menuName = "Scriptable/Monster/Attack/attackObjectCreation")]
    [Serializable]
    public class MAS_AttackObjectCreation : MonsterAction
    {
        private const string prePath = "Prefabs/AttackEffects/";
        public List<AttackObjectData> AttackObjectDatas;
        public float creationDelay;

        protected CommonMonster2 _monster;


        private Coroutine myCr;
        protected GameObject proj;
        private float realCreationDelay;

        public override void Action(CommonMonster2 monster)
        {
            realCreationDelay = Utils.CalculateDurationWithAtkSpeed(monster, creationDelay);
            // Debug.Log("Action");
            base.Action(monster);
            _monster = monster;
            if (creationDelay <= 0)
                foreach (var atk in AttackObjectDatas)
                    CreateAttackObject(monster, atk);
            else
                myCr = GameManager.instance.StartCoroutineWrapper(CreateAttackObjects());
        }

        public override void Update()
        {
        }

        public override void FixedUpdate()
        {
        }

        public override void OnCancel()
        {
            if (myCr != null) GameManager.instance.StopCoroutineWrapper(myCr);
        }

        protected virtual IEnumerator CreateAttackObjects()
        {
            foreach (var atk in AttackObjectDatas)
            {
                CreateAttackObject(_monster, atk);
                yield return new WaitForSeconds(realCreationDelay);
            }
        }

        protected virtual void CreateAttackObject(CommonMonster2 monster, AttackObjectData atk)
        {
            Debug.Log("create attack object");
            proj = GameManager.Factory.Get(FactoryManager.FactoryType.AttackObject, atk.obj.gameObject.name);
            proj.transform.position = monster.Position + atk.offset;
        }
    }
}