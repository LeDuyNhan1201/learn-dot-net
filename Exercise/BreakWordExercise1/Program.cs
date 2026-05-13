using System.Text.RegularExpressions;

namespace BreakWordExercise1;

class Program
{
    private static async Task Main(string[] args)
    {
        var projectRoot = Directory.GetParent(AppContext.BaseDirectory)!
            .Parent!
            .Parent!
            .Parent!
            .FullName;

        var inputFile = Path.Combine(projectRoot, "input.txt");
        var outputFile = Path.Combine(projectRoot, "output.txt");

        if (!File.Exists(inputFile))
        {
            Console.WriteLine("Input file not found");
            return;
        }

        var lines = await File.ReadAllLinesAsync(inputFile);

        var results = lines.Select(BreakWords);

        await File.WriteAllLinesAsync(outputFile, results);

        Console.WriteLine("Done!");
    }

    private static string BreakWords(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;
        
        // TryHard -> Try Hard
        var result = Regex.Replace(input, @"([a-z])([A-Z])", "$1 $2");

        // DotNet10 -> Dot Net 10
        result = Regex.Replace(result, @"([A-Za-z])(\d)", "$1 $2");

        // 5000th -> 5000 th
        result = Regex.Replace(result, @"(\d)([A-Za-z])", "$1 $2");
        return result.Trim();
    }
}