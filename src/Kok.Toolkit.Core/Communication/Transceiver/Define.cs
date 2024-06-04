namespace Kok.Toolkit.Core.Communication.Transceiver;

/// <summary>
/// 发报机类型
/// </summary>
public enum TransmitterType
{
    /// <summary>
    /// 周期性的循环发送
    /// </summary>
    Cyclical,

    /// <summary>
    /// 仅发送固定周期数
    /// </summary>
    FixedCycle
}

/// <summary>
/// 收发器对数据包的封装
/// </summary>
/// <param name="ReceivedTime">接收到的时间</param>
/// <param name="SourceAddress">数据包的来源地址，或来源IP</param>
/// <param name="SourcePort">数据包的来源端口</param>
/// <param name="Data">数据包内容</param>
public record Packet(DateTime ReceivedTime, string SourceAddress, int SourcePort, byte[] Data)
{
    /// <summary>
    /// 数据长度
    /// </summary>
    public int Size => Data.Length;

    /// <summary>
    /// 截取部分数据作为切片
    /// </summary>
    /// <param name="start">截取起始索引</param>
    /// <param name="length">截取长度</param>
    /// <returns></returns>
    public byte[] Slice(int start, int length)
    {
        if (start > Size) return Array.Empty<byte>();
        if (start + length > Size) return Array.Empty<byte>();
        var temp = new byte[length];
        Buffer.BlockCopy(Data, start, temp, 0, length);
        return temp;
    }

    /// <summary>
    /// 将数据转为字符串
    /// </summary>
    /// <returns></returns>
    public override string ToString() => BitConverter.ToString(Data).Replace("-", " ");
}

/// <summary>
/// 报文发送目标
/// </summary>
/// <param name="Name">目标名称</param>
/// <param name="EndPoint">目标终结点</param>
/// <param name="FinalHandlerArgs">发送给目标前，报文处理函数的入参</param>
public record TargetEndPoint(string Name, IPEndPoint EndPoint, object[]? FinalHandlerArgs);
