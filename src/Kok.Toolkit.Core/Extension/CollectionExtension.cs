namespace Kok.Toolkit.Core.Extension;

/// <summary>
/// 集合扩展
/// </summary>
public static class CollectionExtension
{
    /// <summary>
    /// 按指定拼接符号连接所有子项转为字符串
    /// </summary>
    /// <typeparam name="T">集合子项类型</typeparam>
    /// <param name="values">待转为字符串的集合</param>
    /// <param name="separator">连接符</param>
    /// <param name="whenNull">当集合为空时，输出的字符串内容</param>
    /// <param name="formatString"></param>
    /// <returns></returns>
    public static string ToString<T>(this IEnumerable<T>? values, string separator = " ", string whenNull = "", string formatString = "")
    {
        if (values == null)
            return whenNull;
        var array = values.ToArray();
        if (string.IsNullOrWhiteSpace(formatString))
            return array.Any() ? string.Join(separator, array) : whenNull;
        return array.Any() ? string.Join(separator, array.Select(v => string.Format(formatString, v))) : whenNull;
    }

    /// <summary>
    /// 字符串数组转为byte数组
    /// </summary>
    /// <param name="values"></param>
    /// <param name="isHex"></param>
    /// <returns></returns>
    public static byte[] ToByteArray(this string[]? values, bool isHex = true)
    {
        if (values == null || values.Length == 0)
            return Array.Empty<byte>();
        var result = new List<byte>(values.Length);
        try
        {
            result.AddRange(isHex
                ? values.Select(v => Convert.ToByte(v, 16))
                : values.Select(str => !byte.TryParse(str, out var v) ? (byte)0 : v));
        }
        catch (Exception)
        {
            return Array.Empty<byte>();
        }
        return result.ToArray();
    }

    /// <summary>
    /// 字符串集合转为整数集合
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public static int[] ToIntArray(this IEnumerable<string> values)
    {
        var array = values as string[] ?? Array.Empty<string>();
        if (!array.Any())
            return Array.Empty<int>();
        var data = new List<int>(array.Length);
        foreach (var str in array)
        {
            if (!int.TryParse(str, out var v))
                continue;
            data.Add(v);
        }
        return data.ToArray();
    }

    /// <summary>
    /// 判断集合是否为空
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool IsEmpty(this IList? values) =>
        values == null || values.Count == 0;

    /// <summary>
    /// 判断是否与目标集合的子项完全相同
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="src">源集合</param>
    /// <param name="target">待比较集合</param>
    /// <returns></returns>
    public static bool HasSameItems<T>(this List<T> src, List<T> target)
    {
        if (src.IsEmpty() && target.IsEmpty())
            return true;
        if (src.IsEmpty() || target.IsEmpty())
            return false;
        return src.Count == target.Count && src.All(target.Contains);
    }

    /// <summary>
    /// 根据key获取字典内对应的value
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dictionary"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static TValue? GetValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key) where TKey : notnull
        => dictionary.TryGetValue(key, out var value) ? value : default;

    /// <summary>
    /// 将当前数组复制一份
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static byte[] Copy(this byte[] value)
    {
        if (value.Length == 0)
            return Array.Empty<byte>();
        var data = new byte[value.Length];
        Buffer.BlockCopy(value, 0, data, 0, value.Length);
        return data;
    }

    /// <summary>
    /// 复制当前数组的指定部分
    /// </summary>
    /// <param name="value"></param>
    /// <param name="start"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static byte[] Copy(this byte[] value, int start, int length)
    {
        if (value.Length == 0)
            return Array.Empty<byte>();
        var data = new byte[length];
        if (start < 0 || start >= value.Length)
            return data;
        var count = start + length > value.Length ? value.Length - start : length;
        Buffer.BlockCopy(value, start, data, 0, count);
        return data;
    }

    /// <summary>
    /// 将字节数组按大端方式转为无符号整数
    /// </summary>
    /// <param name="data"></param>
    /// <param name="start"></param>
    /// <param name="length"></param>
    /// <param name="isBig"></param>
    /// <returns></returns>
    public static ulong ToUnSignedInteger(this ReadOnlySpan<byte> data, int start, int length, bool isBig = true)
    {
        ulong temp = 0;
        for (var i = 0; i < length && i + start < data.Length; i++)
        {
            var offset = (isBig ? (length - i - 1) : i) * 8;
            ulong d = data[i + start];
            temp |= (d & 0xff) << offset;
        }
        return temp;
    }

    /// <summary>
    /// 将字节数组按大端方式转为无符号整数
    /// </summary>
    /// <param name="data"></param>
    /// <param name="start"></param>
    /// <param name="length"></param>
    /// <param name="isBig"></param>
    /// <returns></returns>
    public static long ToSignedInteger(this ReadOnlySpan<byte> data, int start, int length, bool isBig = true)
    {
        var temp = data.ToUnSignedInteger(start, length, isBig);
        return length switch
        {
            1 => (sbyte)temp,
            2 => (short)temp,
            4 => (int)temp,
            _ => (long)temp
        };
    }

    /// <summary>
    /// 获取指定字节数组的切片，若越界则返回空
    /// </summary>
    /// <param name="data"></param>
    /// <param name="start"></param>
    /// <returns></returns>
    public static ReadOnlySpan<byte> TrySlice(this ReadOnlySpan<byte> data, int start)
        => start >= data.Length ? Array.Empty<byte>() : data[start..];
}