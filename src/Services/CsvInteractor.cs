using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace GilGoblin.Services;

public static class CsvInteractor<T> where T : class
{
    public static IEnumerable<T> LoadFile(string path)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true };
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, config);
        try
        {
            var records = csv.GetRecords<T>().ToList();
            return records;
        }
        catch (Exception)
        {
            return Array.Empty<T>();
        }
    }
}
