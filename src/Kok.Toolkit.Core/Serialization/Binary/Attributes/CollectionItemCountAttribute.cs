namespace Kok.Toolkit.Core.Serialization.Binary.Attributes;

/// <summary>
/// 集合子项的数量
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class CollectionItemCountAttribute : Attribute
{
    /// <summary>
    /// 子项的数量
    /// </summary>
    public int Value { get; }

    /// <summary>
    /// 获取子项数量的路径
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// 构造属性实例
    /// </summary>
    /// <param name="value"></param>
    public CollectionItemCountAttribute(int value)
    {
        Value = value;
        Path = string.Empty;
    }

    /// <summary>
    /// 构造属性实例
    /// </summary>
    /// <param name="path"></param>
    public CollectionItemCountAttribute(string path)
    {
        Path = path;
        Value = -1;
    }
}