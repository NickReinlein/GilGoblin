using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
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

    public List<T> LoadFile<T>(string path)
    {
        var config =
            new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true, IgnoreBlankLines = false };
        try
        {
            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, config);
            var result = csv.GetRecords<T>().ToList();
            _logger.LogInformation($"Loaded {result.Count} entries from the csv using path {path}");
            return result;
        }
        catch
        {
            _logger.LogError($"Failed to get results from the CSV file using path: {path}");
            return new List<T>();
        }
    }
}