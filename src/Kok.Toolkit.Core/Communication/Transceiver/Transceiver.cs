using Kok.Toolkit.Core.Extension;
using Kok.Toolkit.Core.Log;
using Kok.Toolkit.Core.Net;
using Kok.Toolkit.Core.Serialization.Binary;
using Kok.Toolkit.Core.Timers;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Kok.Toolkit.Core.Communication.Transceiver;

/// <summary>
/// 报文收发器
/// </summary>
/// <typeparam name="T"></typeparam>
public class Transceiver<T> where T : class, new()
{
    #region 构造

    /// <summary>
    /// 定时器类型
    /// </summary>
    protected readonly TimerType TimerType = TimerType.AntiReentry;

    /// <summary>
    /// 构造函数
    /// </summary>
    public Transceiver()
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="timerType"></param>
    public Transceiver(TimerType timerType)
    {
        TimerType = timerType;
    }

    #endregion 构造

    #region 启动停止

    /// <summary>
    /// 收发器使用的UDP通信客户端
    /// </summary>
    private UdpClient? _udpClient;

    /// <summary>
    /// 收发器名称
    /// </summary>
    public string Name { get; protected set; } = string.Empty;

    /// <summary>
    /// 启动收发器
    /// </summary>
    /// <param name="localIp">本机使用IP</param>
    /// <param name="localPort">本机使用端口</param>
    /// <param name="name">收发器名称</param>
    /// <returns></returns>
    public virtual bool Start(string localIp, int localPort, string name = "")
    {
        try
        {
            Name = name;
            if (!Network.TryParseEndPoint(localIp, localPort, out var localEndPoint) || localEndPoint == null)
            {
                Tracker.WriteError($"{Name}使用了非法的网络地址，{localIp}:{localPort}");
                return false;
            }
            if (!Network.IsLocalIp(localIp))
            {
                Tracker.WriteError($"{Name} {localIp}:{localPort}不是本地网络地址,请将该地址加入本地网卡");
                return false;
            }

            if (localEndPoint.CheckPort(NetworkProtocol.Udp))
            {
                Tracker.WriteError($"{Name} {localIp}:{localPort}已被占用");
                return false;
            }

            _udpClient = new UdpClient(localEndPoint);
            _udpClient.SetIOControl();

            _udpClient.BeginReceive(Receive, null);
            BuildTimer(TransmitterBuilder);
            return true;
        }
        catch (Exception ex)
        {
            Tracker.WriteError($"收发器启动失败：{ex.Message} IP:{localIp}");
            Stop();
            return false;
        }
    }

    /// <summary>
    /// 构建定时器
    /// </summary>
    /// <param name="builder"></param>
    private void BuildTimer(TransmitterBuilder<T>? builder)
    {
        if (builder?.IsAvailable != true) return;

        _timer = TimerType switch
        {
            TimerType.Multimedia => new MultimediaTimer(SendWork, builder, builder.Interval),
            TimerType.AntiReentry => new AntiReTimer(builder.ChangedJudges, SendWork, TransmitterBuilder,
                builder.Interval, builder.Type == TransmitterType.FixedCycle ? builder.PeriodCount : 0),
            _ => new AntiReTimer(builder.ChangedJudges, SendWork, TransmitterBuilder, builder.Interval,
                builder.Type == TransmitterType.FixedCycle ? builder.PeriodCount : 0)
        };
    }

    /// <summary>
    /// 停止收发器
    /// </summary>
    public virtual void Stop()
    {
        _isReceiverStopped = true;
        _timer?.Stop();
        Thread.Sleep(500);
        _udpClient?.Close();
        _udpClient?.Dispose();
    }

    #endregion 启动停止

    #region 发报机

    /// <summary>
    /// 发报机构建器
    /// </summary>
    protected TransmitterBuilder<T>? TransmitterBuilder;

    /// <summary>
    /// 定时器
    /// </summary>
    private ITimer? _timer;

    /// <summary>
    /// 构建发报机
    /// </summary>
    /// <param name="builder"></param>
    public void SetTransmitter(TransmitterBuilder<T> builder)
    {
        TransmitterBuilder = builder;
    }

