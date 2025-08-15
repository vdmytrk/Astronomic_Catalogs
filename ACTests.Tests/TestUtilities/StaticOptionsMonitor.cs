using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACTests.Tests.TestUtilities;

public class StaticOptionsMonitor<T> : IOptionsMonitor<T>
{
    private readonly T _currentValue;
    public StaticOptionsMonitor(T currentValue) => _currentValue = currentValue;

    public T CurrentValue => _currentValue;
    public T Get(string? name) => _currentValue;
    public IDisposable OnChange(Action<T, string> listener) => null!;
}
