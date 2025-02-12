using Kok.Toolkit.Core.Extension;
using Kok.Toolkit.Core.Net;
using Kok.Toolkit.Core;

namespace Kok.Toolkit.Test
{
    public class ExtensionTest
    {
        [Theory]
        [InlineData(0b_0000_0001, 0b_1000_0000)]
        [InlineData(0b_0000_0011, 0b_1100_0000)]
        [InlineData(0b_0000_0101, 0b_1010_0000)]
        [InlineData(0b_0000_1001, 0b_1001_0000)]
        [InlineData(0b_0001_0001, 0b_1000_1000)]
        public void ByteRevert(byte value, byte revertValue)
        {
            Assert.Equal(revertValue, value.Reverse());
        }

        [Theory]
        [InlineData((ushort)0b_0000_0000_0000_0001, (ushort)0b_1000_0000_0000_0000)]
        [InlineData((ushort)0b_0000_0000_0000_0011, (ushort)0b_1100_0000_0000_0000)]
        [InlineData((ushort)0b_0000_0000_0001_0001, (ushort)0b_1000_1000_0000_0000)]
        public void UshortRevertByBit(ushort value, ushort revertValue)
        {
            Assert.Equal(revertValue, value.ReverseBit());
        }

        [Theory]
        [InlineData((ushort)0b_0000_0000_0000_0001, (ushort)0b_0000_0001_0000_0000)]
        [InlineData((ushort)0b_0000_0000_0000_0011, (ushort)0b_0000_0011_0000_0000)]
        [InlineData((ushort)0b_0001_0000_0001_0001, (ushort)0b_0001_0001_0001_0000)]
        public void UshortRevertByByte(ushort value, ushort revertValue)
        {
            Assert.Equal(revertValue, value.ReverseByte());
        }

        [Theory]
        [InlineData((ushort)0b_0000_0000_0000_0001, (ushort)0b_1111_1110_1111_1111)]
        [InlineData((ushort)0b_0000_0000_0000_0011, (ushort)0b_1111_1100_1111_1111)]
        [InlineData((ushort)0b_0100_0010_0001_0001, (ushort)0b_1110_1110_1011_1101)]
        public void ByteSwapRevert(ushort value, ushort revertValue)
        {
            Assert.Equal(revertValue, value.ReverseComplementByte());
        }

        [Theory]
        [InlineData(0b_0000_0011, 0b_0000_0011, 0, 3)]
        [InlineData(0b_0000_1111, 0b_0000_1111, 0, 3)]
        [InlineData(0b_0011_0000, 0b_0000_0000, 0, 3)]
        [InlineData(0b_0011_0000, 0b_0000_0011, 4, 7)]
        public void GetBitValue(byte value, int newValue, int start, int end)
        {
            Assert.Equal(newValue, value.GetBitValue(start, end));
        }

        [Fact]
        public void TrimEmptyTest()
        {
            var s1 = " A1 00 01 ";
            Assert.Equal("A10001", s1.TrimEmpty());
        }

        [Fact]
        public void HexStringTest()
        {
            var e = "xyz".TryToHexArray(out _);
            Assert.False(e);
            var data = new byte[] { 0x90, 0xa1, 0x12 };
            var str = Convert.ToHexString(data);
            var r = str.TryToHexArray(out var data1);
            Assert.Equal(data1.Length, data.Length);
            Assert.Equal(data1[0], data[0]);
            Assert.Equal(data1[1], data[1]);
            Assert.Equal(data1[2], data[2]);
        }

        [Fact]
        public void LocalIpTest()
        {
            var ips = Network.LocalIps;
            Assert.True(Network.IsLocalIp("127.0.0.1"));

            Assert.True(Network.IsLocalIp(ips[0].ToString()));
        }

        [Fact]
        public void HasSameItemsTest()
        {
            var list1 = new List<int>() { 1, 2, 3, 4, 5 };
            var list2 = new List<int>() { 3, 2, 1, 5, 4 };
            Assert.True(list1.HasSameItems(list2));
            Assert.False(list1.HasSameItems(Array.Empty<int>().ToList()));
            Assert.False(list1.HasSameItems(null));
        }

        [Fact]
        public void AngleTest()
        {
            var a = Calculator.Angle(1391, 945, 1391 + 361, 945, 1391 + 198, 945 - 198);
            var b = Calculator.Angle(2307, 945, 2307 - 329, 945, 2307 - 100, 945 - 100);

            var c = Calculator.Angle(1, 1, 3, 3, 3, 1);
            var d = Calculator.Angle(1, 1, 3, 1, 3, 3);
            var e = Calculator.Angle(1, 1, 3, 0, 3, 1);

            var f = Calculator.TransitionPoint45(1, 1, 3, 1, 3, 2);
            var g = Calculator.TransitionPoint45(3, 1, 1, 1, 1, 2);
            var h = Calculator.TransitionPoint45(3, 2, 6, 2, 10, 1);

            var i = Calculator.TransitionPoint(45, 12127, 595, 12179, 595, 12151, 619);
            var j = Calculator.TransitionPoint(63.435, 4794, 745, 4949, 745, 4856, 645);
        }

        [Fact]
        public void BitArrayConvertTest()
        {
            byte a = 3;
            var t = a.ToBigBitArray(3);
            var a1 = t.ToByte();
            Assert.True(a == a1);

            ushort b = 36864;
            var temp = BitConverter.GetBytes(b);
            var t2 = b.ToBigBitArray(16);
            var c = t2.ToByteArray();
            var b1 = t2.ToUInt16();
            Assert.True(b == b1);

            var bytes = new byte[] { 255, 63 };
            var bin = bytes.ToBigBitArray(16);
            var bin2 = bytes.ToBigBitArray(14);
            var bytes2 = bin.ToByteArray();
            var bytes3 = bin2.ToByteArray();
            Assert.Equal(bytes[1], bytes3[1]);
        }

        [Fact]
        public void NavigateTest()
        {
            var b1 = new ReadOnlySpan<byte>(new byte[] { 251 });
            var v1 = b1.ToSignedInteger(0, b1.Length);
            Assert.Equal(v1, -5);

            var b2 = new ReadOnlySpan<byte>(new byte[] { 0xFE, 0x0C });
            var v2 = b2.ToSignedInteger(0, b2.Length);
            Assert.Equal(v2, -500);
        }
    }
}
