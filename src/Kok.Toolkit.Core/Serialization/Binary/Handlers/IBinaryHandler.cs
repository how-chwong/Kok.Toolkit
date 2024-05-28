namespace Kok.Toolkit.Core.Serialization.Binary.Handlers;

/// <summary>
/// 二进制序列化处理器
/// </summary>
public interface IBinaryHandler
{
    /// <summary>
    /// 处理器所属的序列化器
    /// </summary>
    BinarySerializer Serializer { get; }

    /// <summary>
    /// 写入对象
    /// </summary>
    /// <param name="value">对象值</param>
    /// <param name="type">对象类型</param>
    /// <param name="presetSize">预设对象大小</param>
    /// <returns></returns>
    bool Write(object? value, Type type, PresetSize? presetSize = null);

    /// <summary>
    /// 尝试读取指定类型的对象
    /// </summary>
    /// <param name="type">对象类型</param>
    /// <param name="value">对象值</param>
    /// <param name="presetSize">预设对象大小</param>
    /// <returns></returns>
    bool TryRead(Type type, ref object? value, PresetSize? presetSize = null);
}

/// <summary>
/// 成员预设大小信息
/// </summary>
public class PresetSize
{
    /// <summary>
    /// 预设大小类型
    /// </summary>
    public PresetSizeType Type { get; }

    /// <summary>
    /// 预设值
    /// </summary>
    public int Value { get; }

    /// <summary>
    /// 构造预设大小实例
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    public PresetSize(PresetSizeType type, int value)
    {
        Type = type;
        Value = value;
        if (value == -1)
            Type = PresetSizeType.None;
    }

    /// <summary>
    /// 判断是否有预设值
    /// </summary>
    /// <returns></returns>
    public bool HasPresetSize() => Type != PresetSizeType.None && Value > 0;
}

/// <summary>
/// 预设大小类型
/// </summary>
public enum PresetSizeType
{
    /// <summary>
    /// 无预设大小
    /// </summary>
    None,

    /// <summary>
    /// 字节大小
    /// </summary>
    ByteLength,

    /// <summary>
    /// 子项目数量
    /// </summary>
    SubItemCount
}
