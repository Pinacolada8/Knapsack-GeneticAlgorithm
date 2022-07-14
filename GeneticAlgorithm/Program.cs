// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using GeneticAlgorithm;
using GeneticAlgorithm.Models;
using ScottPlot;
using Utils;
// ReSharper disable InvalidXmlDocComment


const int popSize = 100;
const int maxIterations = 1000;
const string pathPrefix = "Files/dataset";

var availableDataSetFiles = new List<double>() { 5, 10, 15, 20, 25, 30 };

Console.WriteLine("===== Execution Started =====");
Console.WriteLine();

var plotYValues = availableDataSetFiles
                  .Select(availableDataSet => RunGeneticForDataSet($"{pathPrefix}{availableDataSet}").TotalSeconds)
                  .ToList();

var plt = new ScottPlot.Plot(1600, 900);
plt.Title("Genetic Execution Time (Seconds)");
plt.AddScatter(availableDataSetFiles.ToArray(), plotYValues.ToArray(), lineStyle: LineStyle.DashDotDot, label: "Genetic Execution Time (Seconds)");
plt.SaveFig("../../../../Images/Genetic/GeneticExecutionTime.png");

Console.WriteLine();
Console.WriteLine("===== Execution Ended =====");

// ====================================================

TimeSpan RunGeneticForDataSet(string path)
{
    /** ===========================================
     *  Reading data from File
        ===========================================*/
    var (items, knapsackMaxWeight) = KnapsackDataReader.GetKnapsackData(path);
    var knapsackItems = items.OrderBy(x => x.PriceToWeightRatio)
                             .Select((x, i) => (index: i, item: x))
                             .ToDictionary(x => x.index, x => x.item);

    if(knapsackItems is null)
        throw new($"Error, '{nameof(knapsackItems)}' cannot be null");

    var availableItemsQty = knapsackItems.Count;

    /** =========================================== */

    var initialPop = Population.GenerateInitialPopulation(
        popSize,
        new() { ChromosomeSize = knapsackItems.Count },
        (_, configuration) =>
        {
            var random = new Random();
            var randomChromosome = Enumerable.Range(0, configuration.ChromosomeSize)
                                        .Select(x => (object)(random.NextDouble() < 0.3))
                                        .ToList();
            var newInd = new Individual() { Chromosome = randomChromosome };

            ApplyWeightRestriction(newInd);

            return newInd;
        });


    var genetic = new GeneticAlgorithm.GeneticAlgorithm(initialPop)
    {
        MutationRate = 0.2,
        ElitismQty = 1,
        EvaluationFunc = (x) => GetSackItems(x).Sum(chrom => chrom.item.Price),
        CrossOverFunc = (parent1, parent2) =>
        {
            var rnd = new Random();
            var childs = parent1.Chromosome.Zip(parent2.Chromosome, (p1, p2) => rnd.NextDouble() < 0.5
                                                                                    ? (p1, p2)
                                                                                    : (p2, p1)
                ).AsList()!;

            var child1 = new Individual() { Chromosome = childs.Select(x => x.Item1).ToList() };
            var child2 = new Individual() { Chromosome = childs.Select(x => x.Item1).ToList() };

            ApplyWeightRestriction(child1);
            ApplyWeightRestriction(child2);

            return (child1, child2);
        },
        MutationFunc = (individual) =>
        {
            var rnd = new Random();
            individual.Chromosome = individual.Chromosome
                                              .Select((x, i) => rnd.NextDouble() < (i + 1) / (availableItemsQty * 100.0) ? !(bool)x : x)
                                              .ToList();
            ApplyWeightRestriction(individual);
            return individual;
        }
    };

    var plotXValues = new List<double>();
    var plotYValues = new List<double>();

    var stopwatch = new Stopwatch();
    stopwatch.Start();
    for(var i = 0; i < maxIterations; i++)
    {
        genetic.GenerateNewPopulation();

        plotXValues.Add(i + 1);
        plotYValues.Add(genetic.ProcessedIndividuals.First().Value);
    }
    stopwatch.Stop();

    var plt = new ScottPlot.Plot(1600, 900);
    plt.AddScatter(plotXValues.ToArray(), plotYValues.ToArray(), lineStyle: LineStyle.None, label: "Best Individual per iteration");
    plt.SaveFig($"../../../../Images/Genetic/{path.Split("/").Last()}-ValueProgression.png");

    var bestIndividual = genetic.ProcessedIndividuals.First();

    Console.WriteLine("");
    Console.WriteLine($"Best Value found was: {bestIndividual.Value}");
    var itemsInBestSack = GetSackItems(bestIndividual.Individual).OrderBy(x => Convert.ToInt32(x.item.Label)).AsList()!;
    foreach(var itemWithIndex in itemsInBestSack)
        Console.WriteLine($" => Label: {itemWithIndex.item.Label} - Price: {itemWithIndex.item.Price} - Weight: {itemWithIndex.item.Weight}");
    Console.WriteLine($"Weight of best sack: {itemsInBestSack.Sum(x => x.item.Weight)}");
    Console.WriteLine($"Execution Elapsed Time: {stopwatch.Elapsed.TotalSeconds} seconds");
    Console.WriteLine("");
    Console.ForegroundColor = ConsoleColor.DarkBlue;
    Console.WriteLine(" ================================= ");
    Console.ResetColor();
    Console.WriteLine("");

    return stopwatch.Elapsed;


    // ====================================================

    IEnumerable<(int index, KnapsackItem item)> GetSackItems(Individual ind) =>
        ind.Chromosome
           .Select((inSack, i) => ((bool)inSack) ? (index: i, item: knapsackItems![i]) : default)
           .Where(x => x != default);

    void ApplyWeightRestriction(Individual ind)
    {
        var weights = GetSackItems(ind)
                      .Where(x => x != default)
                      .AsList()!;
        var totalWeight = weights.Sum(x => x.item.Weight);

        if(totalWeight <= knapsackMaxWeight)
            return;

        var extraWeight = totalWeight - knapsackMaxWeight;
        var removedWeight = 0.0;
        var toRemoveItems = weights.TakeWhile(x =>
        {
            var decision = extraWeight > removedWeight;
            removedWeight += x.item.Weight;
            return decision;
        });

        foreach(var toRemoveItem in toRemoveItems)
            ind.Chromosome[toRemoveItem.index] = false;
    }
}


