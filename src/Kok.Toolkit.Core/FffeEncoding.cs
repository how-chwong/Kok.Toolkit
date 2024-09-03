namespace Kok.Toolkit.Core;

/// <summary>
/// FFFE协议编解码
/// </summary>
public static class FffeEncoding
{
    private const byte s_flagByte = 0xFF;
    private const byte s_headByte = 0xFE;
    private const byte s_tailByte = 0xFD;

    /// <summary>
    /// 拆解一包FFFE数据
    /// </summary>
    /// <param name="data">源数据</param>
    /// <param name="state">当前拼包状态</param>
    /// <param name="frameBytes">拼包接收得到的数据</param>
    /// <param name="restBytes">拼包后剩余的字节</param>
    /// <returns>true:已经拼出了完成包，false:未拼出完整包</returns>
    public static bool Unpack(Span<byte> data, ref FrameState state, out byte[] frameBytes, out Span<byte> restBytes)
    {
        frameBytes = Array.Empty<byte>();
        restBytes = Span<byte>.Empty;
        if (data.Length == 0) return false;
        var frame = new List<byte>();
        for (var i = 0; i < data.Length; i++)
        {
            switch (state)
            {
                default:
                case FrameState.WaitHeadFlag:
                    if (s_flagByte == data[i])
                    {
                        frame.Add(data[i]);
                        state = FrameState.WaitHead;
                    }

                    break;

                case FrameState.WaitHead:
                    if (s_headByte == data[i])
                    {
                        frame.Add(data[i]);
                        state = FrameState.WaitTailFlag;
                    }
                    else
                    {
                        state = s_flagByte == data[i] ? FrameState.WaitHead : FrameState.WaitHeadFlag;
                    }

                    break;

                case FrameState.WaitTailFlag:
                    frame.Add(data[i]);
                    if (s_flagByte == data[i]) state = FrameState.WaitTail;
                    break;

                case FrameState.WaitTail:
                    frame.Add(data[i]);
                    if (s_tailByte == data[i])
                    {
                        if (i < data.Length - 1) restBytes = data.Slice(i + 1);
                        frameBytes = frame.ToArray();
                        state = FrameState.WaitHeadFlag;

                        return true;
                    }

                    state = s_flagByte == data[i] ? FrameState.WaitTail : FrameState.WaitTailFlag;
                    break;
            }
        }

        frameBytes = frame.ToArray();
        return false;
    }

    /// <summary>
    /// 按FFFE协议解码数据
    /// </summary>
    /// <param name="bytes">已按FFFE协议编码后的数据</param>
    /// <param name="frame">反编码后的数据</param>
    /// <returns></returns>
    public static bool Decode(Span<byte> bytes, out byte[] frame)
    {
        frame = Array.Empty<byte>();
        if (bytes.Length == 0) return false;
        if (bytes[0] != s_flagByte || bytes[1] != s_headByte || bytes[^2] != s_flagByte || bytes[^1] != s_tailByte) return false;

        var data = bytes.Slice(2, bytes.Length - 4);
        var temp = new List<byte>();

        for (var i = 0; i < data.Length; i++)
        {
            temp.Add(data[i]);
            if (data[i] != s_flagByte) continue;

            if (++i >= data.Length) return false;
            var distance = data[i];
            if (distance == 0) continue;
            for (var j = 1; j < distance; j++)
            {
                if (++i >= data.Length) return false;
                var val = data[i];
                if (val == s_flagByte) return false;
                temp.Add(val);
            }

            temp.Add(s_flagByte);
        }

        frame = temp.ToArray();
        return true;
    }

    /// <summary>
    /// 按FFFE协议对指定的数据进行编码
    /// </summary>
    /// <param name="data">待编码的数据</param>
    /// <param name="frame">编码后的数据</param>
    /// <returns></returns>
    public static bool Encode(Span<byte> data, out byte[] frame)
    {
        frame = Array.Empty<byte>();
        if (data.Length == 0) return false;
        var num = 0;
        var temp = new List<byte> { s_flagByte, s_headByte };
        for (var i = 0; i < data.Length; i++)
        {
            if (data[i] == s_flagByte)
            {
                num++;
                if (num % 2 == 0) continue;
                temp.Add(data[i]);
                var j = i + 1;
                for (; j < data.Length; j++)
                {
                    if (data[j] != s_flagByte) continue;
                    num++;
                    break;
                }

                if (num % 2 == 0)
                {
                    temp.Add((byte)(j - i));
                    temp.AddRange(data.Slice(i + 1, j - i - 1).ToArray());
                }
                else
                {
                    temp.Add(0);
                    temp.AddRange(data.Slice(i + 1, j - i - 1).ToArray());
                }
                i = j;
            }
            else
            {
                temp.Add(data[i]);
            }
        }

        temp.Add(s_flagByte);
        temp.Add(s_tailByte);
        frame = temp.ToArray();
        return true;
    }
}

/// <summary>
/// 帧处理状态
/// </summary>
public enum FrameState : byte
{
    /// <summary>
    /// 等待头标识FF
    /// </summary>
    WaitHeadFlag,

    /// <summary>
    /// 等待头 FF FE
    /// </summary>
    WaitHead,

    /// <summary>
    /// 等待尾标识FF
    /// </summary>
    WaitTailFlag,

    /// <summary>
    /// 等待尾 FF FD
    /// </summary>
    WaitTail
}
