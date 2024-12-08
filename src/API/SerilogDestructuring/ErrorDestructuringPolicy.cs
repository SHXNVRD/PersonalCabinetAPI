using System.Diagnostics.CodeAnalysis;
using FluentResults;
using Serilog.Core;
using Serilog.Events;

namespace API.SerilogDestructuring;

public class ErrorDestructuringPolicy : IDestructuringPolicy
{
    public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, 
        [NotNullWhen(true)] out LogEventPropertyValue? result)
    {
        if (value is not List<IError> errors)
        {
            result = null;
            return false;
        }

        result = new SequenceValue(errors.Select(e => new ScalarValue(e.Message)));

        return true;
    }
}