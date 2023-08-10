using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;

namespace GilGoblin.Services;

public interface ICsvInteractor
{
    List<T> LoadFile<T>(string path);
}

public class CsvInteractor : ICsvInteractor
{
    public List<T> LoadFile<T>(string path)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true };
        try
        {
            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, config);
            return csv.GetRecords<T>().ToList();
        }
        catch
        {
            return new List<T>();
        }
    }
}
