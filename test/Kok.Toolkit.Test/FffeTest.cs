﻿using Kok.Toolkit.Core;

namespace Kok.Toolkit.Test
{
    public class FffeTest
    {
        [Theory]
        [InlineData(new byte[] { 0xFF, 0xFE, 0x33, 0xFF, 0xFD, 0xFF }, new byte[] { 0xFF, 0xFE, 0x33, 0xFF, 0xFD }, new byte[] { 0xFF })]
        [InlineData(new byte[] { 0xAA, 0xFF, 0xFE, 0x33, 0xFF, 0xFF, 0xFD, 0xFF }, new byte[] { 0xFF, 0xFE, 0x33, 0xFF, 0xFF, 0xFD }, new byte[] { 0xFF })]
        public void FffeUnpackTest(byte[] data, byte[] frame, byte[] rest)
        {
            FrameState state = FrameState.WaitHeadFlag;
            FffeEncoding.Unpack(data, ref state, out var temp, out var restBytes);
            Assert.Equal(frame, temp);
            Assert.Equal(rest, restBytes.ToArray());
        }

        [Theory]
        [InlineData(new byte[] { 0xFF, 0xFE, 0x11, 0x22, 0x33, 0xFF, 0xFD }, new byte[] { 0x11, 0x22, 0x33 }, true)]
        [InlineData(new byte[] { 0xFF, 0xFE, 0x11, 0xFF, 0x00, 0xFF, 0xFD }, new byte[] { 0x11, 0xFF }, true)]
        [InlineData(new byte[] { 0xFF, 0xFE, 0x01, 0xFF, 0x02, 0x02, 0x03, 0xFF, 0xFD }, new byte[] { 0x01, 0xFF, 0x02, 0xFF, 0x03 }, true)]
        [InlineData(new byte[] { 0xFF, 0xFE, 0xFF, 0x03, 0x01, 0x02, 0x03, 0xFF, 0xFD }, new byte[] { 0xFF, 0x01, 0x02, 0xFF, 0x03 }, true)]
        [InlineData(new byte[] { 0xFF, 0xFE, 0x01, 0xFF, 0x01, 0x02, 0xFF, 0x00, 0x03, 0xFF, 0xFD }, new byte[] { 0x01, 0xFF, 0xFF, 0x02, 0xFF, 0x03 }, true)]
        [InlineData(new byte[] { 0xFF, 0xFD }, new byte[0], false)]
        [InlineData(new byte[] { 0xFF, 0xFE, 0x01, 0xFF, 0xFF, 0xFD }, new byte[0], false)]

        public void FffeDecodeTest(byte[] data, byte[] frame, bool result)
        {
            Assert.Equal(FffeEncoding.Decode(data, out var temp), result);
            if (result) Assert.Equal(frame, temp);
        }

        [Theory]
        [InlineData(new byte[] { 0x11, 0x22, 0x33 }, new byte[] { 0xFF, 0xFE, 0x11, 0x22, 0x33, 0xFF, 0xFD })]
        [InlineData(new byte[] { 0x11, 0xFF }, new byte[] { 0xFF, 0xFE, 0x11, 0xFF, 0x00, 0xFF, 0xFD })]
        [InlineData(new byte[] { 0x01, 0xFF, 0x02, 0xFF, 0x03 }, new byte[] { 0xFF, 0xFE, 0x01, 0xFF, 0x02, 0x02, 0x03, 0xFF, 0xFD })]
        [InlineData(new byte[] { 0xFF, 0x01, 0x02, 0xFF, 0x03 }, new byte[] { 0xFF, 0xFE, 0xFF, 0x03, 0x01, 0x02, 0x03, 0xFF, 0xFD })]
        [InlineData(new byte[] { 0x01, 0xFF, 0xFF, 0x02, 0xFF, 0x03 }, new byte[] { 0xFF, 0xFE, 0x01, 0xFF, 0x01, 0x02, 0xFF, 0x00, 0x03, 0xFF, 0xFD })]
        public void FffeEncodeTest(byte[] data, byte[] frame)
        {
            FffeEncoding.Encode(data, out var result);
            Assert.Equal(frame, result);
        }
    }
}
