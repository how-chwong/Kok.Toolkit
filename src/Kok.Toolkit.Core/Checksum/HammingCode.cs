using Kok.Toolkit.Core.Extension;

namespace Kok.Toolkit.Core.Checksum
{
    /// <summary>
    /// 汉明码
    /// </summary>
    public static class HammingCode
    {
        /// <summary>
        /// 将指定的半字节按8，4编码
        /// </summary>
        /// <param name="data">待编码数据</param>
        /// <param name="isLowNibble">true:取data的低4位；false:取data的高4位</param>
        /// <returns></returns>
        public static byte Encode84(byte data, bool isLowNibble)
        {
            byte result = 0;
            data = (byte)(isLowNibble ? data & 0x0F : data >> 4 & 0x0F);
            var offset = isLowNibble ? 0 : 4;
            //设置数据位
            result = result.SetBitValue(1, data.GetBitValue(0 + offset));
            result = result.SetBitValue(2, data.GetBitValue(1 + offset));
            result = result.SetBitValue(3, data.GetBitValue(2 + offset));
            result = result.SetBitValue(5, data.GetBitValue(3 + offset));
            //设置校验位
            result = result.SetBitValue(7, result.GetBitValue(5) ^ result.GetBitValue(3) ^ result.GetBitValue(1));
            result = result.SetBitValue(6, result.GetBitValue(5) ^ result.GetBitValue(2) ^ result.GetBitValue(1));
            result = result.SetBitValue(4, result.GetBitValue(3) ^ result.GetBitValue(2) ^ result.GetBitValue(1));
            result = result.SetBitValue(0, result.GetBitValue(1) ^ result.GetBitValue(2) ^ result.GetBitValue(3) ^ result.GetBitValue(4) ^ result.GetBitValue(5) ^ result.GetBitValue(6) ^ result.GetBitValue(7));

            return result;
        }

        /// <summary>
        /// 将指定的8位数值按8，4解码为半字节原值
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static (byte errorBit, byte data) Decode84(byte result)
        {
            byte data = 0;
            var p1 = result.GetBitValue(7) ^ result.GetBitValue(5) ^ result.GetBitValue(3) ^ result.GetBitValue(1) ? 1 : 0;
            var p2 = result.GetBitValue(6) ^ result.GetBitValue(5) ^ result.GetBitValue(2) ^ result.GetBitValue(1) ? 1 : 0;
            var p3 = result.GetBitValue(4) ^ result.GetBitValue(3) ^ result.GetBitValue(2) ^ result.GetBitValue(1) ? 1 : 0;
            byte errorBit = (byte)(p1 * 1 + p2 * 2 + p3 * 4);
            if (errorBit > 0)
            {
                result = result.SetBitValue(8 - errorBit, !result.GetBitValue(8 - errorBit));
            }

            data = data.SetBitValue(0, result.GetBitValue(1));
            data = data.SetBitValue(1, result.GetBitValue(2));
            data = data.SetBitValue(2, result.GetBitValue(3));
            data = data.SetBitValue(3, result.GetBitValue(5));

            return (errorBit, data);
        }

        /// <summary>
        /// 将指定的字节按8,4编码
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ushort Encode84(byte data)
        {
            var low = Encode84(data, true);
            var high = Encode84(data, false);
            return (ushort)(low + (ushort)(high << 8));
        }

        /// <summary>
        /// 将指定的数据按8，4解码为单字节
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static (byte error1, byte error2, byte data) Decode84(ushort result)
        {
            var (error1, high) = Decode84((byte)(result / 256));
            var (error2, low) = Decode84((byte)(result % 256));
            return (error1, error2, (byte)((high << 4) + low));
        }
    }
}
