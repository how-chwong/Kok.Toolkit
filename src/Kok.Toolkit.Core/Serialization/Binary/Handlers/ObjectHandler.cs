using Kok.Toolkit.Core.Extension;
using Kok.Toolkit.Core.Serialization.Binary.Attributes;
using System.Collections.Concurrent;

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

    /// <inheritdoc />
    public override bool CanHandle(Type type) =>
        Type.GetTypeCode(type) == TypeCode.Object && !type.IsCollection() && !type.IsDictionary() && type != typeof(IPAddress);

    /// <inheritdoc cref="IBinaryHandler"/>
    public override bool Write(object? value, Type type, PresetSize? presetSize = null)
    {
        if (!CanHandle(type))
            return false;

        if (value == null)
            throw new Exception($"{type.Name}类实例不能为空");
        var propertyInfos = GetSortedProperties(type);
        foreach (var property in propertyInfos)
        {
            if (!property.CanWrite) continue;
            if (property.SetMethod != null && property.SetMethod.IsPrivate) continue;
            var ac = GetAttrCache(property);
            if (ac.HasBinaryIgnore) continue;
            if (ac.HasCrcStartByte)
                _crcStartByteLocations.Push((int)Serializer.StreamPosition);

            if (property.PropertyType.IsNumericType() && HasCrc16Attribute(ac, out var crc16))
            {
                Serializer.Write(crc16);
            }
            else if (property.PropertyType.IsNumericType() && HasCrc32Attribute(ac, out var crc32))
            {
                Serializer.Write(crc32);
            }
            else if (property.PropertyType.IsNumericType() && HasCrc8Attribute(ac, out var crc8))
            {
                Serializer.Write(crc8);
            }
            else if (property.PropertyType.IsNumericType() && HasFcsAttribute(value, type, ac, out var fcs))
            {
                Serializer.Write(fcs, property.PropertyType);
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
        if (!CanHandle(type))
            return false;

        var propertyInfos = GetSortedProperties(type);
        value ??= Activator.CreateInstance(type, true);

        foreach (var property in propertyInfos)
        {
            if (!property.CanWrite)
                continue;
            if (property.SetMethod != null && property.SetMethod.IsPrivate)
                continue;
            var ac = GetAttrCache(property);
            if (ac.HasBinaryIgnore)
                continue;
            var data = property.GetValue(value);
            if (!Serializer.TryRead(property.PropertyType, ref data, GetPresetSize(property, value, propertyInfos)))
                return false;
            property.SetValue(value, data);
        }

        return true;
    }

    #region 内部方法

    private static readonly ConcurrentDictionary<Type, List<PropertyInfo>> _propertyCache = new();
    private static readonly ConcurrentDictionary<PropertyInfo, PropertyAttributeCache> _attrCache = new();

    /// <summary>
    /// 属性特性缓存，避免重复反射查找
    /// </summary>
    private sealed class PropertyAttributeCache
    {
        public bool HasBinaryIgnore { get; init; }
        public bool HasCrcStartByte { get; init; }
        public CollectionItemCountAttribute? ItemCount { get; init; }
        public CollectionByteLengthAttribute? ByteLength { get; init; }
        public SlicesNumberAttribute? SlicesNumber { get; init; }
        public Crc8Attribute? Crc8 { get; init; }
        public Crc16Attribute? Crc16 { get; init; }
        public Crc32Attribute? Crc32 { get; init; }
        public FcsAttribute? Fcs { get; init; }
    }

    private static PropertyAttributeCache GetAttrCache(PropertyInfo property) =>
        _attrCache.GetOrAdd(property, p => new PropertyAttributeCache
        {
            HasBinaryIgnore = p.GetCustomAttribute<BinaryIgnoreAttribute>() != null,
            HasCrcStartByte = p.GetCustomAttribute<CrcStartByteAttribute>() != null,
            ItemCount = p.GetCustomAttribute<CollectionItemCountAttribute>(),
            ByteLength = p.GetCustomAttribute<CollectionByteLengthAttribute>(),
            SlicesNumber = p.GetCustomAttribute<SlicesNumberAttribute>(),
            Crc8 = p.GetCustomAttribute<Crc8Attribute>(),
            Crc16 = p.GetCustomAttribute<Crc16Attribute>(),
            Crc32 = p.GetCustomAttribute<Crc32Attribute>(),
            Fcs = p.GetCustomAttribute<FcsAttribute>(),
        });

    //按FieldOrderAttribute特性值升序排列该类型下的属性，如果未指定FieldOrder特性则将该属性放在最后
    private static List<PropertyInfo> GetSortedProperties(Type type) =>
        _propertyCache.GetOrAdd(type, t =>
            t.GetProperties()
             .OrderBy(p => p.GetCustomAttribute<FieldOrderAttribute>()?.Order ?? int.MaxValue)
             .ToList());

    /// <summary>
    /// 获取预设大小
    /// </summary>
    /// <param name="property"></param>
    /// <param name="value"></param>
    /// <param name="propertyInfos"></param>
    /// <returns></returns>
    private static PresetSize GetPresetSize(PropertyInfo property, object? value, List<PropertyInfo> propertyInfos)
    {
        var cache = GetAttrCache(property);

        if (cache.ItemCount != null)
        {
            if (string.IsNullOrWhiteSpace(cache.ItemCount.Path))
                return new PresetSize(PresetSizeType.SubItemCount, cache.ItemCount.Value);
            var itemCountProperty = propertyInfos.FirstOrDefault(p => p.Name.Equals(cache.ItemCount.Path)) ??
                                    throw new Exception($"指定的子项目数量路径{cache.ItemCount.Path}不存在");
            var v = itemCountProperty.GetValue(value);
            if (v == null || !int.TryParse(v.ToString(), out var size))
                throw new Exception($"子项目数量特性关联的{cache.ItemCount.Path}值读取失败");
            return new PresetSize(PresetSizeType.SubItemCount, size);
        }

        if (cache.ByteLength != null)
        {
            if (string.IsNullOrWhiteSpace(cache.ByteLength.Path))
                return new PresetSize(PresetSizeType.SubItemCount, cache.ByteLength.ByteLength);
            var byteLengthProperty = propertyInfos.FirstOrDefault(p => p.Name.Equals(cache.ByteLength.Path)) ?? throw new Exception("指定的字节长度路径不存在");
            var v = byteLengthProperty.GetValue(value);
            if (v == null || !int.TryParse(v.ToString(), out var size))
                throw new Exception("字节长度特性关联值读取失败");
            return new PresetSize(PresetSizeType.ByteLength, size);
        }

        if (cache.SlicesNumber != null)
        {
            int length = 0;
            foreach (var name in cache.SlicesNumber.Path)
            {
                var sliceProperty = propertyInfos.FirstOrDefault(p => p.Name.Equals(name)) ?? throw new Exception("指定的切片数量路径不存在");
                var sliceValue = sliceProperty.GetValue(value);
                length += (int?)sliceValue ?? throw new Exception("切片长度特性关联值读取失败");
            }
            return new PresetSize(PresetSizeType.SubItemCount, length);
        }
        return new PresetSize(PresetSizeType.None, -1);
    }

    private bool HasFcsAttribute(object? obj, Type type, PropertyAttributeCache cache, out object? value)
    {
        value = null;
        var atr = cache.Fcs;
        if (atr == null || string.IsNullOrWhiteSpace(atr.Algorithm)) return false;
        var method = type.GetMethod(atr.Algorithm);
        if (method == null || !method.IsPublic) throw new Exception($"类型{type.FullName}指定的FCS生成方法不存在或不可访问");
        var paramsInfo = method.GetParameters();
        if (paramsInfo.Length != 1 || paramsInfo[0].ParameterType != typeof(byte[]))
            throw new Exception($"类型{type.FullName}指定的FCS生成方法的参数只能为{typeof(byte[]).Name}");

        var data = Serializer.GetBytes();
        value = method.Invoke(obj, new object[] { data });
        return true;
    }

    private bool HasCrc8Attribute(PropertyAttributeCache cache, out byte crc)
    {
        crc = 0;
        var atr = cache.Crc8;
        if (atr == null)
            return false;
        var data = Serializer.GetBytes();
        var start = GetCrcStartByte();
        crc = atr.Compute(data, start, data.Length - start);
        return true;
    }

    private bool HasCrc16Attribute(PropertyAttributeCache cache, out ushort crc)
    {
        crc = 0;
        var atr = cache.Crc16;
        if (atr == null)
            return false;
        var data = Serializer.GetBytes();
        var start = GetCrcStartByte();
        crc = atr.Compute(data, start, data.Length - start);
        return true;
    }

    private bool HasCrc32Attribute(PropertyAttributeCache cache, out uint crc)
    {
        crc = 0;
        var atr = cache.Crc32;
        if (atr == null)
            return false;
        var data = Serializer.GetBytes();
        var start = GetCrcStartByte();
        crc = atr.Compute(data, start, data.Length - start);
        return true;
    }

    #endregion 内部方法
}
