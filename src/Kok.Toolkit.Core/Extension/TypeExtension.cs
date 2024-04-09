namespace Kok.Toolkit.Core.Extension;

/// <summary>
/// 类型扩展
/// </summary>
public static class TypeExtension
{
    /// <summary>
    /// 判断指定类型是否为集合
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsCollection(this Type type) => typeof(IEnumerable).IsAssignableFrom(type);

    /// <summary>
    /// 判断指定类型是否是字典
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsDictionary(this Type type) => typeof(IDictionary).IsAssignableFrom(type);

    /// <summary>
    /// 获取泛型的具体类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Type[] GetGenericType(this Type type)
    {
        if (type.IsGenericType)
            return type.GetGenericArguments();
        return type.BaseType is { IsGenericType: true } ? type.BaseType.GetGenericArguments() : Array.Empty<Type>();
    }

    /// <summary>
    /// 获取数组元素的类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Type? GetArrayElementType(this Type type)
    {
        if (!type.IsArray)
            return null;
        if (string.IsNullOrWhiteSpace(type.FullName))
            return null;
        var tName = type.FullName.Replace("[]", string.Empty);
        return type.Assembly.GetType(tName);
    }

    /// <summary>
    /// 判断属性是否具有指定的特性
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="member"></param>
    /// <returns></returns>
    public static bool HasAttribute<T>(this MemberInfo member) where T : Attribute
        => member.GetCustomAttribute<T>() != null;
}