    /// <summary>
    /// 发送任务
    /// </summary>
    /// <param name="transmitterBuilder"></param>
    protected virtual void SendWork(object? transmitterBuilder)
    {
        if (transmitterBuilder is TransmitterBuilder<T> builder)
            MessageSender(builder);
    }

    /// <summary>
    /// 报文发送真正任务
    /// </summary>
    /// <typeparam name="TMsg"></typeparam>
    /// <param name="builder"></param>
    protected void MessageSender<TMsg>(TransmitterBuilder<TMsg> builder)
    {
        if (builder.GenerateAction == null) return;
        var data = builder.GenerateAction.Invoke(builder.GenerateArgs);
        if (data == null) return;
        if (builder.TargetEndPoints.IsEmpty())
        {
            Tracker.WriteWarn($"未配置{typeof(TMsg)}报文的目标地址，取消该类报文的发送");
            return;
        }
        builder.TargetEndPoints.ForEach(epa =>
        {
            if (_udpClient == null)
            {
                Tracker.WriteWarn("收发器监控Socket已释放");
                return;
            }
            builder.BeforeSendHandler?.Invoke(data, epa.FinalHandlerArgs);

            var r = BinarySerializer.Serialize(data, out var bytes, out var message);
            if (!r)
            {
                Tracker.WriteError($"序列化失败:Type:{typeof(T).FullName},Message:{message}");
                return;
            }
            var len = _udpClient.Send(bytes, bytes.Length, epa.EndPoint);
            builder.AfterSendHandler?.Invoke(bytes, len, DateTime.Now, epa.EndPoint, builder.GenerateArgs);
            Tracker.WriteDebug($"向{epa.EndPoint}【{epa.Name}】发送{len}字节，报文类型为{typeof(TMsg).Name}");
        });
    }

    #endregion 发报机

    #region 收报机

    /// <summary>
    /// 收报行为
    /// <list type="Packet">收发器收到的数据包</list>
    /// <list type="object?">处理数据包行为的入参</list>
    /// </summary>
    private Action<Packet, object?>? ReceiveAction { get; set; }

    /// <summary>
    /// 收报缓存
    /// </summary>
    private readonly ConcurrentQueue<Packet> _cache = new();

    /// <summary>
    /// 设置收报机行为
    /// </summary>
    /// <param name="receiveAction">数据包处理行为</param>
    /// <param name="args">报文处理参数</param>
    public void SetReceiver(Action<Packet, object?> receiveAction, object? args = null)
    {
        ReceiveAction = receiveAction;
        _isReceiverStopped = false;
        Task.Factory.StartNew(() =>
        {
            while (!_isReceiverStopped)
            {
                try
                {
                    if (ReceiveAction == null)
                    {
                        Tracker.WriteWarn($"报文收发器{Name}未设置对报文处理操作");
                        _isReceiverStopped = true;
                        _cache.Clear();
                        return;
                    }

                    if (_cache.TryDequeue(out var packet))
                        ReceiveAction.Invoke(packet, args);

                    Thread.Sleep(1);
                }
                catch (Exception e)
                {
                    Tracker.WriteError($"报文接收器【{Name}】处理报文错误：{e}");
                }
            }
        }, TaskCreationOptions.LongRunning | TaskCreationOptions.PreferFairness);
    }

    //标识收报机是否在工作
    private bool _isReceiverStopped;

    private void Receive(IAsyncResult result)
    {
        try
        {
            if (_udpClient == null) return;
            IPEndPoint? src = null;
            var buf = _udpClient.EndReceive(result, ref src);
            if (_isReceiverStopped) return;
            _cache.Enqueue(new Packet(DateTime.Now, src?.Address.ToString() ?? string.Empty, src?.Port ?? 0, buf));
        }
        catch (Exception ex)
        {
            Tracker.WriteWarn($"【{Name}】接收报文异常：{ex}");
        }
        finally
        {
            if (!_isReceiverStopped)
                _udpClient?.BeginReceive(Receive, null);
        }
    }

    #endregion 收报机
}
