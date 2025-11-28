namespace Apis.Interface
{
    public interface IActivation
    {
        public bool IsActivated { get; set; }

        public void OnActivated();
        public void OnDisActivated();
    }
}