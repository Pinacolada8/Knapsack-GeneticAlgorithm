//using Plotly.NET;
//using Plotly.NET.LayoutObjects;
//using Microsoft.FSharp.Core;
//using Utils;

//namespace NeuralNetwork.ChartUtils;
//public class ChartUtils
//{
//    public static void PlotNeuralNetwork(Perceptron perceptron, string title, IEnumerable<Perceptron.NNResultStatistics>? testResults = null)
//    {
//        var testResultsList = testResults?.AsList() ?? new List<Perceptron.NNResultStatistics>();

//        var errorHistory = perceptron.SquaredErrorHistory;
//        var xValues = errorHistory.Select(x => x.Key);
//        var yValues = errorHistory.Select(x => x.Value);

//        var xAxis = new LinearAxis();
//        xAxis.SetValue("title", "Iteration");
//        xAxis.SetValue("showgrid", false);
//        xAxis.SetValue("showline", true);

//        var yAxis = new LinearAxis();
//        yAxis.SetValue("title", "Squared Error");
//        yAxis.SetValue("showgrid", false);
//        yAxis.SetValue("showline", true);

//        var layout = new Layout();
//        layout.SetValue("xaxis", xAxis);
//        layout.SetValue("yaxis", yAxis);
//        layout.SetValue("showlegend", true);
//        layout.SetValue("width", 1280);
//        layout.SetValue("height", 720);

//        var titleObj = new Title();
//        titleObj.SetValue("text", title);
//        titleObj.SetValue("font", new { color = "Gray", size = 40 });
//        layout.SetValue("title", titleObj);

//        var trace = new Trace("scatter");
//        trace.SetValue("x", xValues);
//        trace.SetValue("y", yValues);
//        trace.SetValue("mode", "lines");
//        trace.SetValue("name", title);

//        var description = new ChartDescription(
//            "<b style='font-size:30px;'>Neural Network</b>",
//            $"<b style='font-size:20px;'>Accuracy:</b> {testResultsList.Average(x => x.CorrectResult ? 1 : 0)}"            +
//            $"<br/>"                                                                               +
//            $"<br/>"                                                                               +
//            $"Weights: { perceptron.Weights.ToString().Replace("\n", "<br/>").Replace(" ", "_") }" +
//            $"<br/>"                                                                               +
//            $"Bias: {perceptron.Bias.ToString().Replace("\n", "<br/>").Replace(" ", "_")}");

//        var chart = GenericChart
//                    .ofTraceObject(true, trace)
//                    .WithLayout(layout)
//                    .WithDescription(description);

//        //chart.SaveSVG("out/chart");
//        chart.Show();

//    }
//}
