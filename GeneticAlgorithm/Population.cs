using GeneticAlgorithm.Models;

namespace GeneticAlgorithm;

public class ChromosomeConfiguration
{
    public int ChromosomeSize { get; set; } = 2;
}

public class Population
{
    public static List<Individual> GenerateInitialPopulation(int popSize, ChromosomeConfiguration? chromosomeConfiguration = null,
                                                             Func<int, ChromosomeConfiguration, Individual>? indGenerator = null)
    {
        chromosomeConfiguration ??= new();
        indGenerator ??= (_, config) => new()
        {
            Chromosome = Enumerable.Range(0, config.ChromosomeSize)
                                       .Select(_ => (object)new Random().NextDouble())
                                       .ToList()
        };

        var population = new List<Individual>();

        for(var popIndex = 0; popIndex < popSize; popIndex++)
        {
            var individual = indGenerator(popIndex, chromosomeConfiguration);

            population.Add(individual);
        }

        return population;
    }
}
