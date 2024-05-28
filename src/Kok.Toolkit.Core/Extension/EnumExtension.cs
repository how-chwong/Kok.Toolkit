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

    /// <summary>
    /// 给具有Flags特性的枚举赋值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="targetValue"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static T SetFlag<T>(this T value, T targetValue) where T : Enum
    {
        if (value.GetType() != targetValue.GetType())
            throw new Exception("目标值必须与源类型相同");
        return (T)Enum.ToObject(value.GetType(), Convert.ToUInt64(value) | Convert.ToUInt64(targetValue));
    }

    /// <summary>
    /// 取消具有Flags特性的枚举值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="targetValue"></param>
    /// <returns></returns>
    public static T ClearFlag<T>(this T value, T targetValue) where T : Enum
    {
        if (value.GetType() != targetValue.GetType())
            throw new Exception("目标值必须与源类型相同");
        return (T)Enum.ToObject(value.GetType(), Convert.ToUInt64(value) & ~Convert.ToUInt64(targetValue));
    }

    /// <summary>
    /// 判断指定Flags枚举是否已设置指定值
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetValue"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static bool AnyFlag(this Enum value, Enum targetValue)
    {
        if (value.GetType() != targetValue.GetType())
            throw new Exception("目标值必须与源类型相同");
        return (Convert.ToUInt64(value) & Convert.ToUInt64(targetValue)) != 0;
    }
}