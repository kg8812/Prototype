namespace Apis
{
    public interface ICommonMonsterState<T> : IState<T>
    {
        void OnCancel();
    }
}