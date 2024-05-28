using Kok.Toolkit.Core.Extension;

namespace Kok.Toolkit.Core.Serialization.Binary.Attributes;

/// <summary>
/// 集合切片数量特性
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class SlicesNumberAttribute : Attribute
{
    /// <summary>
    /// 切片数量关联的属性路径
    /// </summary>
    public string[] Path { get; } = Array.Empty<string>();

    /// <summary>
    /// 构造一个集合切片数量特性
    /// </summary>
    /// <param name="path"></param>
    public SlicesNumberAttribute(params string[] path)
    {
        if (path.IsEmpty())
            return;
        Path = new string[path.Length];
        for (var i = 0; i < path.Length; i++)
            Path[i] = path[i];
    }
}
