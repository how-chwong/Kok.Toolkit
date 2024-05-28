namespace Kok.Test.WpfDemo.Services;

public interface IMyService
{
    string SayHello(string name);
}

public class MyService : IMyService
{
    public string SayHello(string name)
    {
        return $"Hello,{name}!this is a test demo";
    }
}
