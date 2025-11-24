using Apis.DataType;

namespace Apis
{
    public abstract class Debuff_DotDmg : Debuff_base
    {
        SubBuffOptionDataType _option;

        protected SubBuffOptionDataType option
        {
            get
            {
                if (_option == null)
                {
                    BuffDatabase.DataLoad.TryGetSubBuffIndex(Type, out int index);
                    BuffDatabase.DataLoad.TryGetSubBuffOption(index, out _option);
                }

                return _option;
            }
        } 
        public float Dmg { get; protected set; }
        protected Debuff_DotDmg(Buff buff) : base(buff)
        {
            
        }

        public override void OnAdd()
        {
            base.OnAdd();
            SetDmg();
        }

        protected virtual void SetDmg()
        {
            Dmg = option.amount[0];
        }
    }
}
