using System.Net.NetworkInformation;

namespace Kok.Toolkit.Core.Net;

/// <summary>
/// 网络工具
/// </summary>
public static class Network
{
    /// <summary>
    /// 尝试将一个字符串和数字转换为终结点
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    /// <param name="endPoint"></param>
    /// <returns></returns>
    public static bool TryParseEndPoint(string ip, int port, out IPEndPoint? endPoint)
    {
        endPoint = default;
        if (string.IsNullOrWhiteSpace(ip))
            return false;
        if (!IPAddress.TryParse(ip.Trim(), out var address))
            return false;
        endPoint = new IPEndPoint(address, port);
        return true;
    }

    /// <summary>
    /// 检查端口是否被占用
    /// </summary>
    /// <param name="endPoint"></param>
    /// <param name="protocol"></param>
    /// <returns></returns>
    public static bool CheckPort(this IPEndPoint endPoint, NetworkProtocol protocol)
    {
        var gp = IPGlobalProperties.GetIPGlobalProperties();
        var eps = protocol switch
        {
            NetworkProtocol.Tcp => gp.GetActiveTcpListeners(),
            NetworkProtocol.Udp => gp.GetActiveUdpListeners(),
            _ => null
        };
        return eps != null && eps.Any(t => t.Port == endPoint.Port && t.Address.Equals(endPoint.Address));
    }

    private static List<IPAddress> s_localIps = new();

    /// <summary>
    /// 本机所有网络地址
    /// </summary>
    public static List<IPAddress> LocalIps
    {
        get
        {
            if (s_localIps?.Count > 0)
                return s_localIps;
            s_localIps = GetLocalIps().ToList();
            return s_localIps;
        }
    }

    /// <summary>
    /// 获取本机所有在用网络地址
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<IPAddress> GetLocalIps()
    {
        var result = new List<IPAddress>();
        foreach (var item in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (item.OperationalStatus != OperationalStatus.Up)
                continue;
            if (item.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                continue;

            var property = item.GetIPProperties();
            if (property.UnicastAddresses.Count <= 0)
                continue;
            result.AddRange(property.UnicastAddresses.Select(info => info.Address));
        }

        return result;
    }

    /// <summary>
    /// 指定地址是否为本地IP
    /// </summary>
    /// <param name="ip"></param>
    /// <returns></returns>
    public static bool IsLocalIp(string ip)
    {
        if (string.IsNullOrWhiteSpace(ip))
            return false;
        if (!IPAddress.TryParse(ip, out var address))
            return false;
        return ip.Equals("127.0.0.1") || LocalIps.Contains(address);
    }

    /// <summary>
    /// 校验字符是否符合IP规则
    /// </summary>
    /// <param name="ip"></param>
    /// <returns></returns>
    public static bool CheckIp(string ip) => !string.IsNullOrWhiteSpace(ip) && IPAddress.TryParse(ip, out _);
}

/// <summary>
/// 网络协议
/// </summary>
public enum NetworkProtocol
{
    /// <summary>
    /// UDP
    /// </summary>
    Udp,

    /// <summary>
    /// TCP
    /// </summary>
    Tcp,

    /// <summary>
    /// HTTP
    /// </summary>
    Http
}