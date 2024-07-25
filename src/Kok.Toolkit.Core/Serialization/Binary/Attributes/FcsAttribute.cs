namespace Kok.Toolkit.Core.Serialization.Binary.Attributes;

/// <summary>
/// 帧校验序列特性
/// </summary>
public class FcsAttribute : Attribute
{
    /// <summary>
    /// FCS计算算法名称
    /// </summary>
    public string Algorithm { get; }

    /// <summary>
    /// 构造实例
    /// </summary>
    /// <param name="algorithm"></param>
    public FcsAttribute(string algorithm)
    {
        Algorithm = algorithm;
    }
}
