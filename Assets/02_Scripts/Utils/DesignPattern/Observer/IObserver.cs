public interface IObserver
{
    //옵저버 패턴 옵저버
    public void Notify(Subject subject);
}

public interface IObserver<T>
{
    public void Notify(T value);
}

