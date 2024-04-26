namespace Kok.Test.WpfDemo.services;

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
