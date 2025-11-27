namespace PlayerState
{
    public interface IAutoEscape
    {
        public EPlayerState NextState { get; set; }
        public bool EscapeCondition();
    }
}