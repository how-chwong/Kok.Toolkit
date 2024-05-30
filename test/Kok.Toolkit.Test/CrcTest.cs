using Kok.Toolkit.Core.Checksum;

namespace Kok.Toolkit.Test;

public class CrcTest
{
    [Theory]
    [InlineData(new byte[] { 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39 }, 0xBB3d)]
    public void Crc16_CCITT_Ibm(byte[] bytes, ushort crc)
    {
        Assert.Equal(Crc16.Ibm.Compute(bytes), crc);
    }

    [Theory]
    [InlineData(new byte[] { 0x31, 0x32, 0x33, 0x34, 0x35 }, 0x7437)]
    public void Crc16_CCITT(byte[] bytes, ushort crc)
    {
        Assert.Equal(Crc16.Ccitt.Compute(bytes), crc);
    }

    [Theory]
    [InlineData(new byte[] { 0x31, 0x32, 0x33, 0x34, 0x35 }, 0x4560)]
    [InlineData(new byte[] { 1, 2, 3, 4, 5 }, 0x9304)]
    public void Crc16_CCITT_FALSE(byte[] bytes, ushort crc)
    {
        Assert.Equal(Crc16.CcittFalse.Compute(bytes), crc);
    }

    [Theory]
    [InlineData(new byte[] { 0x31, 0x32, 0x33, 0x34, 0x35 }, 0x711C)]
    public void Crc16_CCITT_DNP(byte[] bytes, ushort crc)
    {
        Assert.Equal(Crc16.Dnp.Compute(bytes), crc);
    }

    [Theory]
    [InlineData(new byte[] { 0x31, 0x32, 0x33, 0x34, 0x35 }, 0xCBF53A1C)]
    public void Crc32_Stand(byte[] bytes, uint crc)
    {
        Assert.Equal(Crc32.Standard.Compute(bytes), crc);
    }
}
