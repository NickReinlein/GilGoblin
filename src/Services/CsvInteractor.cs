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
        try
        {
            var absolutePath = Path.GetFullPath(path);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true };
            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, config);
            return csv.GetRecords<T>().ToList();
        }
        catch (Exception e)
        {
            _logger.LogError($"Error reading from CSV of type {nameof(T)}: {e.Message}");
            return new List<T>();
        }
    }
}