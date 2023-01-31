using System.Globalization;
using CsvHelper;

namespace GilGoblin.Services;

public static class CsvInteractor<T> where T : class
{
    public static IEnumerable<T> LoadFile(string path)
    {
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records = csv.GetRecords<T>();
        return records;
    }
}
