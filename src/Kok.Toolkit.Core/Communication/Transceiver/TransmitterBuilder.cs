using Kok.Toolkit.Core.Extension;

namespace Kok.Toolkit.Core.Communication.Transceiver;

/// <summary>
/// 发报机构建器
/// </summary>
/// <typeparam name="T">报文对应的数据结构</typeparam>
public sealed class TransmitterBuilder<T>
{
    /// <summary>
    /// 目标地址集合
    /// </summary>
    public List<TargetEndPoint> TargetEndPoints { get; private init; } = new();

    /// <summary>
    /// 发报类型
    /// </summary>
    public TransmitterType Type { get; private init; } = TransmitterType.Cyclical;

    /// <summary>
    /// 发报间隔毫秒数
    /// </summary>
    public int Interval { get; private init; } = 1000;

    /// <summary>
    /// 发报周期数
    /// </summary>
    public int PeriodCount { get; private init; } = 1;

    /// <summary>
    /// 报文生成行为的参数
    /// </summary>
    public object? GenerateArgs { get; private init; }

    /// <summary>
    /// 报文生成器
    /// </summary>
    public Func<object?, List<T>>? GenerateAction { get; private init; }

    /// <summary>
    /// 报文变更裁判
    /// </summary>
    public Func<bool>? ChangedJudges { get; private init; }

    /// <summary>
    /// 报文发送前的最后处理
    /// </summary>
    public Action<List<T>, object[]?>? FinalHandler { get; private init; }

    /// <summary>
    /// 发送完成后的处理行为
    /// <list type="IReadOnlyCollection">报文被序列化后的字节集合</list>
    /// <list type="int">发报机实际发出的字节数</list>
    /// <list type="DateTime">发报机发报时间</list>
    /// <list type="EndPoint">发报机发报的目的地</list>
    /// <list type="object?">发报机生成报文使用的参数</list>
    /// </summary>
    public Action<IReadOnlyCollection<byte>, int, DateTime, EndPoint, object?>? AfterSendHandler { get; private init; }

    /// <summary>
    /// 标识构建器是否可用
    /// </summary>
    public bool IsAvailable
    {
        get => !TargetEndPoints.IsEmpty() && GenerateAction != null;
    }

    public static TransmitterBuilder<TMessage>? Create<TMessage>(
        List<TargetEndPoint> targets,
        int interval,
        Func<object?, List<TMessage>> generateAction,
        object? generateArgs,
        Action<List<TMessage>, object[]?>? finalHandler = null,
        Func<bool>? judges = null,
        int cycleCount = 0,
        Action<IReadOnlyCollection<byte>, int, DateTime, EndPoint, object?>? afterSendHandler = null)
    {
        if (targets.IsEmpty()) return null;
        var builder = new TransmitterBuilder<TMessage>
        {
            ChangedJudges = judges,
            GenerateAction = generateAction,
            GenerateArgs = generateArgs,
            FinalHandler = finalHandler,
            Interval = interval,
            PeriodCount = cycleCount,
            Type = cycleCount == 0 ? TransmitterType.Cyclical : TransmitterType.FixedCycle,
            TargetEndPoints = new List<TargetEndPoint>(targets.Count),
            AfterSendHandler = afterSendHandler
        };
        targets.ForEach(t => builder.TargetEndPoints.Add(t));
        return builder;
    }
}
