using System.Collections;
using System.Diagnostics;
using GeneticAlgorithm;
using GeneticAlgorithm.Models;
using ScottPlot;
using Utils;

// ReSharper disable InvalidXmlDocComment

const string pathPrefix = "Files/dataset";
const string pathSuffix = "";

var availableDataSetFiles = new List<double>() { 5, 10, 15, 20, 25, 30 };

Console.WriteLine("===== Execution Started =====");
Console.WriteLine();

var plotYValues = availableDataSetFiles
                  .Select(availableDataSet => RunBruteForceForDataSet($"{pathPrefix}{availableDataSet}").TotalSeconds)
                  .ToList();

var plt = new ScottPlot.Plot(1600, 900);
plt.AddScatter(availableDataSetFiles.ToArray(), plotYValues.ToArray(), lineStyle: LineStyle.DashDotDot, label: "Brute Force Execution Time (Seconds)");
plt.SaveFig($"../../../../Images/BruteForce/BruteForceExecutionTime.png");

Console.WriteLine();
Console.WriteLine("===== Execution Ended =====");

// ===================================================

TimeSpan BruteForceKnapsack(IEnumerable<KnapsackItem> availableItems, double maxWeight)
{
    var itemsList = availableItems.AsList()!;
    var qtyOfItems = itemsList.Count;
    var combinationsQty = Math.Pow(2, qtyOfItems);


    var bestValue = 0.0;
    var finalWeight = 0.0;
    long solution = 0;

    var stopwatch = new Stopwatch();
    stopwatch.Start();
    for(long i = 0; i < combinationsQty; i++)
    {
        var currentWeight = 0.0;
        var currentValue = 0.0;

        for(var j = 0; j < qtyOfItems; j++)
        {
            var bitArray = new BitArray(BitConverter.GetBytes(i));

            if(!bitArray[j]) continue;

            currentWeight += itemsList[j].Weight;
            currentValue += itemsList[j].Price;

        }

        if(currentWeight <= maxWeight && currentValue > bestValue)
        {
            bestValue = currentValue;
            finalWeight = currentWeight;
            solution = i;
        }

    }
    stopwatch.Stop();

    Console.WriteLine("");
    Console.WriteLine("");
    Console.WriteLine($"Best Value found was: {bestValue}");
    Console.WriteLine($"Solution Binary: {Convert.ToString(solution, 2).PadLeft(qtyOfItems, '0')} - ({solution})");
    for(var i = 0; i < qtyOfItems; i++)
        if(new BitArray(BitConverter.GetBytes(solution))[i])
            Console.WriteLine($" => Label: {itemsList[i].Label} - Price: {itemsList[i].Price} - Weight: {itemsList[i].Weight}");
    Console.WriteLine($"Weight of best sack: {finalWeight}");
    Console.WriteLine($"Execution Elapsed Time: {stopwatch.Elapsed.TotalSeconds} seconds");
    Console.WriteLine("");
    Console.ForegroundColor = ConsoleColor.DarkBlue;
    Console.WriteLine(" ================================= ");
    Console.ResetColor();
    Console.WriteLine("");

    return stopwatch.Elapsed;
}

TimeSpan RunBruteForceForDataSet(string path)
{
    /** ===========================================
     *  Reading data from File
        ===========================================*/
    var (items, knapsackMaxWeight) = KnapsackDataReader.GetKnapsackData(path);

    if(items is null)
        throw new($"Error, '{nameof(items)}' cannot be null");

    /** =========================================== */
    Console.ForegroundColor = ConsoleColor.DarkRed;
    Console.WriteLine($"-> DataSet Name: {path}");
    Console.ResetColor();
    return BruteForceKnapsack(items, knapsackMaxWeight);
}