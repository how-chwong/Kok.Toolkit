namespace Kok.Toolkit.Core.Serialization.Binary.Attributes;

/// <summary>
/// 指定编码格式
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class BinaryEncodingAttribute : Attribute
{
    /// <summary>
    /// 编码类型
    /// </summary>
    public Encoding Encoding { get; }

    /// <summary>
    /// 是否小端编码
    /// </summary>
    public bool IsLittleEndian { get; }

    /// <summary>
    /// 构造二进制编码特性
    /// </summary>
    /// <param name="encoding"></param>
    /// <param name="isLittleEndian"></param>
    public BinaryEncodingAttribute(Encoding encoding, bool isLittleEndian)
    {
        Encoding = encoding;
        IsLittleEndian = isLittleEndian;
    }

    /// <summary>
    /// 构造二进制编码特性
    /// </summary>
    public BinaryEncodingAttribute()
    {
        Encoding = Encoding.UTF8;
        IsLittleEndian = false;
    }
}