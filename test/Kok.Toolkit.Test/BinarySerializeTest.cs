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
    public void DeserializeReadOnlyMemoryTest()
    {
        // 基元类型（string）
        var str = "ReadOnlyMemory";
        Assert.True(BinarySerializer.Serialize(str, out var bytes, out _));
        ReadOnlyMemory<byte> mem = bytes;
        Assert.True(BinarySerializer.Deserialize<string>(mem, out var str2, out _));
        Assert.Equal(str, str2);

        // 复杂对象，与 byte[] 重载结果一致
        var obj = new TestMessage<CmdData>
        {
            Header = 42,
            Data = new CmdData { Type = 7, SourceId = 999, Count = 2, StateList = new List<byte> { 11, 22 } }
        };
        Assert.True(BinarySerializer.Serialize(obj, out var objBytes, out _));
        ReadOnlyMemory<byte> objMem = objBytes;
        Assert.True(BinarySerializer.Deserialize<TestMessage<CmdData>>(objMem, out var obj2, out _));
        Assert.NotNull(obj2);
        Assert.Equal(obj.Header, obj2.Header);
        Assert.Equal(obj.Data!.SourceId, obj2.Data!.SourceId);
        Assert.Equal(obj.Data.StateList![1], obj2.Data.StateList![1]);
    }

    [Fact]
    public void DeserializeReadOnlySpanTest()
    {
        // 基元类型（int）
        var num = 123456;
        Assert.True(BinarySerializer.Serialize(num, out var bytes, out _));
        Assert.True(BinarySerializer.Deserialize<int>(bytes.AsSpan(), out var num2, out _));
        Assert.Equal(num, num2);

        // 复杂对象
        var obj = new CmdData { Type = 5, SourceId = 777, Count = 3, StateList = new List<byte> { 1, 2, 3 } };
        Assert.True(BinarySerializer.Serialize(obj, out var objBytes, out _));
        Assert.True(BinarySerializer.Deserialize<CmdData>(objBytes.AsSpan(), out var obj2, out _));
        Assert.NotNull(obj2);
        Assert.Equal(obj.SourceId, obj2.SourceId);
        Assert.Equal(3, obj2.StateList?.Count);
        Assert.Equal(obj.StateList![2], obj2.StateList![2]);
    }

    [Fact]
    public void SpanDeserializeEqualsArrayDeserializeTest()
    {
        // ReadOnlyMemory / ReadOnlySpan 结果与 byte[] 重载完全一致
        var data = new TestMessage<CmdData>
        {
            Header = 99,
            Data = new CmdData { Type = 3, SourceId = 1, Count = 1, StateList = new List<byte> { 77 } }
        };
        Assert.True(BinarySerializer.Serialize(data, out var bytes, out _));

        Assert.True(BinarySerializer.Deserialize<TestMessage<CmdData>>(bytes, out var fromArray, out _));
        Assert.True(BinarySerializer.Deserialize<TestMessage<CmdData>>((ReadOnlyMemory<byte>)bytes, out var fromMemory, out _));
        Assert.True(BinarySerializer.Deserialize<TestMessage<CmdData>>(bytes.AsSpan(), out var fromSpan, out _));

        Assert.Equal(fromArray!.Header, fromMemory!.Header);
        Assert.Equal(fromArray.Header, fromSpan!.Header);
        Assert.Equal(fromArray.Crc, fromMemory.Crc);
        Assert.Equal(fromArray.Crc, fromSpan.Crc);
        Assert.Equal(fromArray.Data!.StateList![0], fromMemory.Data!.StateList![0]);
        Assert.Equal(fromArray.Data.StateList[0], fromSpan.Data!.StateList![0]);
    }

    [Fact]
    public void EnumSerializeTest()
    {
        var val = TestEnum.Second;
        Assert.True(BinarySerializer.Serialize(val, out var bytes, out _));
        Assert.True(BinarySerializer.Deserialize<TestEnum>(bytes, out var val2, out _));
        Assert.Equal(val, val2);
    }

    [Fact]
    public void DateTimeSerializeTest()
    {
        var dt = new DateTime(2026, 3, 20, 12, 0, 0, DateTimeKind.Utc);
        Assert.True(BinarySerializer.Serialize(dt, out var bytes, out _));
        Assert.True(BinarySerializer.Deserialize<DateTime>(bytes, out var dt2, out _));
        Assert.Equal(dt.Ticks, dt2.Ticks);
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
public enum TestEnum : byte
{
    First = 1,
    Second = 2,
    Third = 3
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
