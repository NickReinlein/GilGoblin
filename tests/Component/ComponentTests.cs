using System.Text.Json;

namespace GilGoblin.Tests.Component;

public class ComponentTests : TestWithDatabase
{
    protected static readonly double MissingEntryPercentageThreshold = 0.85;

    protected static JsonSerializerOptions GetSerializerOptions() =>
        new() { PropertyNameCaseInsensitive = true, IncludeFields = true, };
}