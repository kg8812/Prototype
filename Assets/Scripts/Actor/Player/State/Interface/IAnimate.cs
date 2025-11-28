namespace PlayerState
{
    public interface IAnimate
    {
        // State OnEnter 직전 실행
        public void OnEnterAnimate();

        // State OnExit 직후 실행
        public void OnExitAnimate();
    }
}