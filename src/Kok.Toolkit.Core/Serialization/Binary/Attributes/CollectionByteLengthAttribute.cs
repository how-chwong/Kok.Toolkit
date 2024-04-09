namespace Kok.Toolkit.Core.Serialization.Binary.Attributes;

/// <summary>
/// 集合所占字节大小
/// 通过该特性标识属性所占字节大小或标识保存其字节大小值的属性名称
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class CollectionByteLengthAttribute : Attribute
{
    /// <summary>
    /// 字节长度
    /// </summary>
    public int ByteLength { get; }

    /// <summary>
    /// 获取字节大小的路径
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// 用指定长度构造字节长度特性
    /// </summary>
    /// <param name="length"></param>
    public CollectionByteLengthAttribute(int length)
    {
        ByteLength = length;
        Path = string.Empty;
    }

    /// <summary>
    /// 用指定属性名称构造字节长度特性
    /// </summary>
    /// <param name="path"></param>
    public CollectionByteLengthAttribute(string path)
    {
        Path = path;
        ByteLength = 0;
    }
}