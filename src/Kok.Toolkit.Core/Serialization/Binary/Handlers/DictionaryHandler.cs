using Kok.Toolkit.Core.Extension;

namespace Kok.Toolkit.Core.Serialization.Binary.Handlers;

/// <summary>
/// 字典数据处理器
/// </summary>
public class DictionaryHandler : BinaryBaseHandler
{
    /// <inheritdoc />
    public DictionaryHandler(BinarySerializer serializer) : base(serializer)
    {
    }

    /// <inheritdoc />
    public override bool Write(object? value, Type type, PresetSize? presetSize = null)
    {
        if (!type.IsDictionary())
            return false;
        if (value == null) throw new Exception("字典类型值不能为空");
        if (value is not IDictionary dictionary)
            return false;

        var kvType = GetGenericArguments(type);
        var keyType = kvType[0];
        var valueType = kvType[1];

        if (presetSize == null || !presetSize.HasPresetSize())
            Serializer.WriteItemCount(dictionary.Count);
        foreach (DictionaryEntry item in dictionary)
        {
            Serializer.Write(item.Key, keyType);
            Serializer.Write(item.Value, valueType);
        }
        return true;
    }

    /// <inheritdoc />
    public override bool TryRead(Type type, ref object? value, PresetSize? presetSize = null)
    {
        if (!type.IsDictionary())
            return false;

        var kvType = GetGenericArguments(type);
        var keyType = kvType[0];
        var valueType = kvType[1];

        //没人会在协议包里定义字典的吧，这里就不判断长度特性了，使用按无特性处理，先读取长度再读取数据
        var count = Serializer.TryReadItemCount();
        if (count == 0)
            return true;
        value ??= Activator.CreateInstance(type);
        if (value is not IDictionary dictionary)
            return true;
        for (var i = 0; i < count; i++)
        {
            object? key = null;
            object? val = null;
            if (!Serializer.TryRead(keyType, ref key))
                return false;
            if (!Serializer.TryRead(valueType, ref val))
                return false;
            dictionary[key!] = val;
        }

        return true;
    }

    /// <summary>
    /// 获取字典内的泛型类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static Type[] GetGenericArguments(Type type)
    {
        var kvType = type.GetGenericArguments();
        if (kvType.Length != 2)
            throw new NotSupportedException($"字典类型仅支持{typeof(Dictionary<,>).FullName}");
        return kvType; //(kvType[0], kvType[1]);
    }
}