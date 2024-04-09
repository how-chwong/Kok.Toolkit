namespace Kok.Toolkit.Core.Serialization.Binary.Attributes;

/// <summary>
/// 数值范围
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class NumericalRangeAttribute : Attribute
{
    /// <summary>
    /// 最小值
    /// </summary>
    public int MinValue { get; }

    /// <summary>
    /// 最大值
    /// </summary>
    public int MaxValue { get; }

    /// <summary>
    /// 指明有效数值范围
    /// </summary>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    public NumericalRangeAttribute(int min, int max)
    {
        MinValue = min;
        MaxValue = max;
    }
}