using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Kok.Toolkit.Core.Extension;

/// <summary>
/// UDPClient扩展
/// </summary>
public static class UdpClientExtension
{
    /// <summary>
    /// 设置Window下的IOControl
    /// </summary>
    /// <param name="udpClient"></param>
    public static void SetIOControl(this UdpClient udpClient)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

        const uint IOC_IN = 0x80000000;
        const uint IOC_VENDOR = 0x18000000;
        var SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
        udpClient.Client.IOControl((int)SIO_UDP_CONNRESET, new[] { Convert.ToByte(false) }, null);
    }
}
