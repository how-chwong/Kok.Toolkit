using Kok.Toolkit.Core.Serialization.Binary;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

namespace Kok.Toolkit.Core.Communication;

/// <summary>
/// 进程间通信器
/// </summary>
public class ProcessCommunicator
{
    private readonly string _sharedMemoryName;
    private readonly int _sharedMemoryCapacity;
    private MemoryMappedFile? _mmf;

    /// <summary>
    /// 构造一个通过非持久化的共享内存实现进程间通信的实例
    /// </summary>
    /// <param name="sharedMemoryName">共享名称</param>
    /// <param name="sharedMemoryCapacity">内存容量</param>
    public ProcessCommunicator(string sharedMemoryName, int sharedMemoryCapacity)
    {
        _sharedMemoryName = sharedMemoryName;
        _sharedMemoryCapacity = sharedMemoryCapacity;
    }

    /// <summary>
    /// 创建共享内存
    /// </summary>
    /// <param name="message">创建失败时返回的错误信息</param>
    /// <returns></returns>
    public bool CreateSharedMemory(out string message)
    {
        try
        {
            _mmf = MemoryMappedFile.CreateNew(_sharedMemoryName, _sharedMemoryCapacity);
            message = string.Empty;
            return true;
        }
        catch (Exception e)
        {
            message = e.Message;
            return false;
        }
    }

    /// <summary>
    /// 写入共享内存
    /// </summary>
    /// <param name="offset">共享内存的起始字节</param>
    /// <param name="size">共享内存大小，如果为0则表示从offset开始到内存结尾结束</param>
    /// <param name="data">待写入的字节</param>
    /// <param name="message">写入失败时，返回失败原因</param>
    /// <returns></returns>
    public bool WriteSharedMemory(int offset, int size, byte[] data, out string message)
    {
        if (data.Length == 0)
        {
            message = "待写入字节不能为空";
            return false;
        }
        if (size > 0 && (data.Length > size || offset + size > _sharedMemoryCapacity))
        {
            message = "待写入字节数超出了允许的容量大小";
            return false;
        }

        try
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                message = "共享内存的方式仅支持windows平台";
                return false;
            }
            message = string.Empty;
            using var mmf = MemoryMappedFile.OpenExisting(_sharedMemoryName);
            using var stream = mmf.CreateViewStream(offset, size);
            using var writer = new BinaryWriter(stream);
            writer.Write(data);
            return true;
        }
        catch (FileNotFoundException fe)
        {
            message = $"未找到指定名称的共享内存：{fe.Message}";
            return false;
        }
        catch (Exception e)
        {
            message = e.Message;
            return false;
        }
    }

    /// <summary>
    /// 写入共享内存
    /// </summary>
    /// <param name="data">待写入的字节</param>
    /// <param name="message">写入失败时，返回失败原因</param>
    /// <returns></returns>
    public bool WriteSharedMemory(byte[] data, out string message)
        => WriteSharedMemory(0, 0, data, out message);

    /// <summary>
    /// 写入共享内存
    /// </summary>
    /// <typeparam name="T">待写入对象类型</typeparam>
    /// <param name="data">待写入对象</param>
    /// <param name="message">写入失败时，返回失败原因</param>
    /// <returns></returns>
    public bool WriteShareMemory<T>(T data, out string message)
    {
        if (BinarySerializer.Serialize(data, out var bytes, out var error))
            return WriteSharedMemory(bytes, out message);
        message = error;
        return false;
    }

    /// <summary>
    /// 读取共享内存
    /// </summary>
    /// <param name="offset">共享内存的起始字节</param>
    /// <param name="size">共享内存大小，如果为0则表示从offset开始到内存结尾结束</param>
    /// <param name="data">读取到的字节</param>
    /// <param name="message">读取失败时的错误信息</param>
    /// <returns></returns>
    public bool ReadSharedMemory(int offset, int size, out byte[] data, out string message)
    {
        message = string.Empty;
        data = Array.Empty<byte>();
        if (offset > _sharedMemoryCapacity || offset + size > _sharedMemoryCapacity)
        {
            message = "待读取字节数超出了允许的容量大小";
            return false;
        }
        try
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                message = "共享内存的方式仅支持windows平台";
                return false;
            }
            using var mmf = MemoryMappedFile.OpenExisting(_sharedMemoryName);
            using var stream = mmf.CreateViewStream(offset, size);
            var reader = new BinaryReader(stream);
            data = reader.ReadBytes((int)stream.Length);
            return true;
        }
        catch (FileNotFoundException fe)
        {
            message = $"未找到指定名称的共享内存：{fe.Message}";
            return false;
        }
        catch (Exception e)
        {
            message = e.Message;
            return false;
        }
    }

    /// <summary>
    /// 读取共享内存
    /// </summary>
    /// <param name="data">读取到的字节</param>
    /// <param name="message">读取失败时的错误信息</param>
    /// <returns></returns>
    public bool ReadSharedMemory(out byte[] data, out string message)
        => ReadSharedMemory(0, 0, out data, out message);

    /// <summary>
    /// 读取共享内存
    /// </summary>
    /// <typeparam name="T">读取到的对象类型</typeparam>
    /// <param name="data">读取到的对象</param>
    /// <param name="message">读取失败时的错误信息</param>
    /// <returns></returns>
    public bool ReadSharedMemory<T>(out T? data, out string message)
    {
        if (ReadSharedMemory(out var bytes, out message))
            return BinarySerializer.Deserialize(bytes, out data, out message);
        data = default;
        return false;
    }

    /// <summary>
    /// 析构
    /// </summary>
    ~ProcessCommunicator() => _mmf?.Dispose();
}
