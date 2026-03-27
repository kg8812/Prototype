using Apis.DataType;

namespace Apis
{
    public abstract class Debuff_DotDmg : Debuff_base
    {
        private SubBuffOptionDataType _option;

        protected Debuff_DotDmg(Buff buff) : base(buff)
        {
        }

        protected SubBuffOptionDataType option
        {
            get
            {
                if (_option == null)
                {
                    BuffDatabase.DataLoad.TryGetSubBuffIndex(Type, out var index);
                    BuffDatabase.DataLoad.TryGetSubBuffOption(index, out _option);
                }

                return _option;
            }
        }

        public float Dmg { get; protected set; }

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