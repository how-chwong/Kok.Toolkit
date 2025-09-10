namespace Kok.Toolkit.Core.Extension;

/// <summary>
/// 数值扩展
/// </summary>
public static class ValueExtension
{
    /// <summary>
    /// 按位反转
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static byte Reverse(this byte value)
    {
        value = (byte)(((value & 0xaa) >> 1) | ((value & 0x55) << 1));
        value = (byte)(((value & 0xcc) >> 2) | ((value & 0x33) << 2));
        value = (byte)((value >> 4) | (value << 4));
        return value;
    }

    /// <summary>
    /// 按字节反转
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static ushort ReverseByte(this ushort value)
        => (ushort)(((value & 0x00FFU) << 8) | ((value & 0xFF00U) >> 8));

    /// <summary>
    /// 按位反转
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static ushort ReverseBit(this ushort value)
    {
        ushort temp = 0;
        for (var i = 0; i < 16; i++)
        {
            temp = (ushort)(temp << 1);
            temp |= (ushort)((value >> i) & 0x01);
        }
        return temp;
    }

    /// <summary>
    /// 按字节反转补码
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static ushort ReverseComplementByte(this ushort value)
        => ReverseByte((ushort)~value);

    /// <summary>
    /// 按字节反转
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static uint ReverseByte(this uint value)
        => ((value & 0x000000FFU) << 24) | ((value & 0x0000FF00U) << 8) |
           ((value & 0x00FF0000U) >> 8) | ((value & 0xFF000000U) >> 24);

    /// <summary>
    /// 按位反转
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static uint ReverseBit(this uint value)
    {
        uint temp = 0;
        for (var i = 0; i < 32; i++)
            temp |= ((value >> i) & 0x01) << (31 - i);
        return temp;
    }

    /// <summary>
    /// 按字节反转
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static ulong ReverseByte(this ulong value)
        => ((value & 0x00000000000000FFUL) << 56) | ((value & 0x000000000000FF00UL) << 40) |
           ((value & 0x0000000000FF0000UL) << 24) | ((value & 0x00000000FF000000UL) << 8) |
           ((value & 0x000000FF00000000UL) >> 8) | ((value & 0x0000FF0000000000UL) >> 24) |
           ((value & 0x00FF000000000000UL) >> 40) | ((value & 0xFF00000000000000UL) >> 56);

    /// <summary>
    /// 转换为十六机制字符串
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToHexString(this uint value) => value.ToString("X8");

    /// <summary>
    /// 检测字节指定位置的位是否为1
    /// </summary>
    /// <param name="value"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static bool GetBitValue(this byte value, int index)
    {
        return index switch
        {
            0 => (value & 1) == 1,
            1 => (value & 2) == 2,
            2 => (value & 4) == 4,
            3 => (value & 8) == 8,
            4 => (value & 16) == 16,
            5 => (value & 32) == 32,
            6 => (value & 64) == 64,
            7 => (value & 128) == 128,
            _ => false
        };
    }

    /// <summary>
    /// 获取指定索引位置的值
    /// </summary>
    /// <param name="value"></param>
    /// <param name="startIndex"></param>
    /// <param name="endIndex"></param>
    /// <returns></returns>
    public static byte GetBitValue(this byte value, int startIndex, int endIndex)
    {
        if (startIndex < 0 || endIndex > 7)
            throw new Exception("索引越界");
        byte result = 0;
        var index = 0;
        for (var i = startIndex; index < 8 && i <= endIndex; i++, index++)
        {
            result = result.SetBitValue(index, value.GetBitValue(i));
        }
        return result;
    }

    /// <summary>
    /// 设置字节某一位的值
    /// </summary>
    /// <param name="value">待更改的值</param>
    /// <param name="index">位索引，从0开始</param>
    /// <param name="flag">位值</param>
    /// <returns></returns>
    public static byte SetBitValue(this byte value, int index, bool flag)
    {
        if (index is > 7 or < 0)
            return value;
        var v = index < 2 ? index + 1 : (2 << (index - 1));
        return flag ? (byte)(value | v) : (byte)(value & ~v);
    }

