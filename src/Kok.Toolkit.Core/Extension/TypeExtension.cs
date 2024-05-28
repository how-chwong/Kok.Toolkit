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
    /// 判定是否为数值类型
    /// </summary>
    /// <param name="type">要判断的类型</param>
    /// <returns>是否为数值类型</returns>
    public static bool IsNumericType(this Type type)
    {
        var tc = Type.GetTypeCode(type);
        return (type is { IsPrimitive: true, IsValueType: true, IsEnum: false }
                && tc != TypeCode.Char
                && tc != TypeCode.Boolean)
               || tc == TypeCode.Decimal;
    }

    /// <summary>
    /// 判断是否为可空数值类型。
    /// </summary>
    /// <param name="t">要判断的类型</param>
    /// <returns>是否为可空数值类型</returns>
    public static bool IsNumericOrNullableNumericType(this Type t)
        => t.IsNumericType() || (t.IsNullableType() && t.GetGenericArguments()[0].IsNumericType());

    /// <summary>
    /// 判断是否为可空类型。
    /// 注意，直接调用可空对象的.GetType()方法返回的会是其泛型值的实际类型，用其进行此判断肯定返回false。
    /// </summary>
    /// <param name="t">要判断的类型</param>
    /// <returns>是否为可空类型</returns>
    public static bool IsNullableType(this Type t)
        => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>);

    /// <summary>
    /// 判断属性是否具有指定的特性
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="member"></param>
    /// <returns></returns>
    public static bool HasAttribute<T>(this MemberInfo member) where T : Attribute
        => member.GetCustomAttribute<T>() != null;
}
