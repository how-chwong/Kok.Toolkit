using System.Globalization;
using System.Text.RegularExpressions;

namespace Kok.Toolkit.Core.Extension;

/// <summary>
/// 字符串扩展
/// </summary>
public static class StringExtension
{
    /// <summary>
    /// 判断字符串是否为空
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsEmpty(this string value) => string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// 转换为整数
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int ToInt(this string value)
        => !string.IsNullOrEmpty(value) && int.TryParse(value, out var v) ? v : 0;

    /// <summary>
    /// 转换为无符号整数
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static uint ToUint(this string value)
        => !string.IsNullOrWhiteSpace(value) && uint.TryParse(value, out var v) ? v : 0;

    /// <summary>
    /// 去除空白
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string TrimEmpty(this string value)
        => Regex.Replace(value, @"\s+", string.Empty);

    /// <summary>
    /// 16进制字符串转换为byte[]
    /// </summary>
    /// <param name="value"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static bool TryToHexArray(this string value, out byte[] data)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            data = Array.Empty<byte>();
            return true;
        }
        data = new byte[value.Length / 2];
        for (var i = 0; i < data.Length; i++)
        {
            try
            {
                data[i] = (byte)Convert.ToInt32(value.Substring(i * 2, 2), 16);
            }
            catch (Exception)
            {
                data = Array.Empty<byte>();
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 将指定的字符串从源字符串尾部移除
    /// </summary>
    /// <param name="value">源字符串</param>
    /// <param name="ends">待移除字符串</param>
    /// <returns></returns>
    public static string TrimEnd(this string value, params string[] ends)
    {
        if (string.IsNullOrEmpty(value))
            return value;
        if (ends.IsEmpty() || string.IsNullOrEmpty(ends[0]))
            return value;

        for (var i = 0; i < ends.Length; i++)
        {
            if (!value.EndsWith(ends[i], StringComparison.OrdinalIgnoreCase))
                continue;

            value = value[..^ends[i].Length];
            if (string.IsNullOrEmpty(value))
                break;
            i = -1;
        }
        return value;
    }

    /// <summary>
    /// 将字符串转换为指定的类型值
    /// </summary>
    /// <param name="value"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static object ChangeType(this string value, Type type)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;
        if (type == typeof(string))
            return value;
        if (type.IsEnum)
            return Enum.Parse(type, value, true);
        return Type.GetTypeCode(type) switch
        {
            TypeCode.Boolean => Convert.ToBoolean(value),
            TypeCode.Byte => Convert.ToByte(value),
            TypeCode.Int16 => Convert.ToInt16(value),
            TypeCode.UInt16 => Convert.ToUInt16(value),
            TypeCode.Int32 => Convert.ToInt32(value),
            TypeCode.UInt32 => Convert.ToUInt32(value),
            TypeCode.Int64 => Convert.ToInt64(value),
            TypeCode.UInt64 => Convert.ToUInt64(value),
            TypeCode.Single => Convert.ToSingle(value),
            _ => throw new ArgumentException($"不支持的转换类型    type:{type.Name}")
        };
    }

    /// <summary>
    /// 转换格式为yyyy-MM-dd HH:mm:ss.fff
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static string ToNormalString(this DateTime time) => time.ToString("yyyy-MM-dd HH:mm:ss.fff");

    /// <summary>
    /// 尝试将十六进制字符串转换为整数
    /// </summary>
    /// <param name="hexStr"></param>
    /// <returns></returns>
    public static (bool result, uint value) TryConvertHexStringToUint(string hexStr) => uint.TryParse(hexStr, NumberStyles.HexNumber, null, out var data) ? (true, data) : ((bool result, uint value))(false, 0);

    /// <summary>
    /// 尝试将二进制字符串转为无符号整数
    /// </summary>
    /// <param name="binStr"></param>
    /// <returns></returns>
    public static (bool result, ulong value) TryConvertBinaryStringToUlong(string binStr)
    {
        try
        {
            var value = Convert.ToUInt64(binStr, 2);
            return (true, value);
        }
        catch (Exception)
        {
            return (false, 0);
        }
    }

    /// <summary>
    /// 尝试将二进制字符串转为有符号整数
    /// </summary>
    /// <param name="binStr"></param>
    /// <returns></returns>
    public static (bool result, long value) TryConvertBinaryStringToLong(string binStr)
    {
        try
        {
            var value = Convert.ToInt64(binStr, 2);
            return (true, value);
        }
        catch (Exception)
        {
            return (false, 0);
        }
    }

    /// <summary>
    /// 将指定字符串加入StringBuilder并在其后增加分隔符
    /// </summary>
    /// <param name="sb"></param>
    /// <param name="value"></param>
    /// <param name="separator"></param>
    public static void AppendWithSeparator(this StringBuilder sb, string value, string separator = ",")
    {
        sb.Append(value);
        sb.Append(separator);
    }

    /// <summary>
    /// 将文本字符串转化为对应的Type
    /// </summary>
    /// <param name="typeName">类型名称</param>
    /// <returns></returns>
    public static Type? ToType(this string typeName)
    {
        var types = Assembly.GetEntryAssembly()?.GetTypes();
        return types?.FirstOrDefault(t => t.Name.Equals(typeName));
    }

    /// <summary>
    /// 计算表达式的数值
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static double Evaluate(this string expression)
    {
        var tokens = expression.ToCharArray();
        var values = new Stack<double>();
        var ops = new Stack<char>();
        for (var i = 0; i < tokens.Length; i++)
        {
            switch (tokens[i])
            {
                case ' ':
                    continue;

                case >= '0' and <= '9':
                    {
                        var buff = new StringBuilder();
                        while (i < tokens.Length &&
                               ((tokens[i] >= '0' && tokens[i] <= '9') || tokens[i] == '.'))
                        {
                            buff.Append(tokens[i++]);
                        }
                        values.Push(double.Parse(buff.ToString()));
                        i--;
                        break;
                    }
                case '(':
                    ops.Push(tokens[i]);
                    break;

                case ')':
                    {
                        while (ops.Peek() != '(')
                        {
                            values.Push(ApplyOp(ops.Pop(), values.Pop(), values.Pop()));
                        }
                        ops.Pop();
                        break;
                    }
                case '-':
                    {
                        if (i == 0 || tokens[i - 1] == '(')
                        {
                            var buff = new StringBuilder();
                            buff.Append(tokens[i]);
                            i++;
                            while (i < tokens.Length &&
                                   ((tokens[i] >= '0' && tokens[i] <= '9') || tokens[i] == '.'))
                            {
                                buff.Append(tokens[i++]);
                            }
                            values.Push(double.Parse(buff.ToString()));
                            i--;
                        }
                        else
                        {
                            while (ops.Count > 0 && HasPrecedence(tokens[i], ops.Peek()))
                            {
                                values.Push(ApplyOp(ops.Pop(), values.Pop(), values.Pop()));
                            }
                            ops.Push(tokens[i]);
                        }

                        break;
                    }
                case '+':
                case '*':
                case '/':
                    {
                        while (ops.Count > 0 && HasPrecedence(tokens[i], ops.Peek()))
                        {
                            values.Push(ApplyOp(ops.Pop(), values.Pop(), values.Pop()));
                        }

                        ops.Push(tokens[i]);
                        break;
                    }
            }
        }
        while (ops.Count > 0)
        {
            values.Push(ApplyOp(ops.Pop(), values.Pop(), values.Pop()));
        }

        return values.Pop();
    }

    private static bool HasPrecedence(char op1, char op2)
    {
        if (op2 is '(' or ')')
            return false;

        return (op1 != '*' && op1 != '/') || (op2 != '+' && op2 != '-');
    }

    private static double ApplyOp(char op, double b, double a)
    {
        switch (op)
        {
            case '+':
                return a + b;

            case '-':
                return a - b;

            case '*':
                return a * b;

            case '/':
                if (b == 0)
                    throw new NotSupportedException("Cannot divide by zero");
                return a / b;
        }
        return 0.0;
    }
}
