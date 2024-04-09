namespace Kok.Toolkit.Core.Extension;

/// <summary>
/// DateTime扩展
/// </summary>
public static class DateTimeExtension
{
    /// <summary>
    /// 从DateTime构造10位时间戳（单位：秒)
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static ulong ToTimeStamp(this DateTime dateTime)
        => (ulong)dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;

    /// <summary>
    /// 转换为13位时间戳（单位：毫秒)
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static ulong ToLongTimeStamp(this DateTime dateTime)
        => (ulong)dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;

    /// <summary>
    /// 将本地时间转换为10位时间戳（单位：秒)
    /// 以本地时间为基准
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static ulong ToLocalTimeStamp(this DateTime dateTime)
        => (ulong)dateTime.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local)).TotalSeconds;

    /// <summary>
    /// 将本地时间转换为13位时间戳（单位：毫秒)
    /// 以本地时间为基准
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static ulong ToLocalLongTimeStamp(this DateTime dateTime)
        => (ulong)dateTime.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local)).TotalMilliseconds;
}

/// <summary>
/// 时间戳
/// </summary>
public static class TimeStampUtil
{
    /// <summary>
    /// 10位时间戳（单位：秒)转换为本地时间
    /// </summary>
    /// <param name="timeStamp"></param>
    /// <returns></returns>
    public static DateTime TimeStampToLocalTime(ulong timeStamp)
        => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timeStamp).ToLocalTime();

    /// <summary>
    /// 13位时间戳（单位：毫秒)转换为本地时间
    /// </summary>
    /// <param name="timeStamp"></param>
    /// <returns></returns>
    public static DateTime LongTimeStampToLocalTime(ulong timeStamp)
        => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(timeStamp).ToLocalTime();

    /// <summary>
    /// 10位时间戳（单位：秒)转换为本地时间
    /// </summary>
    /// <param name="timeStamp"></param>
    /// <returns></returns>
    public static DateTime LocalTimeStampToLocalTime(ulong timeStamp)
        => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local).AddSeconds(timeStamp).ToLocalTime();

    /// <summary>
    /// 13位时间戳（单位：毫秒)转换为本地时间
    /// </summary>
    /// <param name="timeStamp"></param>
    /// <returns></returns>
    public static DateTime LocalLongTimeStampToLocalTime(ulong timeStamp)
        => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local).AddMilliseconds(timeStamp).ToLocalTime();
}