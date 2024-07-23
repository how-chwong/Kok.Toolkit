namespace Kok.Toolkit.Core.Model;

/// <summary>
/// 观察者基类
/// </summary>
public abstract class ObserverBase<T> : IObserver<T>
{
    ///<inheritdoc />
    public void OnCompleted()
    {
    }

    ///<inheritdoc />
    public virtual void OnError(Exception error)
    {
    }

    ///<inheritdoc />
    public abstract void OnNext(T value);
}
