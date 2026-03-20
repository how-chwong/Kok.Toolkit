using Kok.Toolkit.Core.Serialization.Binary.Attributes;
using Kok.Toolkit.Core.Serialization.Binary.Handlers;

namespace Kok.Toolkit.Core.Serialization.Binary;

/// <summary>
/// 二进制序列化器
/// </summary>
public class BinarySerializer : IDisposable
{
    #region 属性及构造

    /// <summary>
    /// 字节编码
    /// </summary>
    public Encoding Encoding { get; } = Encoding.UTF8;

    /// <summary>
    /// 标识是否为小端字节序，默认False
    /// </summary>
    public bool IsLittleEndian { get; }

    /// <summary>
    /// 使用内存流构造默认二进制序列化器
    /// </summary>
    public BinarySerializer()
    {
        _stream = new MemoryStream();
        _ownsStream = true;
        InitDefaultHandler();
    }

    /// <summary>
    /// 从指定流构造二进制序列化器
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="leaveOpen">是否在Dispose时保留流（不释放）</param>
    public BinarySerializer(Stream stream, bool leaveOpen = false)
    {
        _stream = stream;
        _ownsStream = !leaveOpen;
        InitDefaultHandler();
    }

    /// <summary>
    /// 从指定流构造序列化器
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="encoding"></param>
    /// <param name="isLittleEndian"></param>
    /// <param name="leaveOpen">是否在Dispose时保留流（不释放）</param>
    public BinarySerializer(Stream stream, Encoding encoding, bool isLittleEndian = false, bool leaveOpen = false)
    {
        _stream = stream;
        Encoding = encoding;
        IsLittleEndian = isLittleEndian;
        _ownsStream = !leaveOpen;
        InitDefaultHandler();
    }

    #endregion 属性及构造

    #region 私有字段及方法

    /// <summary>
    /// 序列化流
    /// </summary>
    private readonly Stream _stream;

    /// <summary>
    /// 标识是否拥有流的所有权（Dispose时是否释放）
    /// </summary>
    private readonly bool _ownsStream;

    /// <summary>
    /// 二进制处理器列表
    /// </summary>
    private readonly List<IBinaryHandler> _handlerList = new();

    /// <summary>
    /// 初始化默认处理器
    /// </summary>
    private void InitDefaultHandler()
    {
        _handlerList.Add(new GeneralHandler(this));
        _handlerList.Add(new IpAddressHandler(this));
        _handlerList.Add(new CollectionHandler(this));
        _handlerList.Add(new DictionaryHandler(this));
        _handlerList.Add(new ObjectHandler(this));
    }

    /// <summary>
    /// 注册自定义处理器，优先于默认处理器匹配
    /// </summary>
    /// <param name="handler">自定义处理器实例</param>
    public void RegisterHandler(IBinaryHandler handler)
    {
        _handlerList.Insert(0, handler);
    }

    #endregion 私有字段及方法

    #region 写字节流

    /// <summary>
    /// 向当前流中写入字节
    /// </summary>
    /// <param name="value">待写入字节</param>
    public void Write(byte value) => _stream.WriteByte(value);

    /// <summary>
    /// 向当前流中写入字节
    /// </summary>
    /// <param name="value">待写入字节</param>
    public void Write(byte[] value) => _stream.Write(value, 0, value.Length);

    /// <summary>
    /// 写入集合内的Item数量
    /// </summary>
    /// <param name="value"></param>
    public void WriteItemCount(int value)
    {
        var handler = GetHandler<GeneralHandler>();
        handler.Write(value, typeof(int));
    }

    /// <summary>
    /// 写入字节
    /// </summary>
    /// <param name="value"></param>
    /// <param name="type"></param>
    /// <param name="presetSize"></param>
    /// <returns></returns>
    public bool Write(object? value, Type type, PresetSize? presetSize = null)
    {
        foreach (var handler in _handlerList)
        {
            if (!handler.CanHandle(type)) continue;
            if (handler.Write(value, type, presetSize))
                return true;
        }
        return false;
    }

    /// <summary>
    /// 写入字节
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="presetSize"></param>
    /// <returns></returns>
    public bool Write<T>(T value, PresetSize? presetSize = null)
        => _handlerList.Where(h => h.CanHandle(typeof(T))).Any(handler => handler.Write(value, typeof(T), presetSize));

    #endregion 写字节流

    #region 读字节流

    /// <summary>
    /// 字节流位置
    /// </summary>
    public long StreamPosition => _stream.Position;

    /// <summary>
    /// 字节流长度
    /// </summary>
    public long StreamLength => _stream.Length;

    /// <summary>
    /// 从当前流读取字节
    /// </summary>
    /// <returns></returns>
    public byte Read()
    {
        var val = _stream.ReadByte();
        if (val < 0)
            throw new Exception("已超出数据的可读取范围");
        return (byte)val;
    }

    /// <summary>
    /// 从当前流中读取字节
    /// </summary>
    /// <param name="count">读取字节个数</param>
    /// <returns></returns>
    public byte[] Read(int count)
    {
        var data = new byte[count];
        var offset = 0;
        while (offset < count)
        {
            var bytesRead = _stream.Read(data, offset, count - offset);
            if (bytesRead == 0)
                throw new EndOfStreamException($"已超出数据的可读取范围，期望读取{count}字节，实际读取{offset}字节");
            offset += bytesRead;
        }
        return data;
    }

