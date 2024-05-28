namespace Kok.Toolkit.Core.Checksum;

/// <summary>
/// CRC-8
/// </summary>
public enum Crc8Algorithm
{
    /// <summary>
    /// CRC-8 0x07
    /// </summary>
    CRC_8_STANDARD,

    /// <summary>
    /// CRC8-ITU 0x07
    /// </summary>
    CRC_8_ITU,

    /// <summary>
    /// CRC8-ROHC 0x07
    /// </summary>
    CRC_8_ROHC,

    /// <summary>
    /// CRC9-MAXIM 0x31
    /// </summary>
    CRC_8_MAXIM
}

/// <summary>
/// CRC-16
/// </summary>
public enum Crc16Algorithm
{
    /// <summary>
    /// CRC-16-IBM 0x8005
    /// </summary>
    CRC_16_IBM,

    /// <summary>
    /// CRC-16-MAXIM
    /// </summary>
    CRC_16_MAXIM,

    /// <summary>
    /// CRC-16-USB 0X8005
    /// </summary>
    CRC_16_USB,

    /// <summary>
    /// CRC-16-MODBUS 0X8005
    /// </summary>
    CRC_16_MODBUS,

    /// <summary>
    /// CRC-16-CCITT 0X1021
    /// </summary>
    CRC_16_CCITT,

    /// <summary>
    /// CRC-16-CCITT-FALSE 0X1021
    /// </summary>
    CRC_16_CCITT_FALSE,

    /// <summary>
    /// CRC-16-X25 0X1021
    /// </summary>
    CRC_16_X25,

    /// <summary>
    /// CRC-16-YMODEM 0X0121
    /// </summary>
    CRC_16_YMODEM,

    /// <summary>
    /// CRC-16-DNP 0X3D65
    /// </summary>
    CRC_16_DNP
}

/// <summary>
/// CRC-32
/// </summary>
public enum Crc32Algorithm
{
    /// <summary>
    /// CRC-32 0X4C11DB7
    /// 初始F,异或F,反转
    /// </summary>
    CRC_32_STANDARD,

    /// <summary>
    /// CRC-32 0X4C11DB7
    /// 初始0，异或0,不反转
    /// </summary>
    CRC_32_STANDARD_FALSE,

    /// <summary>
    /// CRC-32-MPEG2 0X04C11DB7
    /// 初始F,异或0，不反转
    /// </summary>
    CRC_32_MPEG2
}

/// <summary>
/// 循环冗余校验
/// </summary>
public static class Crc
{
    /// <summary>
    /// 计算校验和
    /// </summary>
    /// <param name="data"></param>
    /// <param name="algorithm"></param>
    /// <param name="start"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static byte Compute(byte[] data, Crc8Algorithm algorithm, int start, int length)
    {
        return algorithm switch
        {
            Crc8Algorithm.CRC_8_STANDARD => Crc8.Standard.Compute(data, start, length),
            Crc8Algorithm.CRC_8_ITU => Crc8.Itu.Compute(data, start, length),
            Crc8Algorithm.CRC_8_MAXIM => Crc8.Maxim.Compute(data, start, length),
            Crc8Algorithm.CRC_8_ROHC => Crc8.Rohc.Compute(data, start, length),
            _ => throw new ArgumentOutOfRangeException(nameof(algorithm), algorithm, "不支持的校验和算法")
        };
    }

    /// <summary>
    /// 计算校验和
    /// </summary>
    /// <param name="data"></param>
    /// <param name="algorithm"></param>
    /// <returns></returns>
    public static byte Compute(byte[] data, Crc8Algorithm algorithm)
        => Compute(data, algorithm, 0, data.Length);

    /// <summary>
    /// 计算校验和
    /// </summary>
    /// <param name="data"></param>
    /// <param name="algorithm"></param>
    /// <param name="start"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static ushort Compute(byte[] data, Crc16Algorithm algorithm, int start, int length)
    {
        return algorithm switch
        {
            Crc16Algorithm.CRC_16_IBM => Crc16.Ibm.Compute(data, start, length),
            Crc16Algorithm.CRC_16_MAXIM => Crc16.Maxim.Compute(data, start, length),
            Crc16Algorithm.CRC_16_USB => Crc16.Usb.Compute(data, start, length),
            Crc16Algorithm.CRC_16_MODBUS => Crc16.ModBus.Compute(data, start, length),
            Crc16Algorithm.CRC_16_CCITT => Crc16.Ccitt.Compute(data, start, length),
            Crc16Algorithm.CRC_16_CCITT_FALSE => Crc16.CcittFalse.Compute(data, start, length),
            Crc16Algorithm.CRC_16_X25 => Crc16.X25.Compute(data, start, length),
            Crc16Algorithm.CRC_16_YMODEM => Crc16.YModem.Compute(data, start, length),
            Crc16Algorithm.CRC_16_DNP => Crc16.Dnp.Compute(data, start, length),
            _ => throw new ArgumentOutOfRangeException(nameof(algorithm), algorithm, "不支持的校验和算法")
        };
    }

    /// <summary>
    /// 计算校验和
    /// </summary>
    /// <param name="data"></param>
    /// <param name="algorithm"></param>
    /// <returns></returns>
    public static ushort Compute(byte[] data, Crc16Algorithm algorithm)
        => Compute(data, algorithm, 0, data.Length);

    /// <summary>
    /// 计算校验和
    /// </summary>
    /// <param name="data"></param>
    /// <param name="algorithm"></param>
    /// <param name="start"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static uint Compute(byte[] data, Crc32Algorithm algorithm, int start, int length)
    {
        return algorithm switch
        {
            Crc32Algorithm.CRC_32_STANDARD => Crc32.Standard.Compute(data, start, length),
            Crc32Algorithm.CRC_32_MPEG2 => Crc32.Mpeg2.Compute(data, start, length),
            Crc32Algorithm.CRC_32_STANDARD_FALSE => Crc32.StandardFalse.Compute(data, start, length),
            _ => throw new ArgumentOutOfRangeException(nameof(algorithm), algorithm, "不支持的校验和算法")
        };
    }

    /// <summary>
    /// 计算校验和
    /// </summary>
    /// <param name="data"></param>
    /// <param name="algorithm"></param>
    /// <returns></returns>
    public static uint Compute(byte[] data, Crc32Algorithm algorithm)
        => Compute(data, algorithm, 0, data.Length);
}