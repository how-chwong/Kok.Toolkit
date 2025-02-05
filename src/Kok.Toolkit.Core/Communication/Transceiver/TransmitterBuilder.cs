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
    public Func<object?, T?>? GenerateAction { get; private init; }

    /// <summary>
    /// 报文变更裁判
    /// </summary>
    public Func<bool>? ChangedJudges { get; private init; }

    /// <summary>
    /// 报文发送前的最后处理
    /// </summary>
    public Action<T?, object[]?>? BeforeSendHandler { get; private init; }

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

    /// <summary>
    /// 创建一个周期性不间断发送的发报机构建器
    /// </summary>
    /// <typeparam name="TMessage">发报机发送的报文类型</typeparam>
    /// <param name="targets">发送报文的目的地</param>
    /// <param name="interval">周期发送时的发报间隔，单位毫秒</param>
    /// <param name="generateAction">报文生成函数</param>
    /// <param name="generateArgs">报文生成的入参</param>
    /// <param name="beforeSend">报文发送前的处理函数</param>
    /// <param name="afterSend">报文发送后的处理函数</param>
    /// <returns></returns>
    public static TransmitterBuilder<TMessage> CreateCyclical<TMessage>(
        List<TargetEndPoint> targets,
        int interval,
        Func<object?, TMessage?> generateAction,
        object? generateArgs = null,
        Action<TMessage?, object[]?>? beforeSend = null,
        Action<IReadOnlyCollection<byte>, int, DateTime, EndPoint, object?>? afterSend = null)
    {
        if (targets.IsEmpty()) throw new ArgumentException("目标地址不能为空", nameof(targets));
        var builder = new TransmitterBuilder<TMessage>
        {
            GenerateAction = generateAction,
            GenerateArgs = generateArgs,
            BeforeSendHandler = beforeSend,
            Interval = interval,
            Type = TransmitterType.Cyclical,
            TargetEndPoints = new List<TargetEndPoint>(targets.Count),
            AfterSendHandler = afterSend
        };
        targets.ForEach(t => builder.TargetEndPoints.Add(t));
        return builder;
    }

    /// <summary>
    /// 创建一个仅发送固定几个周期的发报机构建器
    /// </summary>
    /// <typeparam name="TMessage">发报机发送的报文类型</typeparam>
    /// <param name="targets">发送报文的目的地</param>
    /// <param name="interval">周期发送时的发报间隔，单位毫秒</param>
    /// <param name="generateAction">报文生成函数</param>
    /// <param name="generateArgs">报文生成的入参</param>
    /// <param name="beforeSend">报文发送前的处理函数</param>
    /// <param name="judges">判定报文是否发送变化的函数</param>
    /// <param name="cycleCount">非周期发送时，报文的发送次数</param>
    /// <param name="afterSend">报文发送后的处理函数</param>
    /// <returns></returns>
    public static TransmitterBuilder<TMessage> CreateFixedCycle<TMessage>(
        int cycleCount,
        List<TargetEndPoint> targets,
        int interval,
        Func<object?, TMessage?> generateAction,
        object? generateArgs = null,
        Action<TMessage?, object[]?>? beforeSend = null,
        Func<bool>? judges = null,
        Action<IReadOnlyCollection<byte>, int, DateTime, EndPoint, object?>? afterSend = null)
    {
        if (targets.IsEmpty()) throw new ArgumentException("目标地址不能为空", nameof(targets));
        if (cycleCount <= 0) throw new ArgumentException("固定周期数量的值不能为0", nameof(cycleCount));
        if (judges == null) throw new ArgumentNullException(nameof(judges), "判定报文是否发生变化的方法不能为空");
        var builder = new TransmitterBuilder<TMessage>
        {
            ChangedJudges = judges,
            GenerateAction = generateAction,
            GenerateArgs = generateArgs,
            BeforeSendHandler = beforeSend,
            Interval = interval,
            PeriodCount = cycleCount,
            Type = TransmitterType.FixedCycle,
            TargetEndPoints = new List<TargetEndPoint>(targets.Count),
            AfterSendHandler = afterSend
        };
        targets.ForEach(t => builder.TargetEndPoints.Add(t));
        return builder;
    }
}
