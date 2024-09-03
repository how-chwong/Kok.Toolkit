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
        var data = new TestMessage<CmdData>() { Data = new CmdData() { Count = 3, StateList = null } };
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

    public byte GetFcs(byte[] bytes)
    {
        var sum = 0;
        Array.ForEach(bytes.ToArray(), b => sum += b);
        return (byte)sum;
    }
}
