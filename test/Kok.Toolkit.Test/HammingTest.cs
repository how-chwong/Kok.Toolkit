using Kok.Toolkit.Core.Checksum;

namespace Kok.Toolkit.Test
{
    public class HammingTest
    {
        [Theory]
        [InlineData(0b1011, 0b01100110)]
        [InlineData(0b1100, 0b01111000)]
        public void Encode84Test(byte data, byte code)
        {
            Assert.Equal(code, HammingCode.Encode84(data, true));
        }

        [Theory]
        [InlineData(0b01100110, 0b1011, 0)]
        [InlineData(0b01101110, 0b1011, 5)]
        [InlineData(0b01000110, 0b1011, 3)]
        [InlineData(0b11100110, 0b1011, 1)]
        [InlineData(0b01100111, 0b1011, 0)]
        [InlineData(0b00100110, 0b1011, 2)]
        public void Decode84Test(byte code, byte data, byte errorBit)
        {
            var (error, temp) = HammingCode.Decode84(code);
            Assert.Equal(data, temp);
            Assert.Equal(errorBit, error);
        }

        [Theory]
        [InlineData(0b0110011001100110, 0b10111011)]
        public void DecodeByte84Test(ushort code, byte data)
        {
            var (_, _, temp) = HammingCode.Decode84(code);
            Assert.Equal(data, temp);
        }
    }
}
