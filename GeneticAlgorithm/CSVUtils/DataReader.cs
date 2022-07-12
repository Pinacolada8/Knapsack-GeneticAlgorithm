using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

// ReSharper disable once CheckNamespace
namespace CSVUtils;
public class DataReader
{
    public static readonly CsvConfiguration CSV_CONFIGURATION
        = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            Delimiter = ";"
        };

    public static IEnumerable<T> ReadData<T>(string filePath)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, CSV_CONFIGURATION);
        var models = csv.GetRecords<T>().ToList();
        return models;
    }
}
