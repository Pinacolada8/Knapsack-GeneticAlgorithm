using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using GeneticAlgorithm.Models;

namespace GeneticAlgorithm;
public class KnapsackDataReader
{

    public static  (IEnumerable<KnapsackItem> items, double maxWeight) GetKnapsackData(string filePath)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = " ",
            WhiteSpaceChars = Array.Empty<char>(),
            HasHeaderRecord = false
        });

        var items = new List<KnapsackItem>();

        csv.Read();
        var qtyLeftToRead = csv.GetRecord<double>();

        while(csv.Read() && qtyLeftToRead > 0)
        {
            var record = csv.GetRecord<KnapsackItem>();
            items.Add(record);

            qtyLeftToRead--;
        }

        var maxWeight = csv.GetRecord<double>();
        
        return (items, maxWeight);
    }
}
