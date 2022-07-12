namespace GeneticAlgorithm.Models;
public class KnapsackItem
{
    public string Label { get; set; } = null!;

    public double Price { get; set; }

    public double Weight { get; set; }

    public double PriceToWeightRatio => Price / Weight;
}
