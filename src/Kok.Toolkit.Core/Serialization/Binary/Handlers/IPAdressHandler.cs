namespace Kok.Toolkit.Core.Serialization.Binary.Handlers;

/// <summary>
/// IPv4地址处理器
/// </summary>
public class IpAddressHandler : BinaryBaseHandler//todo:增加IPv4和IPv6特性，区分字节数
{
    /// <inheritdoc />
    public IpAddressHandler(BinarySerializer serializer) : base(serializer)
    {
    }

    ///<inheritdoc />
    public override bool Write(object? value, Type type, PresetSize? presetSize = null)
    {
        if (type != typeof(IPAddress))
            return false;
        if (value == null)
            throw new Exception("IPAddress实例不能为空");
        var temp = ((IPAddress)value).GetAddressBytes();
        Serializer.Write(temp);
        return true;
    }

    ///<inheritdoc />
    public override bool TryRead(Type type, ref object? value, PresetSize? presetSize = null)
    {
        if (type != typeof(IPAddress))
            return false;
        object? temp = new byte[] { 0, 0, 0, 0 };
        if (!Serializer.TryRead(typeof(byte[]), ref temp, new PresetSize(PresetSizeType.ByteLength, 4)))
            return false;
        value = new IPAddress((byte[])temp!);
        return true;
    }
}