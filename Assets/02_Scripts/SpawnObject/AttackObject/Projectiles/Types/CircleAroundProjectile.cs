using Apis;
using UnityEngine;

namespace Apis
{
    public class CircleAroundProjectile : AttackObject
    {
        public CircleAround move;

        private Rigidbody2D _rigid;

        public void Init(float speed, float radius, CircleAround.Direction dir = CircleAround.Direction.ClockWise)
        {
            move = new CircleAround(_attacker, transform, radius, speed, dir);
        }

        protected void FixedUpdate()
        {
            move?.Update();
        }
    }
}