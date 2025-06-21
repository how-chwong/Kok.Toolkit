using System.ComponentModel;

namespace Kok.Toolkit.Core.Extension;

/// <summary>
/// 枚举类型扩展
/// </summary>
public static class EnumExtension
{
    /// <summary>
    /// 获取枚举值描述
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string GetDescription(this Enum value)
    {
        var type = value.GetType();
        var valueArray = value.ToString().TrimEmpty().Split(',');
        var sb = new StringBuilder();
        foreach (var valueStr in valueArray)
        {
            var field = type.GetField(valueStr);
            if (field == null || !field.IsDefined(typeof(DescriptionAttribute), true))
                return value.ToString();
            var attr = field.GetCustomAttributes(typeof(DescriptionAttribute));
            sb.Append((attr.FirstOrDefault() as DescriptionAttribute)?.Description ?? valueStr);
            sb.Append(',');
        }

        return sb.ToString(0, sb.ToString().Length - 1);
    }

    /// <summary>
    /// 获取枚举的描述
    /// </summary>
    /// <param name="enumType"></param>
    /// <returns></returns>
    public static Array GetEnumDescriptions(Type enumType)
        => Enum.GetValues(enumType).Cast<object>().Select(val => ((Enum)val).GetDescription()).ToArray();
}
