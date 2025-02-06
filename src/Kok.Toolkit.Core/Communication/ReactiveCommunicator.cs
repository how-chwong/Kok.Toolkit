using Kok.Toolkit.Core.Communication.Transceiver;
using Kok.Toolkit.Core.Extension;
using Kok.Toolkit.Core.Log;
using Kok.Toolkit.Core.Net;
using System.Net.Sockets;

namespace Kok.Toolkit.Core.Communication;

/// <summary>
/// 响应式通信器
/// 通信器在收到预期报文后立即发送特定响应报文
/// </summary>
public class ReactiveCommunicator
{
    /// <summary>
    /// 构造一个通信实例
    /// </summary>
    /// <param name="receiveAction">收到报文后的处理函数</param>
    /// <param name="generateAck">生成回应报文的函数</param>
    public ReactiveCommunicator(Func<Packet, bool> receiveAction, Func<byte[]> generateAck)
    {
        _receiveAction = receiveAction;
        _generateAck = generateAck;
    }

    private UdpClient? _udpClient;

    /// <summary>
    /// 开启通信器
    /// </summary>
    /// <param name="localIp">本地</param>
    /// <param name="localPort"></param>
    /// <returns></returns>
    public (bool result, string error) Start(string localIp, int localPort)
    {
        if (!Network.TryParseEndPoint(localIp, localPort, out var localEndPoint) || localEndPoint == null) return (false, "本地网络地址非法");
        if (!Network.IsLocalIp(localIp)) return (false, $"{localIp}:{localPort}不是本地网络地址");
        if (localEndPoint.CheckPort(NetworkProtocol.Udp)) return (false, $"{localIp}:{localPort}已被占用");

        try
        {
            _udpClient = new UdpClient(localEndPoint);
            _udpClient.SetIOControl();
            _udpClient.BeginReceive(Receive, null);
            _isReceiverStopped = false;
            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            Stop();
            return (false, ex.Message);
        }
    }

    private bool _isReceiverStopped;

    private readonly Func<Packet, bool> _receiveAction;
    private readonly Func<byte[]> _generateAck;

    private void Receive(IAsyncResult result)
    {
        try
        {
            if (_udpClient == null) return;
            IPEndPoint? src = null;
            var buf = _udpClient.EndReceive(result, ref src);
            if (_isReceiverStopped) return;
            if (!_receiveAction.Invoke(new Packet(DateTime.Now, src?.Address.ToString() ?? string.Empty, src?.Port ?? 0, buf))) return;
            var data = _generateAck.Invoke();
            if (data is { Length: > 0 }) _udpClient.Send(data, data.Length, src);
        }
        catch (Exception ex)
        {
            Tracker.WriteWarn($"报文处理异常：{ex}");
        }
        finally
        {
            if (!_isReceiverStopped)
                _udpClient?.BeginReceive(Receive, null);
        }
    }

    /// <summary>
    /// 发送报文
    /// </summary>
    /// <param name="message"></param>
    /// <param name="ip"></param>
    /// <returns></returns>
    public (bool result, string error) Send(byte[] message, IPEndPoint ip)
    {
        try
        {
            _udpClient?.Send(message, message.Length, ip);
            return (true, string.Empty);
        }
        catch (Exception e)
        {
            return (false, e.Message);
        }
    }

    /// <summary>
    /// 停止报文
    /// </summary>
    public void Stop()
    {
        _isReceiverStopped = true;
        _udpClient?.Close();
        _udpClient?.Dispose();
    }
}
