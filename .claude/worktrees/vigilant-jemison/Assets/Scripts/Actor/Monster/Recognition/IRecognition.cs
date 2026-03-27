namespace Apis.Interface
{
    public interface IRecognition
    {
        public bool IsRecognized { get; }

        public void OnRecognized();
        public void OnDisRecognized();
    }
}