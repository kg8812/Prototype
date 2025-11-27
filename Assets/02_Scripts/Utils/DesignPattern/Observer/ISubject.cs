public interface ISubject<T>
{
    public void Attach(IObserver<T> observer);

    public void Detach(IObserver<T> observer);

    public void NotifyObservers();
}