namespace Tcp.Helpers.CheckBuilder;

public interface ICheckBuilder
{
    ICheckBuilder AddLine(string line);
    ICheckBuilder AddLineBreak();
    ICheckBuilder AddSeparator();
    ICheckBuilder AddIndent(int size = 2);
    string Build();
}