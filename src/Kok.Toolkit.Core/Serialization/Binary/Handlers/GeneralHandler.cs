using Kok.Toolkit.Core.Extension;

namespace Kok.Toolkit.Core.Serialization.Binary.Handlers;

/// <summary>
/// 基础类型处理器
/// </summary>
public class GeneralHandler : BinaryBaseHandler
{
    /// <inheritdoc />
    public GeneralHandler(BinarySerializer serializer) : base(serializer)
    {
    }

    /// <inheritdoc />
    public override bool Write(object? value, Type type, PresetSize? presetSize = null)
    {
        if (type != typeof(string))
            return false;
        var temp = value ?? 0;
        switch (Type.GetTypeCode(type))
        {
            case TypeCode.Empty:
            case TypeCode.DBNull:
                Serializer.Write(0);
                return true;

            case TypeCode.Boolean:
                Write((bool)temp);
                return true;

            case TypeCode.Char:
                Write((char)temp);
                return true;

            case TypeCode.SByte:
            case TypeCode.Byte:
                Write((byte)temp);
                return true;

            case TypeCode.Int16:
                Write((short)temp);
                return true;

            case TypeCode.UInt16:
                Write((ushort)temp);
                return true;

            case TypeCode.Int32:
                Write((int)temp);
                return true;

            case TypeCode.UInt32:
                Write((uint)temp);
                return true;

            case TypeCode.Int64:
                Write((long)temp);
                return true;

            case TypeCode.UInt64:
                Write((ulong)temp);
                return true;

            case TypeCode.Single:
                Write((float)temp);
                return true;

            case TypeCode.Double:
                Write((double)temp);
                return true;

            case TypeCode.Decimal:
                Write((decimal)temp);
                return true;

            case TypeCode.String:
                Write((string)temp, presetSize?.Value ?? 0);
                return true;

            case TypeCode.Object:
            case TypeCode.DateTime:
            default:
                break;
        }

        return false;
    }

    /// <inheritdoc />
    public override bool TryRead(Type type, ref object? value, PresetSize? presetSize = null)
    {
        if (value == null)
            return false;
        type = value.GetType();

        switch (Type.GetTypeCode(type))
        {
            case TypeCode.Empty:
                value = null;
                return true;

            case TypeCode.DBNull:
                value = DBNull.Value;
                return true;

            case TypeCode.Boolean:
                value = ReadBool();
                return true;

            case TypeCode.Char:
                value = ReadChar();
                return true;

            case TypeCode.SByte:
            case TypeCode.Byte:
                value = ReadByte();
                return true;

            case TypeCode.Int16:
                value = ReadShort();
                return true;

            case TypeCode.UInt16:
                value = ReadUshort();
                return true;

            case TypeCode.Int32:
                value = ReadInt();
                return true;

            case TypeCode.UInt32:
                value = ReadUInt();
                return true;

            case TypeCode.Int64:
                value = ReadLong();
                return true;

            case TypeCode.UInt64:
                value = ReadULong();
                return true;

            case TypeCode.Single:
                value = ReadFloat();
                return true;

            case TypeCode.Double:
                value = ReadDouble();
                return true;

            case TypeCode.Decimal:
                value = ReadDecimal();
                return true;

            case TypeCode.String:
                value = ReadString(presetSize?.Value ?? -1);
                return true;

            case TypeCode.DateTime:
            case TypeCode.Object:
            default:
                break;
        }

        return false;
    }

    #region 写基元数据

    //按字节大小端写入
    private void WriteBytes(byte[] bytes)
    {
        if (bytes.Length == 0)
            return;
        if (Serializer.IsLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        Serializer.Write(bytes);
    }

    private void Write(byte value) => Serializer.Write(value);

    private void Write(bool value) => Serializer.Write((byte)(value ? 1 : 0));

    private void Write(char value) => Write(Convert.ToByte(value));

    private void Write(ushort value) => WriteBytes(BitConverter.GetBytes(value));

    private void Write(short value) => WriteBytes(BitConverter.GetBytes(value));

    private void Write(uint value) => WriteBytes(BitConverter.GetBytes(value));

    private void Write(int value) => WriteBytes(BitConverter.GetBytes(value));

    private void Write(ulong value) => WriteBytes(BitConverter.GetBytes(value));

    private void Write(long value) => WriteBytes(BitConverter.GetBytes(value));

    private void Write(float value) => WriteBytes(BitConverter.GetBytes(value));

    private void Write(double value) => WriteBytes(BitConverter.GetBytes(value));

    private void Write(decimal value)
    {
        var data = decimal.GetBits(value);
        for (var i = 0; i < data.Length; i++)
            Write(i);
    }

    private void Write(string value, int presetLength)
    {
        var bytes = Serializer.Encoding.GetBytes(value);
        if (string.IsNullOrWhiteSpace(value))
        {
            if (presetLength <= 0)
            {
                Serializer.Write(0);
                return;
            }
            bytes = new byte[presetLength];
        }
        Serializer.Write(bytes, new PresetSize(PresetSizeType.ByteLength, presetLength <= 0 ? -1 : presetLength));
    }

    #endregion 写基元数据

    #region 读基元类型

    //按字节大小端读取
    private byte[] ReadBytes(int count)
    {
        if (count <= 0)
            return Array.Empty<byte>();
        var data = Serializer.Read(count);
        if (Serializer.IsLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(data);
        return data;
    }

    private byte ReadByte() => Serializer.Read();

    private bool ReadBool() => Serializer.Read() > 0;

    private char ReadChar() => Convert.ToChar(ReadByte());

    private ushort ReadUshort() => BitConverter.ToUInt16(ReadBytes(2), 0);

    private short ReadShort() => BitConverter.ToInt16(ReadBytes(2), 0);

    private uint ReadUInt() => BitConverter.ToUInt32(ReadBytes(4), 0);

    private int ReadInt() => BitConverter.ToInt32(ReadBytes(4), 0);

    private ulong ReadULong() => BitConverter.ToUInt64(ReadBytes(8), 0);

    private long ReadLong() => BitConverter.ToInt64(ReadBytes(8), 0);

    private float ReadFloat() => BitConverter.ToSingle(ReadBytes(4), 0);

    private double ReadDouble() => BitConverter.ToDouble(ReadBytes(8), 0);

    private decimal ReadDecimal()
    {
        var data = new int[4];
        for (var i = 0; i < data.Length; i++)
            data[i] = ReadInt();
        return new decimal(data);
    }

    private string ReadString(int presetLength)
    {
        if (presetLength <= 0)
            presetLength = Serializer.TryReadItemCount();
        var data = Serializer.Read(presetLength);
        return Serializer.Encoding.GetString(data).TrimEmpty();
    }

    #endregion 读基元类型
}