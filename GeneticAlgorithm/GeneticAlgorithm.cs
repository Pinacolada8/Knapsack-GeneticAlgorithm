using System;
using GeneticAlgorithm.Models;
using Utils;

namespace GeneticAlgorithm;

public class ProcessedIndividual
{
    public Individual Individual { get; set; } = null!;

    public double Value { get; set; }

    public double Fitness { get; set; }
}

public class GeneticAlgorithm
{
    public Func<Individual, double> EvaluationFunc { get; init; } = (_) => 0;

    public Func<Individual, Individual, (Individual, Individual)> CrossOverFunc { get; init; } = (_, _) => new();

    public Func<Individual, Individual> MutationFunc { get; init; } = (_) => new();

    public double MutationRate { get; set; } = 0.2;

    public double ElitismQty { get; set; } = 1;

    public List<ProcessedIndividual> ProcessedIndividuals { get; private set; }

    public GeneticAlgorithm(IEnumerable<Individual> individuals)
    {
        ProcessedIndividuals = ProcessIndividuals(individuals).AsList()!;
    }

    // ================================================================================

    public IEnumerable<ProcessedIndividual> ProcessIndividuals(IEnumerable<Individual> individuals)
    {
        var processed = individuals.Select(x => new ProcessedIndividual()
        {
            Individual = x,
            Value = EvaluationFunc(x)
        }).AsList()!;

        return CalcFitness(processed).AsList()!;
    }

    public IEnumerable<ProcessedIndividual> CalcFitness(IEnumerable<ProcessedIndividual> individuals)
    {
        var indsList = individuals.AsList()!;

        var min = indsList.Min(x => x.Value);
        min = min >= 0 ? 0 : Math.Abs(min);

        indsList.ForEach(x => x.Fitness = x.Value + min);
        var result = indsList.OrderByDescending(x => x.Fitness);

        return result;
    }

    // ================================================================================

    public void GenerateNewPopulation()
    {
        var selectedIndividuals = SelectIndividuals(ProcessedIndividuals, ProcessedIndividuals.Count);

        var childs = CrossOver(selectedIndividuals);

        childs = Mutate(childs, MutationRate);

        // Elitism
        for(var i = 0; i < ElitismQty; i++)
            childs[i] = ProcessedIndividuals[i].Individual;

        ProcessedIndividuals = ProcessIndividuals(childs).AsList()!;
    }

    public List<ProcessedIndividual> SelectIndividuals(IEnumerable<ProcessedIndividual> individuals, int qty)
    {
        var rnd = new Random();
        var sum = 0.0;
        var indsWithProbability = individuals.Select(x =>
                                             {
                                                 var result = (accum: sum, ind: x);
                                                 sum += x.Fitness;
                                                 return result;
                                             })
                                             .ToList();

        sum -= indsWithProbability.Last().ind.Fitness;

        var selectedIndividuals = new List<ProcessedIndividual>();
        for(var i = 0; i < qty; i++)
        {
            var rndValue = rnd.NextDouble() * sum;

            var selected = indsWithProbability.First(x => x.accum >= rndValue);

            selectedIndividuals.Add(selected.ind);
        }

        return selectedIndividuals;
    }

    public List<Individual> CrossOver(List<ProcessedIndividual> individuals)
    {
        var childs = new List<Individual>();
        var halfSize = individuals.Count / 2;
        for(var i = 0; i < halfSize; i++)
        {
            var parent1 = individuals[i];
            var parent2 = individuals[i + halfSize];

            var crossOverResult = CrossOverFunc(parent1.Individual, parent2.Individual);
            childs.Add(crossOverResult.Item1);
            childs.Add(crossOverResult.Item2);
        }

        return childs;
    }

    public List<Individual> Mutate(List<Individual> individuals, double mutationRate)
    {
        var rnd = new Random();
        for(var i = 0; i < individuals.Count; i++)
            if(rnd.NextDouble() < mutationRate)
                individuals[i] = MutationFunc(individuals[i]);

        return individuals;
    }

}
