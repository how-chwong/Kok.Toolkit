using Kok.Toolkit.Core.Extension;
using Kok.Toolkit.Core.Serialization.Binary.Attributes;

namespace Kok.Toolkit.Core.Serialization.Binary.Handlers;

/// <summary>
/// 对象（非集合对象）序列化处理器
/// </summary>
public class ObjectHandler : BinaryBaseHandler
{
    /// <inheritdoc />
    public ObjectHandler(BinarySerializer serializer) : base(serializer)
    {
    }

    //保存所有的crc起始字节位置
    private readonly Stack<int> _crcStartByteLocations = new();

    /// <summary>
    /// crc计算起始字节位置
    /// </summary>
    private int GetCrcStartByte() => _crcStartByteLocations.TryPop(out var b) ? b : 0;

    /// <inheritdoc cref="IBinaryHandler"/>
    public override bool Write(object? value, Type type, PresetSize? presetSize = null)
    {
        if (Type.GetTypeCode(type) != TypeCode.Object)
            return false;
        if (type.IsCollection())
            return false;

        if (value == null)
            throw new Exception($"{type.Name}类实例不能为空");
        var propertyInfos = GetSortedProperties(type);
        foreach (var property in propertyInfos)
        {
            if (!property.CanWrite)
                continue;
            if (property.SetMethod != null && property.SetMethod.IsPrivate)
                continue;
            if (property.HasAttribute<BinaryIgnoreAttribute>())
                continue;
            if (property.HasAttribute<CrcStartByteAttribute>())
                _crcStartByteLocations.Push((int)Serializer.StreamPosition);

            if (type.IsNumericType() && HasCrc16Attribute(property, out var crc16))
            {
                Serializer.Write(crc16);
            }
            else if (type.IsNumericType() && HasCrc32Attribute(property, out var crc32))
            {
                Serializer.Write(crc32);
            }
            else if (type.IsNumericType() && HasCrc8Attribute(property, out var crc8))
            {
                Serializer.Write(crc8);
            }
            else
            {
                if (!Serializer.Write(property.GetValue(value), property.PropertyType,
                        GetPresetSize(property, value, propertyInfos)))
                    return false;
            }
        }
        return true;
    }

    /// <inheritdoc />
    public override bool TryRead(Type type, ref object? value, PresetSize? presetSize = null)
    {
        if (Type.GetTypeCode(type) != TypeCode.Object)
            return false;
        if (type.IsCollection())
            return false;

        var propertyInfos = GetSortedProperties(type);
        value ??= Activator.CreateInstance(type, true);

        foreach (var property in propertyInfos)
        {
            if (!property.CanWrite)
                continue;
            if (property.SetMethod != null && property.SetMethod.IsPrivate)
                continue;
            if (property.HasAttribute<BinaryIgnoreAttribute>())
                continue;
            var data = property.GetValue(value);
            if (!Serializer.TryRead(property.PropertyType, ref data, GetPresetSize(property, value, propertyInfos)))
                return false;
            property.SetValue(value, data);
        }

        return true;
    }

    #region 内部方法

    //按FieldOrderAttribute特性值升序排列该类型下的属性，如果未指定FieldOrder特性则将该属性放在最后
    private static List<PropertyInfo> GetSortedProperties(Type type) =>
        type.GetProperties()
            .OrderBy(p => p.GetCustomAttribute<FieldOrderAttribute>()?.Order ?? int.MaxValue)
            .ToList();

    /// <summary>
    /// 获取预设大小
    /// </summary>
    /// <param name="property"></param>
    /// <param name="value"></param>
    /// <param name="propertyInfos"></param>
    /// <returns></returns>
    private static PresetSize GetPresetSize(PropertyInfo property, object? value, List<PropertyInfo> propertyInfos)
    {
        var itemCountAttr = property.GetCustomAttribute<CollectionItemCountAttribute>();
        if (itemCountAttr != null)
        {
            if (string.IsNullOrWhiteSpace(itemCountAttr.Path))
                return new PresetSize(PresetSizeType.SubItemCount, itemCountAttr.Value);
            var itemCountProperty = propertyInfos.FirstOrDefault(p => p.Name.Equals(itemCountAttr.Path));
            if (itemCountProperty == null)
                throw new Exception($"指定的子项目数量路径{itemCountAttr.Path}不存在");
            var v = itemCountProperty.GetValue(value);
            if (v == null || !int.TryParse(v.ToString(), out var size))
                throw new Exception($"子项目数量特性关联的{itemCountAttr.Path}值读取失败");
            return new PresetSize(PresetSizeType.SubItemCount, size);
        }

        var byteLengthAttr = property.GetCustomAttribute<CollectionByteLengthAttribute>();
        if (byteLengthAttr != null)
        {
            if (string.IsNullOrWhiteSpace(byteLengthAttr.Path))
                return new PresetSize(PresetSizeType.SubItemCount, byteLengthAttr.ByteLength);
            var byteLengthProperty = propertyInfos.FirstOrDefault(p => p.Name.Equals(byteLengthAttr.Path));
            if (byteLengthProperty == null)
                throw new Exception("指定的字节长度路径不存在");
            var v = byteLengthProperty.GetValue(value);
            if (v == null || !int.TryParse(v.ToString(), out var size))
                throw new Exception("字节长度特性关联值读取失败");
            return new PresetSize(PresetSizeType.ByteLength, size);
        }

        var sliceNumAttr = property.GetCustomAttribute<SlicesNumberAttribute>();
        if (sliceNumAttr != null)
        {
            int length = 0;
            foreach (var name in sliceNumAttr.Path)
            {
                var sliceProperty = propertyInfos.FirstOrDefault(p => p.Name.Equals(name));
                if (sliceProperty == null)
                    throw new Exception("指定的切片数量路径不存在");
                var sliceValue = sliceProperty.GetValue(value);
                length += (int?)sliceValue ?? throw new Exception("切片长度特性关联值读取失败");
            }
            return new PresetSize(PresetSizeType.SubItemCount, length);
        }
        return new PresetSize(PresetSizeType.None, -1);
    }

    private bool HasCrc8Attribute(MemberInfo member, out byte crc)
    {
        crc = 0;
        var atr = member.GetCustomAttribute<Crc8Attribute>();
        if (atr == null)
            return false;
        var data = Serializer.GetBytes();
        var start = GetCrcStartByte();
        crc = atr.Compute(data, start, data.Length - start);
        return true;
    }

    private bool HasCrc16Attribute(MemberInfo member, out ushort crc)
    {
        crc = 0;
        var atr = member.GetCustomAttribute<Crc16Attribute>();
        if (atr == null)
            return false;
        var data = Serializer.GetBytes();
        var start = GetCrcStartByte();
        crc = atr.Compute(data, start, data.Length - start);
        return true;
    }

    private bool HasCrc32Attribute(MemberInfo member, out uint crc)
    {
        crc = 0;
        var atr = member.GetCustomAttribute<Crc32Attribute>();
        if (atr == null)
            return false;
        var data = Serializer.GetBytes();
        var start = GetCrcStartByte();
        crc = atr.Compute(data, start, data.Length - start);
        return true;
    }

    #endregion 内部方法
}
