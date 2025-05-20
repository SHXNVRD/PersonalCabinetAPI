using System.Text;
using Microsoft.Extensions.Primitives;

namespace Tcp.Helpers.CheckBuilder;

public class CheckBuilder : ICheckBuilder
{
    private readonly StringBuilder _stringBuilder = new();
    private readonly string _lineSeparator;
    private readonly string _lineBreak;

    public CheckBuilder(CheckBuilderSettings? settings = null)
    {
        settings ??= new CheckBuilderSettings();
        _lineBreak = settings.LineBreak!;
        _lineSeparator = settings.LineSeparator!;
    }
    
    public ICheckBuilder AddLine(string line)
    {
        _stringBuilder.Append(line);
        return AddLineBreak();
    }

    public ICheckBuilder AddLineBreak()
    {
        _stringBuilder.Append(_lineBreak);
        return this;
    }

    public ICheckBuilder AddSeparator()
    {
        _stringBuilder.Append(_lineSeparator);
        return this;
    }

    public ICheckBuilder AddIndent(int size = 2)
    {
        for (int i = 0; i < size; i++)
            _stringBuilder.Append(_lineBreak);

        return this;
    }

    public string Build()
    {
        var check = _stringBuilder.ToString();
        _stringBuilder.Clear();
        return check;
    }
}