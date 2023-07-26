using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;

namespace GilGoblin.Services;

public static class CsvInteractor<T>
    where T : class
{
    public static IEnumerable<T> LoadFile(string path)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true };
        try
        {
            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, config);
            return csv.GetRecords<T>().ToList();
        }
        catch (Exception)
        {
            return Array.Empty<T>();
        }
    }
}
