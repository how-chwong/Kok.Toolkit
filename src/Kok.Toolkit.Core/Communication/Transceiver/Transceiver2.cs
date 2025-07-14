using Kok.Toolkit.Core.Timers;

namespace Kok.Toolkit.Core.Communication.Transceiver;

/// <summary>
/// 收发器
/// </summary>
/// <typeparam name="T1">报文类型1</typeparam>
/// <typeparam name="T2">报文类型2</typeparam>
public class Transceiver<T1, T2> : Transceiver<T1> where T1 : class, new() where T2 : class, new()
{
    #region 启动停止

    /// <inheritdoc />
    public override bool Start(string localIp, int localPort, string name = "")
    {
        BuildTimer(TransmitterBuilder2);
        return base.Start(localIp, localPort, name);
    }

    /// <summary>
    /// 构建定时器
    /// </summary>
    /// <param name="builder"></param>
    private void BuildTimer(TransmitterBuilder<T2>? builder)
    {
        if (builder?.IsAvailable != true) return;

        _timer2 = TimerType switch
        {
            TimerType.Multimedia => new MultimediaTimer(SendWork, builder, builder.Interval),

            _ => new AntiReTimer(builder.ChangedJudges, SendWork, builder, builder.Interval,
                builder.Type == TransmitterType.FixedCycle ? builder.PeriodCount : 0)
        };
    }

    /// <inheritdoc />
    public override void Stop()
    {
        _timer2?.Stop();
        base.Stop();
    }

    #endregion 启动停止

    #region 发报机

    /// <summary>
    /// 报文类型2的发报机构建器
    /// </summary>
    protected TransmitterBuilder<T2>? TransmitterBuilder2;

    /// <summary>
    /// 报文类型2的定时器
    /// </summary>
    private ITimer? _timer2;

    /// <summary>
    /// 设置发报机
    /// </summary>
    /// <param name="builder1">报文类型1的发报机构建器</param>
    /// <param name="builder2">报文类型2的发报机构建器</param>
    public void SetTransmitter(TransmitterBuilder<T1> builder1, TransmitterBuilder<T2> builder2)
    {
        TransmitterBuilder = builder1;
        TransmitterBuilder2 = builder2;
    }

    /// <summary>
    /// 设置发报机
    /// </summary>
    /// <param name="builder">发报机构建器</param>
    public void SetTransmitter(TransmitterBuilder<T2> builder)
    {
        TransmitterBuilder2 = builder;
    }

    /// <inheritdoc />
    protected override void SendWork(object? transmitterBuilder)
    {
        base.SendWork(transmitterBuilder);
        if (transmitterBuilder is TransmitterBuilder<T2> builder)
            MessageSender(builder);
    }

    #endregion 发报机
}
