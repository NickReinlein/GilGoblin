using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Services;

public interface ICsvInteractor
{
    List<T> LoadFile<T>(string path);
}

public class CsvInteractor : ICsvInteractor
{
    private ILogger<CsvInteractor> _logger;

    public CsvInteractor(ILogger<CsvInteractor> logger)
    {
        _logger = logger;
    }

    public List<T> LoadFile<T>(string tableName)
    {
        try
        {
            var filePath = GetResourceFilePath(tableName);
            var absolutePath = Path.GetFullPath(filePath);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true };
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, config);
            var list = csv.GetRecords<T>().ToList();
            return list;
        }
        catch (Exception e)
        {
            _logger.LogError($"Error reading from CSV of type {nameof(T)}: {e.Message}");
            return new List<T>();
        }
    }

    private string GetResourceFilePath(string filename) =>
        Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../resources/"), filename);
}