using TimeZoneConverter;

namespace TimezoneConverterExercise2;

class Program
{
    private const string ToZone = "America/New_York";
    
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

        var results = new List<string>();

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            // Format:
            // 2026-05-13 08:00:00 Asia/Ho_Chi_Minh

            var lastSpace = line.LastIndexOf(' ');

            if (lastSpace == -1)
                continue;

            var datePart = line[..lastSpace];
            var fromZone = line[(lastSpace + 1)..];

            if (!DateTime.TryParse(datePart, out DateTime dateTime))
            {
                results.Add($"Invalid datetime: {line}");
                continue;
            }

            try
            {
                var fromTz = TZConvert.GetTimeZoneInfo(fromZone);
                var toTz = TZConvert.GetTimeZoneInfo(ToZone);

                var sourceTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
                
                var utcTime = TimeZoneInfo.ConvertTimeToUtc(sourceTime, fromTz);

                var converted = TimeZoneInfo.ConvertTimeFromUtc(utcTime, toTz);

                results.Add($"{converted:yyyy-MM-dd HH:mm:ss} {ToZone}");
            }
            catch (Exception ex)
            {
                results.Add($"Error: {ex.Message}");
            }
        }

        await File.WriteAllLinesAsync(outputFile, results);

        Console.WriteLine("Done!");
    }
}