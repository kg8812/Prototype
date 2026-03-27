using UnityEngine;

namespace Apis
{
    public class CircleAroundProjectile : AttackObject
    {
        private Rigidbody2D _rigid;
        public CircleAround move;

        protected void FixedUpdate()
        {
            move?.Update();
        }

        public void Init(float speed, float radius, CircleAround.Direction dir = CircleAround.Direction.ClockWise)
        {
            move = new CircleAround(_attacker, transform, radius, speed, dir);
        }
    }
}