using Kok.Toolkit.Core.Checksum;
using System.Buffers.Binary;

namespace Kok.Toolkit.Core.Serialization.Binary.Attributes;

/// <summary>
/// CRC-8
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class Crc8Attribute : Attribute
{
    /// <summary>
    /// CRC
    /// </summary>
    public Crc8Algorithm Algorithm { get; }

    /// <summary>
    /// 构造CRC-8特性
    /// </summary>
    /// <param name="algorithm"></param>
    public Crc8Attribute(Crc8Algorithm algorithm = Crc8Algorithm.CRC_8_STANDARD)
    {
        Algorithm = algorithm;
    }

    /// <summary>
    /// 计算校验和
    /// </summary>
    /// <param name="data"></param>
    /// <param name="start"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public byte Compute(byte[] data, int start, int length) => Crc.Compute(data, Algorithm, start, length);
}

/// <summary>
/// CRC-16
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class Crc16Attribute : Attribute
{
    /// <summary>
    /// CRC
    /// </summary>
    public Crc16Algorithm Algorithm { get; }

    /// <summary>
    /// 是否反转字节序
    /// </summary>
    public bool IsReverseEndian { get; }

    /// <summary>
    /// 构造CRC-16特性
    /// </summary>
    /// <param name="algorithm"></param>
    /// <param name="isReverseEndian"></param>
    public Crc16Attribute(Crc16Algorithm algorithm = Crc16Algorithm.CRC_16_CCITT, bool isReverseEndian = false)
    {
        Algorithm = algorithm;
        IsReverseEndian = isReverseEndian;
    }

    /// <summary>
    /// 计算校验和
    /// </summary>
    /// <param name="data"></param>
    /// <param name="start"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public ushort Compute(byte[] data, int start, int length)
    {
        var crc = Crc.Compute(data, Algorithm, start, length);
        return IsReverseEndian ? BinaryPrimitives.ReverseEndianness(crc) : crc;
    }
}

/// <summary>
/// CRC-32
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class Crc32Attribute : Attribute
{
    /// <summary>
    /// CRC
    /// </summary>
    public Crc32Algorithm Algorithm { get; }

    /// <summary>
    /// 是否反转字节序
    /// </summary>
    public bool IsReverseEndian { get; }

    /// <summary>
    /// 构造CRC-32特性
    /// </summary>
    /// <param name="algorithm"></param>
    /// <param name="isReverseEndian"></param>
    public Crc32Attribute(Crc32Algorithm algorithm = Crc32Algorithm.CRC_32_STANDARD, bool isReverseEndian = false)
    {
        Algorithm = algorithm;
        IsReverseEndian = isReverseEndian;
    }

    /// <summary>
    /// 计算校验和
    /// </summary>
    /// <param name="data"></param>
    /// <param name="start"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public uint Compute(byte[] data, int start, int length)
    {
        var crc = Crc.Compute(data, Algorithm, start, length);
        return IsReverseEndian ? BinaryPrimitives.ReverseEndianness(crc) : crc;
    }
}
