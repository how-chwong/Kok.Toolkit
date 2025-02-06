namespace Kok.Toolkit.Core;

/// <summary>
/// 计算器
/// </summary>
public static class Calculator
{
    /// <summary>
    /// 利用余弦定理计算三个点的夹角
    /// </summary>
    /// <param name="x">圆心坐标X值</param>
    /// <param name="y">圆心坐标Y值</param>
    /// <param name="x1">坐标1X值</param>
    /// <param name="y1">坐标1Y值</param>
    /// <param name="x2">坐标2X值</param>
    /// <param name="y2">坐标2Y值</param>
    /// <returns></returns>
    public static double Angle(double x, double y, double x1, double y1, double x2, double y2)
    {
        var p1X = x1 - x;
        var p1Y = y1 - y;
        var p2X = x2 - x;
        var p2Y = y2 - y;

        var cos = (p1X * p2X + p1Y * p2Y) / (Math.Sqrt(Math.Pow(p1X, 2) + Math.Pow(p1Y, 2)) * Math.Sqrt(Math.Pow(p2X, 2) + Math.Pow(p2Y, 2)));
        var acos = Math.Acos(cos);
        return acos * 180 / Math.PI;
    }

    /// <summary>
    /// 计算两点夹角为45度时的过渡点
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    /// <returns></returns>
    public static (bool result, double x, double y) TransitionPoint45(double x, double y, double x1, double y1,
        double x2, double y2)
        => TransitionPoint(45, x, y, x1, y1, x2, y2);

    /// <summary>
    /// 计算两点夹角为指定角度的过渡点
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    /// <returns></returns>
    public static (bool result, double x, double y) TransitionPoint(double angle, double x, double y, double x1,
        double y1, double x2, double y2)
    {
        var temp = Angle(x, y, x1, y1, x2, y2);
        if (Math.Abs(temp - angle) < 2) return (false, 0, 0);
        var p1Y = y1 - y;
        var p2Y = y2 - y;
        var len = Math.Abs(p2Y - p1Y) / Math.Tan(angle.ToRadianFromAngle());
        var px = x2 - x > 0 ? x + len : x - len;
        var py = y2;
        return (true, px, py);
    }

    /// <summary>
    /// 角度值转换为弧度值
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static double ToRadianFromAngle(this double angle) => angle * Math.PI / 180;

    /// <summary>
    /// 计算坡的高度
    /// </summary>
    /// <param name="slopeValue">坡度值</param>
    /// <param name="bevelLength">斜边长度</param>
    /// <returns></returns>
    public static double GetHeightOfSlope(double slopeValue, double bevelLength)
        => Math.Sin(Math.Atan(slopeValue)) * bevelLength;

    /// <summary>
    /// 根据坡的高度和斜边长度计算坡度值
    /// </summary>
    /// <param name="height"></param>
    /// <param name="bevelLength"></param>
    /// <returns></returns>
    public static double GetSlopValue(double height, double bevelLength)
    {
        if (bevelLength == 0) return 0;
        var angle = Math.Asin(height / bevelLength);
        return Math.Tan(angle);
    }

    /// <summary>
    /// 双字节转ushort
    /// </summary>
    /// <param name="highByte"></param>
    /// <param name="lowByte"></param>
    /// <returns></returns>
    public static ushort ToUShort(byte highByte, byte lowByte)
        => (ushort)((short)(highByte << 8) + lowByte);

    /// <summary>
    /// 四字节转uint
    /// </summary>
    /// <param name="highByte1"></param>
    /// <param name="highByte2"></param>
    /// <param name="lowByte1"></param>
    /// <param name="lowByte2"></param>
    /// <returns></returns>
    public static uint ToUInt(byte highByte1, byte highByte2, byte lowByte1, byte lowByte2)
        => (uint)(int)(((uint)highByte1 << 24) + (int)((uint)highByte2 << 16) + (int)((uint)lowByte1 << 8) + lowByte2);

    /// <summary>
    /// 下一个顺序值
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static byte NextSequenceValue(ref byte value) => (byte)(value % 0xff + 1);

    /// <summary>
    /// 下一个顺序值
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static byte NextSequenceValue(byte value) => (byte)(value % 0xff + 1);

    /// <summary>
    /// 下一个顺序值
    /// </summary>
    /// <param name="value"></param>
    /// <param name="interval">间隔数值</param>
    /// <returns></returns>
    public static uint NextSequenceValue(ref uint value, uint interval)
    {
        if (value == uint.MaxValue || uint.MaxValue - value < interval)
            value = uint.MinValue;
        else
            value += interval;
        return value;
    }

    /// <summary>
    /// 下一个顺序值
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static ushort NextSequenceValue(ref ushort value)
    {
        if (value == ushort.MaxValue) value = ushort.MinValue;
        else value += 1;
        return value;
    }
}
