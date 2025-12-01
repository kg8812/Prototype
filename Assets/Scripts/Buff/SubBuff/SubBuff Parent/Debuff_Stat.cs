namespace Apis
{
    public abstract class Debuff_Stat : Debuff_base
    {
        protected Debuff_Stat(Buff buff) : base(buff)
        {
            switch (buff.ValueType)
            {
                case ValueType.Value:
                    Stat.AddValue(StatType, -amount[0]);
                    break;
                case ValueType.Ratio:
                    Stat.AddRatio(StatType, -amount[0]);
                    break;
            }
        }

        protected abstract ActorStatType StatType { get; }

        public override void PermanentApply()
        {
            base.PermanentApply();
            var _statUser = _user?.gameObject.GetComponent<IStatUser>();
            _statUser?.AddStat(StatType, -amount[0], buff.ValueType);
        }

        public override void TempApply(EventParameters parameters)
        {
            base.TempApply(parameters);

            switch (buff.ValueType)
            {
                case ValueType.Value:
                    parameters.statData.stat.AddValue(StatType, -amount[0]);
                    break;
                case ValueType.Ratio:
                    parameters.statData.stat.AddRatio(StatType, -amount[0]);
                    break;
            }
        }

        public override void OnAdd()
        {
            base.OnAdd();
        }
    }
}