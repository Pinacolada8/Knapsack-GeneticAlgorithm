using GeneticAlgorithm.Models;

namespace GeneticAlgorithm;

public class ChromosomeConfiguration
{
    public int ChromosomeSize { get; set; } = 2;
    public Func<int, ChromosomeConfiguration, List<object>> GenerateRandomValue { get; set; } = (_, config) 
        => Enumerable.Range(0, config.ChromosomeSize)
                     .Select(_ => (object)new Random().NextDouble())
                     .ToList();

}

public class Population
{
    public static List<Individual> GenerateInitialPopulation(int popSize, ChromosomeConfiguration? chromosomeConfiguration = null)
    {
        chromosomeConfiguration ??= new();
        var population = new List<Individual>();

        for(var popIndex = 0; popIndex < popSize; popIndex++)
        {
            var individual = new Individual()
            {
                Chromosome = chromosomeConfiguration.GenerateRandomValue(popIndex, chromosomeConfiguration)
            };
            
            population.Add(individual);
        }

        return population;
    }
}
