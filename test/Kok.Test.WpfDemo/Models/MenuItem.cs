namespace Kok.Test.WpfDemo.Models;

public sealed record MenuItem(string Title, ViewType type);

public enum ViewType
{
    Dialog,
    Message
}
