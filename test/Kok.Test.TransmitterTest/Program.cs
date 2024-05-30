using Kok.Toolkit.Core.Communication.Transceiver;
using Kok.Toolkit.Core.Extension;
using System.Net;

namespace Kok.Test.TransmitterTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, Transmitter Tester!");

            var transceiver1 = new Transceiver<MyTelegram>();
            //创建发报机构建器
            var builder = TransmitterBuilder<MyTelegram>.CreateCyclical(new List<TargetEndPoint>()
            {
                new("收发器1", new IPEndPoint(IPAddress.Loopback, 8090), null)
            }, 1000, MakeHelloText, "收发器1");
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

        //处理收到的报文
        private static void OnGotTelegram(Packet arg1, object? arg2)
            => Console.WriteLine($"收到来自{arg1.SourceAddress}:{arg1.SourcePort}的报文,{arg1.Data.ToString(" ", string.Empty, "{0:X}")}");

        //生成报文
        private static List<MyTelegram> MakeHelloText(object? arg)
            => new() { new MyTelegram(1, "my name is t1") };
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
