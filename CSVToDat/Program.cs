// See https://aka.ms/new-console-template for more information
using System.IO;
using System.Text;
using GeneticAlgorithm;


var path = $"Files/dataset180";

var (items, knapsackMaxWeight) = KnapsackDataReader.GetKnapsackData(path);
var itemsList = items.ToList();

var sb = new StringBuilder();

sb.AppendLine($"n={itemsList.Count()};");
sb.AppendLine($"v=[{string.Join(",", itemsList.Select(x => x.Price))}];");
sb.AppendLine($"p=[{string.Join(",", itemsList.Select(x => x.Weight))}];");
sb.AppendLine($"c={knapsackMaxWeight};");


var text = sb.ToString();
Console.WriteLine(text);