    /// <summary>
    /// 将字节数组转为指定长度的位数组
    /// 如果位长度不足8，则提取字节的低位，抛弃高位
    /// </summary>
    /// <param name="value">待转换的数组</param>
    /// <param name="length">待截取的位数</param>
    /// <param name="fromLowByte">是否从低字节开始取位</param>
    /// <returns></returns>
    public static BitArray ToBigBitArray(this byte[] value, int length, bool fromLowByte = true)
    {
        var array = new BitArray(length, false);
        var bitCount = 0;
        if (fromLowByte)
        {
            for (var i = 0; i < value.Length && bitCount <= length; i++)
            {
                var count = length - bitCount >= 8 ? 8 : length - bitCount;
                array.Append(bitCount, value[i].ToBigBitArray(count));
                bitCount += count;
            }
        }
        else
        {
            for (var i = value.Length - 1; i >= 0 && bitCount <= length; i--)
            {
                var count = length - bitCount >= 8 ? 8 : length - bitCount;
                bitCount += count;
                var index = length - bitCount;
                array.Append(index, value[i].ToBigBitArray(count));
            }
        }
        return array;
    }

    /// <summary>
    /// 将字节转换为指定长度的位数组
    /// </summary>
    /// <param name="value"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static BitArray ToBigBitArray(this byte value, int length)
    {
        if (length < 0)
            length = 0;
        var result = new BitArray(length, false);
        var startIndex = length <= 8 ? length - 1 : 7;
        var startBit = length <= 8 ? 0 : length - 8;
        for (var i = startIndex; i >= 0; i--, startBit++)
            result.Set(startBit, value.GetBitValue(i));
        return result;
    }

    /// <summary>
    /// 将整数转换为指定长度的位数组
    /// </summary>
    /// <param name="value">待转换的数值</param>
    /// <param name="length">待截取的位长度</param>
    /// <param name="isLittleEndian">是否为小端编码</param>
    /// <returns></returns>
    public static BitArray ToBigBitArray(this ushort value, int length, bool isLittleEndian = false)
    {
        var temp = BitConverter.GetBytes(value);
        if (BitConverter.IsLittleEndian != isLittleEndian)
            Array.Reverse(temp);
        return temp.ToBigBitArray(length, isLittleEndian);
    }

    /// <summary>
    /// 将整数转换为指定长度的位数组
    /// </summary>
    /// <param name="value"></param>
    /// <param name="length"></param>
    /// <param name="isLittleEndian"></param>
    /// <returns></returns>
    public static BitArray ToBigBitArray(this uint value, int length, bool isLittleEndian = false)
    {
        var temp = BitConverter.GetBytes(value);
        if (BitConverter.IsLittleEndian != isLittleEndian)
            Array.Reverse(temp);
        return temp.ToBigBitArray(length, isLittleEndian);
    }

    /// <summary>
    /// 向位数组的指定位置后附加
    /// </summary>
    /// <param name="value"></param>
    /// <param name="startIndex">起始索引</param>
    /// <param name="data">待填充值</param>
    public static void Append(this BitArray value, int startIndex, BitArray data)
    {
        if (startIndex < 0)
            startIndex = 0;
        var length = Math.Min(value.Length - startIndex, data.Length);
        for (var i = 0; i < length; i++, startIndex++)
            value.Set(startIndex, data[i]);
    }

