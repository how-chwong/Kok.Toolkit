using Kok.Toolkit.Core.Log;
using System.Text;

namespace Kok.Test.ConsoleLog
{
    internal class Program
    {
        private static void Main()
        {
            Console.WriteLine("Hello, Kok!");
            Console.WriteLine("This is a test project for ConsoleLog");
            //Console.WriteLine("clear default logger");
            //Tracker.ClearLogger();
            //Console.WriteLine("add a console logger");
            //Tracker.AddLogger(new Toolkit.Core.Log.ConsoleLog());
            Tracker.WriteInfo("start make one log per 100 millisecond");
            StartOut();
            StartOut();
            StartOut();
            StartOut();
            StartOut();
            StartOut();
            Console.ReadLine();
        }

        private static void StartOut()
        {
            var sb = new StringBuilder(1024);
            for (var i = 0; i < 1024; i++)
            {
                sb.Append("a");
            }
            Task.Run(() =>
            {
                while (true)
                {
                    Tracker.WriteInfo($"【{Thread.CurrentThread.ManagedThreadId}】this is a info log:{sb}");
                    Thread.Sleep(100);
                    Tracker.WriteDebug($"【{Thread.CurrentThread.ManagedThreadId}】this is a debug log:{sb}");
                    Thread.Sleep(100);
                    Tracker.WriteWarn($"【{Thread.CurrentThread.ManagedThreadId}】this is a warm log:{sb}");
                    Thread.Sleep(100);
                    Tracker.WriteError($"【{Thread.CurrentThread.ManagedThreadId}】this is a error log:{sb}");
                    Thread.Sleep(100);
                    Tracker.WriteFatal($"【{Thread.CurrentThread.ManagedThreadId}】this is a fatal log:{sb}");
                    Thread.Sleep(100);
                }
            });
        }
    }
}
