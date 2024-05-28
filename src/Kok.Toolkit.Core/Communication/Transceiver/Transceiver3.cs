﻿namespace Kok.Toolkit.Core.Communication.Transceiver;

/// <summary>
/// 收发器
/// </summary>
/// <typeparam name="T1">报文类型1</typeparam>
/// <typeparam name="T2">报文类型2</typeparam>
/// <typeparam name="T3">报文类型3</typeparam>
public class Transceiver<T1, T2, T3> : Transceiver<T1, T2>
    where T1 : class, new()
    where T2 : class, new()
    where T3 : class, new()
{
    #region 启动停止

    /// <inheritdoc />
    public override bool Start(string localIp, int localPort, string name = "")
    {
        if (TransmitterBuilder3?.IsAvailable == true)
            _timer3 = new AntiReTimer(
                TransmitterBuilder3.ChangedJudges!,
                SendWork,
                TransmitterBuilder3,
                5000,
                TransmitterBuilder3.Interval,
                TransmitterBuilder3?.Type == TransmitterType.FixedCycle ? TransmitterBuilder3.PeriodCount : 0);
        return base.Start(localIp, localPort, name);
    }

    /// <inheritdoc />
    public override void Stop()
    {
        _timer3?.Stop();
        base.Stop();
    }

    #endregion 启动停止

    #region 发报机

    /// <summary>
    /// 报文类型3的发报机构建器
    /// </summary>
    protected TransmitterBuilder<T3>? TransmitterBuilder3;

    /// <summary>
    /// 报文类型3的定时器
    /// </summary>
    private AntiReTimer? _timer3;

    /// <summary>
    /// 设置发报机
    /// </summary>
    /// <param name="builder1">报文类型1的发报机</param>
    /// <param name="builder2">报文类型2的发报机</param>
    /// <param name="builder3">报文类型3的发报机</param>
    public void SetTransmitter(TransmitterBuilder<T1> builder1, TransmitterBuilder<T2> builder2, TransmitterBuilder<T3> builder3)
    {
        TransmitterBuilder = builder1;
        TransmitterBuilder2 = builder2;
        TransmitterBuilder3 = builder3;
    }

    /// <summary>
    /// 设置发报机
    /// </summary>
    /// <param name="builder">发报机构建器</param>
    public void SetTransmitter(TransmitterBuilder<T3> builder)
    {
        TransmitterBuilder3 = builder;
    }

    /// <inheritdoc />
    protected override void SendWork(object? transmitterBuilder)
    {
        base.SendWork(transmitterBuilder);
        if (transmitterBuilder is TransmitterBuilder<T3> builder)
            MessageSender(builder);
    }

    #endregion 发报机
}
