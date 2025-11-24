using UnityEngine;
namespace Command
{
    [CreateAssetMenu(fileName = "PressDown", menuName = "ActorCommand/Player/PressDown", order = 1)]    
    public class PressDown : PlayerCommand
    {
        public bool Value;
        protected override void Invoke(Player go)
        {
            go.Controller.PressDown(Value);
        }

        public override bool InvokeCondition(Player go)
        {
            return true;
        }
    }
}

