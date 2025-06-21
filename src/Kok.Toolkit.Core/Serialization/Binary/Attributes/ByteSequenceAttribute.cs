namespace Kok.Toolkit.Core.Serialization.Binary.Attributes;

/// <summary>
/// 字节顺序
/// 仅显示功能
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ByteSequenceAttribute : Attribute
{
    /// <summary>
    /// 顺序值
    /// </summary>
    public int Sequence { get; }

    /// <summary>
    /// 字节长度
    /// </summary>
    public int Length { get; }

    /// <summary>
    /// 构造特性
    /// </summary>
    /// <param name="sequence"></param>
    /// <param name="length"></param>
    public ByteSequenceAttribute(int sequence, int length = 1)
    {
        Sequence = sequence;
        Length = length;
    }
}

/// <summary>
/// 位顺序特性
/// 仅显示功能
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class BitSequenceAttribute : Attribute
{
    /// <summary>
    /// 顺序值
    /// </summary>
    public int Sequence { get; }

    /// <summary>
    /// 位长度
    /// </summary>
    public byte Length { get; }

    /// <summary>
    /// 构造特性
    /// </summary>
    /// <param name="sequence"></param>
    /// <param name="length"></param>
    public BitSequenceAttribute(int sequence, byte length = 1)
    {
        Sequence = sequence;
        Length = length;
    }
}

/// <summary>
/// 位开始值
/// 仅显示功能
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class BitStartSequenceAttribute : Attribute
{
    /// <summary>
    /// 顺序值
    /// </summary>
    public int Sequence { get; }

    /// <summary>
    /// 构造特性
    /// </summary>
    /// <param name="sequence"></param>
    public BitStartSequenceAttribute(int sequence)
    {
        Sequence = sequence;
    }
}

/// <summary>
/// 协议字段名称
/// 仅显示功能
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class AgreementNameAttribute : Attribute
{
    /// <summary>
    /// 字段名称
    /// </summary>
    public string FieldName { get; }

    /// <summary>
    /// 构造特性
    /// </summary>
    /// <param name="fieldName"></param>
    public AgreementNameAttribute(string fieldName)
    {
        FieldName = fieldName;
    }
}
