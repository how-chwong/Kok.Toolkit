using Kok.Toolkit.Core.Communication.Transceiver;
using Kok.Toolkit.Core.Extension;
using Kok.Toolkit.Core.Log;
using Kok.Toolkit.Core.Timers;
using System.Net;

namespace Kok.Test.TransmitterTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello, Transmitter Tester!");
            Tracker.AddLogger<ExternalOutputLog>(new ExternalOutputLog(OutFunc));
            Tracker.WriteVersion("Kok");
            var transceiver1 = new Transceiver<MyTelegram>(TimerType.Multimedia);
            //创建发报机构建器
            var builder = TransmitterBuilder<MyTelegram>.CreateCyclical(new List<TargetEndPoint>()
            {
                new("收发器1", new IPEndPoint(IPAddress.Loopback, 8090), null)
            }, 200, MakeHelloText, "收发器1", null, OnSentTelegram);
            //设置发报机
            transceiver1.SetTransmitter(builder);
            //设置收报机
            transceiver1.SetReceiver(OnGotTelegram);
            //启动收发器
            if (transceiver1.Start("127.0.0.1", 8081, "收发器1"))
                Console.WriteLine("成功启动收发器");

            var receiver = new Transceiver<MyTelegram>();
            receiver.SetReceiver(OnGotTelegram);
            if (receiver.Start("127.0.0.1", 8090, "接收器"))
                Console.WriteLine("接收器启动成功");

            Console.ReadLine();

            transceiver1.Stop();
            receiver.Stop();
        }

        private static Task OutFunc(string arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        private static void OnSentTelegram(IReadOnlyCollection<byte> arg1, int arg2, DateTime arg3, EndPoint arg4,
            object? arg5)
            => Console.WriteLine($"{arg3:HH:mm:ss.fff}    发送数据");

        //处理收到的报文
        private static void OnGotTelegram(Packet arg1, object? arg2)
            => Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}  收到来自{arg1.SourceAddress}:{arg1.SourcePort}的报文,{arg1.Data.ToString("{0:X}")}");

        //生成报文
        private static MyTelegram MakeHelloText(object? arg)
            => new MyTelegram(1, "my name is t1");
    }

    public class MyTelegram
    {
        public int Id { get; set; }

        public string Content { get; set; } = string.Empty;

        public MyTelegram()
        {
        }

        public MyTelegram(int id, string content)
        {
            Id = id;
            Content = content;
        }
    }
}
