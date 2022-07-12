using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils;

public interface CandidateProbability
{
    double Probability { get; }
}

public class Candidate<T> : CandidateProbability
{
    public double Probability { get; set; }

    public T? Value { get; set; }
}

public static class ChoseAlgorithms
{
    private class CandidateOption<T>
        where T: CandidateProbability
    {
        public T? Candidate { get; set; }

        public double AccumulatedProbability { get; set; }
    }

    public static IEnumerable<T> Roullete<T>(IEnumerable<T> candidates, int qtyToChose)
        where T : CandidateProbability
    {
        var rnd = new Random();
        var cadidatesList = candidates.AsList() ?? new();
        var sum = 0.0;
        var selectedCandidates = new List<T>();

        var valuesToChose = cadidatesList.Select(candidate =>
        {
            var candOption = new CandidateOption<T>()
            {
                Candidate = candidate,
                AccumulatedProbability = sum
            };
            sum += candidate.Probability;
            return candOption;
        });

        for(var i = 0; i < qtyToChose; i++)
        {
            var rndValue = rnd.NextDouble() * sum;

            var selected = valuesToChose.First(x => x.AccumulatedProbability >= rndValue);

            selectedCandidates.Add(selected.Candidate!);
        }

        return selectedCandidates;
    }
}
