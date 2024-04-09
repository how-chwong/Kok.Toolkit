namespace Kok.Toolkit.Core.Serialization.Binary.Handlers;

/// <summary>
/// 二进制序列化处理器
/// </summary>
public abstract class BinaryBaseHandler : IBinaryHandler
{
    /// <summary>
    /// 序列化器
    /// </summary>
    public BinarySerializer Serializer { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="serializer"></param>
    protected BinaryBaseHandler(BinarySerializer serializer)
    {
        Serializer = serializer;
    }

    /// <inheritdoc cref="IBinaryHandler"/>
    public abstract bool Write(object? value, Type type, PresetSize? presetSize = null);

    /// <inheritdoc cref="IBinaryHandler"/>
    public abstract bool TryRead(Type type, ref object? value, PresetSize? presetSize = null);
}