using Kok.Toolkit.Core.Extension;

namespace Kok.Toolkit.Core.IO;

/// <summary>
/// CSV文件
/// </summary>
public class CsvFile : IDisposable
{
    /// <summary>
    /// 文件编码
    /// </summary>
    public Encoding Encoding { get; } = Encoding.UTF8;

    /// <summary>
    /// 分隔符，默认逗号
    /// </summary>
    public char Separator { get; } = ',';

    private readonly Stream? _steam;

    /// <summary>
    /// 构造一个csv文件
    /// </summary>
    /// <param name="file">文件路径</param>
    public CsvFile(string file)
    {
        if (File.Exists(file))
            _steam = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
    }

    /// <summary>
    /// 构造一个csv文件
    /// </summary>
    /// <param name="file"></param>
    /// <param name="encoding"></param>
    public CsvFile(string file, Encoding encoding)
    {
        if (File.Exists(file))
            _steam = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        Encoding = encoding;
    }

    private StreamReader? _reader;

    /// <summary>
    /// 读取一行
    /// </summary>
    /// <returns></returns>
    public string[]? ReadLine()
    {
        if (_reader == null)
        {
            if (_steam == null) return null;
            _reader = new StreamReader(_steam, Encoding);
        }
        var line = _reader.ReadLine();
        return line?.Split(Separator);
    }

    /// <summary>
    /// 读取所有行
    /// </summary>
    /// <returns></returns>
    public string[][] ReadAll()
    {
        var list = new List<string[]>();
        while (true)
        {
            var line = ReadLine();
            if (line == null || line.Length == 0)
                break;
            list.Add(line);
        }
        return list.ToArray();
    }

    /// <summary>
    /// 读取一行
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    private bool ReadLine<T>(out T? value, out string error) where T : new()
    {
        error = string.Empty;
        var str = ReadLine();
        if (str.IsEmpty())
        {
            value = default;
            return true;
        }
        value = new T();
        var properties = value.GetType().GetProperties();
        if (properties.Length != str!.Length)
        {
            value = default;
            error = $"读取到的内容与目标属性数量不符:{str.ToString("", Separator.ToString())}";
            return false;
        }
        try
        {
            for (var i = 0; i < properties.Length; i++)
            {
                properties[i].SetValue(value, str[i].ChangeType(properties[i].PropertyType));
            }
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return false;
        }

        return true;
    }

    /// <summary>
    /// 读取所有行
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public bool ReadAll<T>(out List<T> result, out string error) where T : new()
    {
        result = new List<T>();
        while (true)
        {
            if (!ReadLine(out T? line, out error))
                return false;

            if (line == null)
                break;
            result.Add(line);
        }

        return true;
    }

    /// <summary>
    /// 销毁
    /// </summary>
    public void Dispose()
    {
        _reader?.Dispose();
        _steam?.Dispose();
        GC.SuppressFinalize(this);
    }
}
