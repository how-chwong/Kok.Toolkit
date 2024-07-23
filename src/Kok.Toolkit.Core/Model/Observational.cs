namespace Kok.Toolkit.Core.Model;

/// <summary>
/// 具备观察行为的对象
/// 既是观察者又是被观察者
/// </summary>
public abstract class Observational<TObservable, TObserver> : ObservableBase<TObservable>, IObserver<TObserver>
{
    ///<inheritdoc />
    public virtual void OnCompleted()
    {
    }

    ///<inheritdoc />
    public virtual void OnError(Exception error)
    {
    }

    ///<inheritdoc />
    public abstract void OnNext(TObserver value);
}
