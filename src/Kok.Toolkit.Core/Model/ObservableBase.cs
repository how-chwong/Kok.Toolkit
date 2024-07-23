namespace Kok.Toolkit.Core.Model;

/// <summary>
/// 可被观察的对象基类
/// </summary>
/// <typeparam name="T">通知消息的类型</typeparam>
public abstract class ObservableBase<T> : IObservable<T>
{
    /// <summary>
    /// 观察者集合
    /// </summary>
    protected readonly HashSet<IObserver<T>> Observers = new();

    ///<inheritdoc />
    public IDisposable Subscribe(IObserver<T> observer)
    {
        Observers.Add(observer);
        return new Unsubscriber<T>(Observers, observer);
    }

    /// <summary>
    /// 通知所有的观察者
    /// </summary>
    /// <param name="value"></param>
    protected void Notify(T value)
        => Array.ForEach(Observers.ToArray(), o => o.OnNext(value));
}
