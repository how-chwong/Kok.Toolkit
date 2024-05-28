namespace Kok.Toolkit.Core.Serialization.Binary.Attributes;

/// <summary>
/// 字段顺序
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class FieldOrderAttribute : Attribute
{
    /// <summary>
    /// 字节循序
    /// </summary>
    public int Order { get; }

    /// <summary>
    /// 构造字节顺序特性
    /// </summary>
    /// <param name="order"></param>
    public FieldOrderAttribute(int order)
    {
        Order = order;
    }
}