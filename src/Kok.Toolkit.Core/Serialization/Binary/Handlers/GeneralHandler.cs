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
    public override bool CanHandle(Type type)
    {
        if (type.IsEnum) return true;
        var code = Type.GetTypeCode(type);
        return code != TypeCode.Object;
    }

    /// <inheritdoc />
    public override bool Write(object? value, Type type, PresetSize? presetSize = null)
    {
        if (type.IsEnum)
        {
            type = Enum.GetUnderlyingType(type);
            value = value == null ? 0 : Convert.ChangeType(value, type);
        }
        if (value == null && type != typeof(string)) return false;
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
                Write(unchecked((byte)(sbyte)temp));
                return true;

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

            case TypeCode.DateTime:
                Write(((DateTime)temp).Ticks);
                return true;

            case TypeCode.Object:
            default:
                break;
        }

        return false;
    }

    /// <inheritdoc />
    public override bool TryRead(Type type, ref object? value, PresetSize? presetSize = null)
    {
        var isEnum = type.IsEnum;
        if (isEnum)
            type = Enum.GetUnderlyingType(type);

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
                value = unchecked((sbyte)ReadByte());
                return true;

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
                value = new DateTime(ReadLong());
                return true;

            case TypeCode.Object:
            default:
                break;
        }

        if (isEnum && value != null)
            value = Enum.ToObject(type, value);

        return false;
    }

    #region 写基元数据

    // 按字节大小端写入，使用Span<byte>避免堆分配
    private void WriteBytes(Span<byte> bytes)
    {
        if (bytes.IsEmpty)
            return;
        if (Serializer.IsLittleEndian != BitConverter.IsLittleEndian)
            bytes.Reverse();
        Serializer.Write(bytes);
    }

    private void Write(byte value) => Serializer.Write(value);

    private void Write(bool value) => Serializer.Write((byte)(value ? 1 : 0));

    private void Write(char value) => Write(Convert.ToByte(value));

    private void Write(ushort value) { Span<byte> b = stackalloc byte[2]; BitConverter.TryWriteBytes(b, value); WriteBytes(b); }

    private void Write(short value) { Span<byte> b = stackalloc byte[2]; BitConverter.TryWriteBytes(b, value); WriteBytes(b); }

    private void Write(uint value) { Span<byte> b = stackalloc byte[4]; BitConverter.TryWriteBytes(b, value); WriteBytes(b); }

    private void Write(int value) { Span<byte> b = stackalloc byte[4]; BitConverter.TryWriteBytes(b, value); WriteBytes(b); }

    private void Write(ulong value) { Span<byte> b = stackalloc byte[8]; BitConverter.TryWriteBytes(b, value); WriteBytes(b); }

    private void Write(long value) { Span<byte> b = stackalloc byte[8]; BitConverter.TryWriteBytes(b, value); WriteBytes(b); }

    private void Write(float value) { Span<byte> b = stackalloc byte[4]; BitConverter.TryWriteBytes(b, value); WriteBytes(b); }

    private void Write(double value) { Span<byte> b = stackalloc byte[8]; BitConverter.TryWriteBytes(b, value); WriteBytes(b); }

    private void Write(decimal value)
    {
        var data = decimal.GetBits(value);
        foreach (var d in data) Write(d);
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

    // 按字节大小端读取到stackalloc缓冲区，避免堆分配
    private void ReadBytes(Span<byte> buffer)
    {
        Serializer.Read(buffer);
        if (Serializer.IsLittleEndian != BitConverter.IsLittleEndian)
            buffer.Reverse();
    }

    private byte ReadByte() => Serializer.Read();

    private bool ReadBool() => Serializer.Read() > 0;

    private char ReadChar() => Convert.ToChar(ReadByte());

    private ushort ReadUshort() { Span<byte> b = stackalloc byte[2]; ReadBytes(b); return BitConverter.ToUInt16(b); }

    private short ReadShort() { Span<byte> b = stackalloc byte[2]; ReadBytes(b); return BitConverter.ToInt16(b); }

    private uint ReadUInt() { Span<byte> b = stackalloc byte[4]; ReadBytes(b); return BitConverter.ToUInt32(b); }

    private int ReadInt() { Span<byte> b = stackalloc byte[4]; ReadBytes(b); return BitConverter.ToInt32(b); }

    private ulong ReadULong() { Span<byte> b = stackalloc byte[8]; ReadBytes(b); return BitConverter.ToUInt64(b); }

    private long ReadLong() { Span<byte> b = stackalloc byte[8]; ReadBytes(b); return BitConverter.ToInt64(b); }

    private float ReadFloat() { Span<byte> b = stackalloc byte[4]; ReadBytes(b); return BitConverter.ToSingle(b); }

    private double ReadDouble() { Span<byte> b = stackalloc byte[8]; ReadBytes(b); return BitConverter.ToDouble(b); }

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
        if (presetLength <= 0)
            return string.Empty;
        Span<byte> buffer = presetLength <= 256 ? stackalloc byte[presetLength] : new byte[presetLength];
        Serializer.Read(buffer);
        return Serializer.Encoding.GetString(buffer);
    }

    #endregion 读基元类型
}