    /// <summary>
    /// 读取集合内的Item数量
    /// </summary>
    /// <returns></returns>
    public int TryReadItemCount()
    {
        var l = 0;
        if (TryRead(ref l))
            return l;
        return -1;
    }

    /// <summary>
    /// 读取字节
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <param name="presetSize"></param>
    /// <returns></returns>
    public bool TryRead(Type type, ref object? value, PresetSize? presetSize = null)
    {
        foreach (var handler in _handlerList)
        {
            if (!handler.CanHandle(type)) continue;
            if (handler.TryRead(type, ref value, presetSize))
                return true;
        }

        return false;
    }

    /// <summary>
    /// 读取字节
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="presetSize"></param>
    /// <returns></returns>
    private bool TryRead<T>(ref T? value, PresetSize? presetSize = null)
    {
        foreach (var handler in _handlerList)
        {
            if (!handler.CanHandle(typeof(T))) continue;
            object? obj = value;
            if (!handler.TryRead(typeof(T), ref obj, presetSize))
                continue;
            value = obj == null ? default : (T)obj;
            return true;
        }
        return false;
    }

    #endregion 读字节流

    #region 公开方法

    /// <summary>
    /// 获取序列化后的字节数组
    /// </summary>
    /// <returns></returns>
    public byte[] GetBytes()
    {
        if (_stream is MemoryStream ms)
            return ms.ToArray();
        var length = _stream.Position;
        _stream.Seek(0, SeekOrigin.Begin);
        var bytes = new byte[length];
        var offset = 0;
        while (offset < length)
        {
            var bytesRead = _stream.Read(bytes, offset, (int)(length - offset));
            if (bytesRead == 0)
                throw new EndOfStreamException("从流中读取数据失败");
            offset += bytesRead;
        }
        return bytes;
    }

    /// <summary>
    /// 获取指定类型的序列化处理器
    /// </summary>
    /// <typeparam name="T">序列化处理器类型</typeparam>
    /// <returns></returns>
    public T GetHandler<T>() where T : class, IBinaryHandler
    {
        foreach (var item in _handlerList)
        {
            if (item is T handler)
                return handler;
        }

        throw new Exception($"不支持的序列化处理器类型：Type：{typeof(T)}");
    }

    /// <summary>
    /// 序列化为字节数组
    /// </summary>
    /// <typeparam name="T">待序列化实例的类型</typeparam>
    /// <param name="value">待序列化的实例</param>
    /// <param name="bytes">序列化完成后的字节数组</param>
    /// <param name="message">序列化失败的错误信息，若成功则为空</param>
    /// <returns></returns>
    public static bool Serialize<T>(T value, out byte[] bytes, out string message)
    {
        bytes = Array.Empty<byte>();
        try
        {
            message = string.Empty;
            using var serializer = CreateFromAttribute(typeof(T));
            if (serializer.Write(value))
                bytes = serializer.GetBytes();
            else
                message = $"请确保待序列化对象中无空对象，Type:{typeof(T)}";
        }
        catch (Exception ex)
        {
            message = ex.Message;
        }

        return bytes.Length > 0;
    }

    /// <summary>
    /// 将字节数组反序列化为指定类型
    /// </summary>
    /// <typeparam name="T">待反序列化实例的类型</typeparam>
    /// <param name="data">待反序列化的字节数组</param>
    /// <param name="value">反序列化得到的实例</param>
    /// <param name="message">反序列化失败的错误信息，若成功则为空</param>
    /// <returns></returns>
    public static bool Deserialize<T>(byte[] data, out T? value, out string message)
    {
        message = string.Empty;
        value = default;
        try
        {
            using var serializer = CreateFromAttribute(typeof(T), new MemoryStream(data));
            return serializer.TryRead(ref value);
        }
        catch (Exception ex)
        {
            message = ex.Message;
            return false;
        }
    }

    /// <summary>
    /// 从指定的二进制文件反序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="filePath">二进制文件绝对路径</param>
    /// <param name="value"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static bool Deserialize<T>(string filePath, out T? value, out string message)
    {
        value = default;
        if (!File.Exists(filePath))
        {
            message = "指定的文件不存在";
            return false;
        }
        var bytes = File.ReadAllBytes(filePath);
        return Deserialize(bytes, out value, out message);
    }

    /// <summary>
    /// 根据类型上的BinaryEncodingAttribute构造序列化器
    /// </summary>
    private static BinarySerializer CreateFromAttribute(Type type, MemoryStream? stream = null)
    {
        var attr = type.GetCustomAttribute<BinaryEncodingAttribute>();
        if (attr != null)
            return new BinarySerializer(stream ?? new MemoryStream(), attr.Encoding, attr.IsLittleEndian);
        return stream != null ? new BinarySerializer(stream) : new BinarySerializer();
    }

    #endregion 公开方法

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        if (_ownsStream)
            _stream?.Dispose();
        GC.SuppressFinalize(this);
    }
}
