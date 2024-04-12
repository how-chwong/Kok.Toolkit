namespace Kok.Toolkit.Core.Extension;

/// <summary>
/// 程序集扩展
/// </summary>
public static class AssemblyExtension
{
    /// <summary>
    /// 程序集名称
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static string Name(this Assembly assembly)
        => assembly.GetName().Name ?? string.Empty;

    /// <summary>
    /// 程序集版本
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static string Version(this Assembly assembly)
        => assembly.GetName().Version?.ToString() ?? string.Empty;

    /// <summary>
    /// 程序集版本
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static string[] Versions(this Assembly assembly)
        => assembly.GetName().Version?.ToString().Split('.') ?? Array.Empty<string>();

    /// <summary>
    /// 程序集标题
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static string Title(this Assembly assembly)
        => assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? string.Empty;

    /// <summary>
    /// 程序集文件版本
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static string FileVersion(this Assembly assembly)
        => assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? string.Empty;

    /// <summary>
    /// 程序集所属公司
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static string Company(this Assembly assembly)
        => assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? string.Empty;

    /// <summary>
    /// 程序集所在路径
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static string FilePath(this Assembly assembly)
        => assembly.Location;

    /// <summary>
    /// 程序集编译时间
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static DateTime CompileTime(this Assembly assembly)
        => File.GetLastWriteTime(assembly.Location);
}
