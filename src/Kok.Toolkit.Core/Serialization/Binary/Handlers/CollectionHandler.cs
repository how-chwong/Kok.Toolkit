using Kok.Toolkit.Core.Extension;

namespace Kok.Toolkit.Core.Serialization.Binary.Handlers;

/// <summary>
/// 集合对象序列化处理器
/// </summary>
public class CollectionHandler : BinaryBaseHandler
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="serializer"></param>
    public CollectionHandler(BinarySerializer serializer) : base(serializer)
    {
    }

    /// <inheritdoc />
    public override bool Write(object? value, Type type, PresetSize? presetSize = null)
    {
        if (Type.GetTypeCode(type) != TypeCode.Object)
            return false;
        if (type.IsDictionary())
            return false;
        if (!type.IsCollection())
            return false;
        if (type.IsArray && type != typeof(byte[]))
            throw new Exception("数组类型仅支持byte[]");
        if (type == typeof(byte[]))
        {
            var temp = value == null ? Array.Empty<byte>() : (byte[])value;
            var handler = Serializer.GetHandler<GeneralHandler>();
            if (presetSize == null || !presetSize.HasPresetSize())
            {
                Serializer.WriteItemCount(temp.Length);
            }
            else
            {
                //给定了字节长度限制，不管value有没有值都要写入固定数量的字节,如果value长度超过给定值则抛出异常
                var byteLength = temp.Length == 0 ? 0 : temp.Length;
                if (byteLength < presetSize.Value)
                {
                    for (var i = 0; i < presetSize.Value - byteLength; i++)
                        Serializer.Write((byte)0);
                }

                if (temp is { Length: > 0 } && temp.Length > presetSize.Value)
                    throw new Exception($"字节数组长度{temp.Length}超出了其预设大小{presetSize.Value}");
            }
            //按实际value长度写入字节
            if (temp is { Length: > 0 })
                Array.ForEach(temp, b => handler.Write(b, typeof(byte)));
            return true;
        }

        if (value == null)
        {
            if (presetSize != null && presetSize.HasPresetSize())
            {
                if (presetSize.Value > 0)
                    throw new Exception($"类型为{type.Name}的集合指定了预设大小，但未赋值");
                return true;
            }

            Serializer.WriteItemCount(0);
            return true;
        }

        if (value is not IList list)
            return false;

        if (presetSize == null || !presetSize.HasPresetSize())
        {
            Serializer.WriteItemCount(list.Count);
        }
        else
        {
            if (presetSize.Type == PresetSizeType.SubItemCount && list.Count != presetSize.Value)
                throw new Exception($"{type.Name}实例给定数量{list.Count}与预设项目数量{presetSize.Value}不符");
        }
        var startBytes = Serializer.StreamPosition;
        foreach (var item in list)
            Serializer.Write(item, item.GetType());

        var byteCount = Serializer.StreamPosition - startBytes;
        if (presetSize is { Type: PresetSizeType.ByteLength } && byteCount != presetSize.Value)
            throw new ArgumentOutOfRangeException(type.Name, $"{type.Name}实例给定字节数{byteCount}与预设字节数量{presetSize.Value}不符");

        return true;
    }

    /// <inheritdoc />
    public override bool TryRead(Type type, ref object? value, PresetSize? presetSize = null)
    {
        if (Type.GetTypeCode(type) != TypeCode.Object)
            return false;
        if (!type.IsCollection())
            return false;
        if (type.IsDictionary())
            return false;
        if (type.IsArray && type != typeof(byte[]))
            throw new Exception("数组类型仅支持byte[]");

        if (type == typeof(byte[]))
        {
            int presetSizeValue;
            if (presetSize == null || !presetSize.HasPresetSize())
                presetSizeValue = Serializer.TryReadItemCount();
            else
                presetSizeValue = presetSize.Value;
            value = Serializer.Read(presetSizeValue);
            return true;
        }

        value ??= Activator.CreateInstance(type);
        if (value is not IList list)
            return false;
        var genericType = GetGenericType(type);
        var start = Serializer.StreamPosition;
        if (presetSize == null || !presetSize.HasPresetSize())
            presetSize = new PresetSize(PresetSizeType.SubItemCount, Serializer.TryReadItemCount());

        switch (presetSize.Type)
        {
            case PresetSizeType.ByteLength:
                for (var i = 0;
                     i < 10240 && Serializer.StreamPosition - start < presetSize.Value;
                     i++) //此处固定上限最多读取10240次，防止死循环
                {
                    var item = Activator.CreateInstance(genericType);
                    Serializer.TryRead(genericType, ref item);
                    list.Add(item);
                }
                break;

            case PresetSizeType.SubItemCount:
                for (var i = 0; i < presetSize.Value; i++)
                {
                    var item = Activator.CreateInstance(genericType);
                    Serializer.TryRead(genericType, ref item);
                    list.Add(item);
                }
                break;

            default:
                throw new Exception($"集合无法确认大小,Type:{type.Name}");
        }
        return true;
    }

    private static Type GetGenericType(Type type)
    {
        var types = type.GetGenericType();
        if (types.IsEmpty())
            throw new Exception($"不支持非泛型列表集合,非法类型值type={type.FullName}");
        if (types.Length > 1)
            throw new Exception($"仅支持一元泛型列表集合，非法类型值type:{type.FullName}");
        return types[0];
    }

    //todo:实现基元类型数组处理
}
