namespace GilGoblin.Tests.Database;

public static class DatabaseTests
{
    public static string GetTestDirectory() =>
        Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.Parent?.FullName
        ?? string.Empty;
}
