using UnityEngine;

namespace Command
{
    [CreateAssetMenu(fileName = "Resist", menuName = "ActorCommand/Player/Resist")]
    public class Resist : PlayerCommand
    {
        protected override void Invoke(Player go)
        {
            go.resister.Resist();
        }

        public override bool InvokeCondition(Player go)
        {
            return true;
        }
    }
}