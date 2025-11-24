using Default;
using UnityEngine;

namespace Apis
{
    public class GoseguBossSpineEvents : MonoBehaviour
    {
        GoseguBoss boss;
        // Start is called before the first frame update
        void Start()
        {
            boss = Utils.GetComponentInParentAndChild<GoseguBoss>(gameObject);
        }

        public void Attack1()
        {
            boss.Attack1();
        }
        public void Attack2()
        {
            boss.Attack2();
        }
        public void Attack3Move(int index)
        {
            boss.Attack3Move(index);
        }
        public void Attack4()
        {
            boss.Attack4();
        }
        public void Attack5Jump1()
        {
            boss.Attack5Jump1();
        }

        public void Flip()
        {
            boss.Flip();
        }

        public void ThrowToTarget(int num)
        {
            boss.ThrowToPlayerDir(num);
        }
        
        public void Attack6()
        {
            boss.Attack6();
        }

        public void Alert(string str)
        {
            boss.treeRunner.Alert(str);
        }

        public void SetIdle()
        {
            boss.IdleOn();
        }

        public void AfterJump()
        {
            boss.AfterJump();
        }

        public void SetPointPos()
        {
            boss.SetPointPos();
        }
    }
}