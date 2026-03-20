using Kok.Toolkit.Core.Serialization.Binary;
using Kok.Toolkit.Core.Serialization.Binary.Attributes;

namespace Kok.Toolkit.Test;

public class BinarySerializeTest
{
    [Fact]
    public void StringSerializeTest()
    {
        var str = "H H";
        Assert.True(BinarySerializer.Serialize(str, out var data, out _));
        Assert.True(BinarySerializer.Deserialize<string>(data, out var newStr, out _));
        Assert.Equal(str, newStr);
    }

    [Fact]
    public void GeneralTypeSerializeTest()
    {
        var bytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var r = BinarySerializer.Serialize(bytes, out var r1, out _);
        Assert.True(r);
        Assert.True(BinarySerializer.Deserialize<byte[]>(r1, out var d1, out _));
        Assert.True(d1 != null);
        Assert.Equal(bytes.Length, d1.Length);
        for (var i = 0; i < bytes.Length; i++) Assert.Equal(bytes[i], d1[i]);

        var str = "Hello World!";
        Assert.True(BinarySerializer.Serialize(str, out var r2, out _));
        Assert.True(BinarySerializer.Deserialize<string>(r2, out var d2, out _));
        Assert.Equal(str, d2);
    }

    [Fact]
    public void NullSerializeTest()
    {
        Assert.False(BinarySerializer.Serialize<TestMessage<CmdData>?>(null, out _, out _));
        var data = new TestMessage<CmdData>() { Header = 10, Data = new CmdData() { Type = 20, Count = 0 } };
        Assert.True(BinarySerializer.Serialize(data, out var temp, out _));
        Assert.True(BinarySerializer.Deserialize<TestMessage<CmdData>>(temp, out var newData, out _));
        data = new TestMessage<CmdData>() { Data = new CmdData() { Count = 3, StateList = null } };
        Assert.False(BinarySerializer.Serialize(data, out byte[] _, out _));
        data.Data.Count = 0;
        Assert.True(BinarySerializer.Serialize(data, out var bytes2, out _));
        Assert.True(
            BinarySerializer.Deserialize<TestMessage<CmdData>>(bytes2, out var data2, out _));
        Assert.True(data2?.Data?.StateList?.Count == data.Data.Count);
    }

    [Fact]
    public void ObjectSerializeTest()
    {
        var data = new TestMessage<CmdData>()
        {
            Header = 88,
            Data = new CmdData() { Type = 33, SourceId = 1234, Count = 2, StateList = new List<byte>() { 55, 66 } }
        };
        Assert.True(BinarySerializer.Serialize(data, out var temp, out _));
        Assert.True(BinarySerializer.Deserialize<TestMessage<CmdData>>(temp, out var data1, out _));
        Assert.True(data1?.Data?.StateList?.Count > 0);
        Assert.Equal(data.Data.StateList[1], data1.Data.StateList[1]);
        Assert.True(data1.Crc > 0);
    }

    [Fact]
    public void FcsTest()
    {
        var data = new TestData { Header = 1, Status = 2, Name = "0r84" };

        Assert.True(BinarySerializer.Serialize(data, out var temp, out _));
    }
    [Fact]
    public void ByteArrayPropertyOrderTest()
    {
    // 排列：类中 byte[] 前有其他属性，验证序列化后前序字段不被覆盖
    var data = new PacketWithByteArray
    {
        Header = 0xAA,
        Length = 6,
        Payload = new byte[] { 1, 2, 3, 4, 5, 6 },
        Footer = 0xFF
    };

    Assert.True(BinarySerializer.Serialize(data, out var bytes, out var msg), msg);

    // 手动验证字节布局：
    // [0]      = Header (1 byte = 0xAA)
    // [1]      = Length (1 byte = 6)
    // [2..3]   = Payload 数组长度 int32 前两字节（WriteItemCount 写的是 int，4字节）
    // 此处使用 CollectionItemCount 关联 Length，不写 count 头，直接写 6 字节
    // [2..7]   = Payload 内容
    // [8]      = Footer (1 byte = 0xFF)
    Assert.True(BinarySerializer.Deserialize<PacketWithByteArray>(bytes, out var result, out var msg2), msg2);

    Assert.NotNull(result);
    Assert.Equal(0xAA, result.Header);      // 前序字段未被覆盖
    Assert.Equal(6, result.Length);
    Assert.Equal(0xFF, result.Footer);      // 后续字段正常
    Assert.NotNull(result.Payload);
    Assert.Equal(6, result.Payload.Length);
    for (var i = 0; i < data.Payload.Length; i++)
        Assert.Equal(data.Payload[i], result.Payload[i]);
    }
}
public class PacketWithByteArray
{
    [FieldOrder(0)]
    public byte Header { get; set; }

    [FieldOrder(1)]
    public byte Length { get; set; }

    [FieldOrder(2)]
    [CollectionItemCount(nameof(Length))]
    public byte[] Payload { get; set; } = Array.Empty<byte>();

    [FieldOrder(3)]
    public byte Footer { get; set; }
}

public class TestMessage<T> where T : class, new()
{
    public byte Header { get; set; }

    public T? Data { get; set; }

    [Crc16]
    public ushort Crc { get; set; }
}

public class CmdData
{
    public byte Type { get; set; }

    public int SourceId { get; set; }

    public int Count { get; set; }

    [CollectionItemCount(nameof(Count))]
    public List<byte>? StateList { get; set; }
}

public class TestData
{
    public byte Header { get; set; }

    public byte Status { get; set; }

    [CollectionByteLength(4)]
    public string Name { get; set; } = string.Empty;

    [Fcs(nameof(GetFcs))]
    public byte Fcs { get; set; }

    public byte GetFcs(byte[] data)
    {
        var sum = 0;
        Array.ForEach(data, d => sum += d);
        sum = ~sum;
        sum += 1;
        return (byte)(sum & 255);
    }
}
