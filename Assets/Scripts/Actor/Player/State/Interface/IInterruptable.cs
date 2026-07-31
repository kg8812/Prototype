namespace PlayerState
{
    public interface IInterruptable
    {
        public float InterruptTime { get; set; }
        public EPlayerState[] InterruptableStates { get; }
    }
}