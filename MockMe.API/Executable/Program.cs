/*
 * MockMe.CMD Console
class Program
{
    static void Main(string[] args)
    {
        const string APP_DATA = "App_Data";
        var fileName = (args.Length == 0) ? "hello world.txt" : $"{args[0]}.txt";
        var filePath = Path.Combine(APP_DATA, fileName);

        Console.WriteLine("Hello World -- Start");
        Directory.CreateDirectory(APP_DATA);
        File.WriteAllText(filePath, $"Hello World {DateTime.Now}");
        Console.WriteLine($"file path: {filePath}");
        Console.WriteLine("Hello World -- End");
    }
}
*/