namespace Kok.Toolkit.Core.Model
{
    /// <summary>
    /// 退订器
    /// 适用于观察者模式中的取消订阅
    /// </summary>
    public class Unsubscriber<T> : IDisposable
    {
        private readonly ISet<IObserver<T>>? _observers;
        private readonly IObserver<T>? _observer;

        /// <summary>
        /// 构造一个实例
        /// </summary>
        /// <param name="observers"></param>
        /// <param name="observer"></param>
        public Unsubscriber(ISet<IObserver<T>> observers, IObserver<T> observer)
        {
            _observers = observers;
            _observer = observer;
        }

        ///<inheritdoc />
        public void Dispose()
        {
            if (_observers != null && _observer != null)
                _observers.Remove(_observer);
        }
    }
}