    /// <summary>
    /// 将指定的位数组转换为字节数组
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromLowIndex">位数组是从开始遍历</param>
    /// <param name="isLowBit">遍历到的位是否位字节的低位</param>
    /// <returns></returns>
    public static byte[] ToByteArray(this BitArray value, bool fromLowIndex = false, bool isLowBit = false)
    {
        var byteCount = value.Length % 8 > 0 ? value.Length / 8 + 1 : value.Length / 8;
        var result = new byte[byteCount];
        var bitPosition = 0;
        if (fromLowIndex)
        {
            for (var i = 0; i < result.Length; i++)
            {
                for (var j = 0; j < 8 && bitPosition < value.Length; j++, bitPosition++)
                {
                    result[i] = isLowBit
                        ? result[i].SetBitValue(j, value[bitPosition])
                        : result[i].SetBitValue(7 - j, value[bitPosition]);
                }
            }
        }
        else
        {
            bitPosition = value.Length - 1;
            for (var i = 0; i < result.Length; i++)
            {
                for (var j = 0; j < 8 && bitPosition >= 0; j++, bitPosition--)
                {
                    result[i] = isLowBit
                        ? result[i].SetBitValue(j, value[bitPosition])
                        : result[i].SetBitValue(7 - j, value[bitPosition]);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// 按指定长度填充位数组
    /// </summary>
    /// <param name="value"></param>
    /// <param name="length"></param>
    /// <param name="fromLowBit"></param>
    /// <returns></returns>
    public static BitArray FillBitArray(this BitArray value, int length, bool fromLowBit = false)
    {
        var array = new BitArray(length, false);
        if (value.Length >= length)
            array.Append(0, value);
        else
            array.Append(fromLowBit ? 0 : length - value.Length, value);
        return array;
    }

    /// <summary>
    /// 将位数组转换为字节
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromLowIndex">位数组遍历是否从0开始</param>
    /// <param name="isLowBit">位值是否按先低位后高位</param>
    /// <returns></returns>
    public static byte ToByte(this BitArray value, bool fromLowIndex = false, bool isLowBit = true)
    {
        if (value.Length > 8)
            throw new ArgumentException("位数组的长度不能大于8");
        byte result = 0;
        if (fromLowIndex)
        {
            for (var i = 0; i < value.Length; i++)
                result = isLowBit
                    ? result.SetBitValue(i, value[i])
                    : result.SetBitValue(7 - i, value[i]);
        }
        else
        {
            var index = 0;
            for (var i = value.Length - 1; i >= 0; i--)
            {
                result = isLowBit ? result.SetBitValue(index, value[i]) : result.SetBitValue(7 - index, value[i]);
                index++;
            }
        }
        return result;
    }

    /// <summary>
    /// 将位数组转换为无符号整数
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromLowBit"></param>
    /// <param name="isLittleEndian"></param>
    /// <returns></returns>
    public static uint ToUInt32(this BitArray value, bool fromLowBit = false, bool isLittleEndian = false)
    {
        var array = value.FillBitArray(32, fromLowBit);
        var temp = array.ToByteArray();
        if (BitConverter.IsLittleEndian != isLittleEndian)
            Array.Reverse(temp);
        return BitConverter.ToUInt32(temp, 0);
    }

    /// <summary>
    /// 将位数组转换为无符号整数
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromLowBit"></param>
    /// <param name="isLittleEndian"></param>
    /// <returns></returns>
    public static ushort ToUInt16(this BitArray value, bool fromLowBit = false, bool isLittleEndian = false)
    {
        var array = value.FillBitArray(32, fromLowBit);
        var temp = array.ToByteArray();
        if (BitConverter.IsLittleEndian != isLittleEndian)
            Array.Reverse(temp);
        return BitConverter.ToUInt16(temp, 0);
    }

    /// <summary>
    /// 获取位数组的子集
    /// </summary>
    /// <param name="value"></param>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static BitArray SubArray(this BitArray value, int offset, int length = 0)
    {
        if (length == 0)
            length = value.Length - offset;
        var result = new BitArray(length);
        for (var i = 0; i < length; i++)
            result[i] = value[offset + i];
        return result;
    }

    /// <summary>
    /// 转为二进制字符串
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToBinaryString(this BitArray value)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < value.Length; i++)
            sb.Append(value[i] ? "1" : "0");
        return sb.ToString();
    }
}
