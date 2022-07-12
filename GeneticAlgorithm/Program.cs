// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Text.Json;
using GeneticAlgorithm;
using GeneticAlgorithm.Models;
using ScottPlot;
using Utils;

Console.WriteLine("Execution Started");

const int popSize = 1000;

const int knapsackMaxWeight = 10;


var knapsackData = KnapsackDataReader.GetKnapsackData("Files/test-set1.txt");
var knapsackItems = knapsackData.items
                                .OrderBy(x => x.PriceToWeightRatio)
                                .Select((x, i) => (index: i, item: x))
                                .ToDictionary(x => x.index, x => x.item);
if(knapsackItems is null)
    throw new($"Error, '{nameof(knapsackItems)}' cannot be null");

foreach(var knapsackItem in knapsackItems)
    Console.WriteLine(JsonSerializer.Serialize(knapsackItem.Value));

IEnumerable<(int index, KnapsackItem item)> GetSackItems(Individual ind) =>
    ind.Chromosome
       .Select((inSack, i) => ((bool)inSack) ? (index: i, item: knapsackItems![i + 1]) : default);

void ApplyWeightRestriction(Individual ind)
{
    var weights = GetSackItems(ind)
                  .Where(x => x != default)
                  .AsList()!;
    var totalWeight = weights.Sum(x => x.item.Weight);

    if(totalWeight > knapsackMaxWeight)
    {
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

var individuals = Population.GenerateInitialPopulation(popSize, new()
{
    ChromosomeSize = knapsackItems.Count,
    // TODO: Generate initial population
    //GenerateRandomValue = 
});


var genetic = new GeneticAlgorithm.GeneticAlgorithm(individuals)
{
    EvaluationFunc = (x) => x.Chromosome.Select((inSack, i) => ((bool)inSack) ? knapsackItems[i + 1].Price : 0).Sum(),
    CrossOverFunc = (parent1, parent2) =>
    {
        var rnd = new Random();
        var mask = Enumerable.Range(0, parent1.Chromosome.Count).Select(_ => rnd.Next(0, 1));

        var childs = parent1.Chromosome.Zip(parent2.Chromosome, (p1, p2) => rnd.NextDouble() < 0.5
                                                                                ? (p1, p2)
                                                                                : (p2, p1)
            ).AsList()!;

        var child1 = new Individual() { Chromosome = childs.Select(x => x.Item1).ToList() };
        var child2 = new Individual() { Chromosome = childs.Select(x => x.Item1).ToList() };

        ApplyWeightRestriction(child1);
        ApplyWeightRestriction(child2);

        return (child1, child2);
    }
};


var bestFound = false;

for(var i = 0; i < 1000; i++)
{
    bestFound = genetic.GenerateNewPopulation();

    if(new List<int> { 10, 100, 200, 500, 1000, 10000 }.Contains(i + 1))
    {
        var dataX = genetic.Individuals.Select(x => x.Chromosome[0]).ToArray();
        var dataY = genetic.Individuals.Select(x => x.Chromosome[1]).ToArray();
        var plt = new ScottPlot.Plot(400, 300);
        plt.AddScatter(dataX, dataY, lineStyle: LineStyle.None);
        plt.SaveFig($"../../../../Images/Iteration-{i + 1}.png");
    }

    if(bestFound)
        break;
}

if(bestFound)
{
    Console.WriteLine($"Result Found: {genetic.CalcIndividual(genetic.Best)}");
    Console.WriteLine($"X Values=> X[0]: {genetic.Best.Chromosome[0]} - X[1]:{genetic.Best.Chromosome[1]}");
}
else
{
    Console.WriteLine($"Result Found: {genetic.CalcIndividual(genetic.Best)}");
    Console.WriteLine($"X Values=> X[0]: {genetic.Best.Chromosome[0]} - X[1]:{genetic.Best.Chromosome[1]}");
    Console.WriteLine($"Result was not within {errorRange} margin of error");
}