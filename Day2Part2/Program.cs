using System.Diagnostics;
using Microsoft.Extensions.Configuration;

using var file = await GetInputStream(2);
using var fileReader = new StreamReader(file);

int answer = 0;

while (!fileReader.EndOfStream)
{
    var entry = GetEntry(fileReader);

    //Console.WriteLine(entry);

    answer += entry.Item3;
}

Console.WriteLine($"Answer: {answer}");

// Change object? to whatever is needed
static (IPlay, IPlay, int) GetEntry(StreamReader streamReader)
{
    var line = streamReader.ReadLine() ?? "";

    var plays = line.Split(' ');

    if (plays.Length != 2)
    {
        throw new Exception();
    }

    var opponentPlays = plays[0] switch
    {
        "A" => Rock.Instance,
        "B" => Paper.Instance,
        "C" => Scissors.Instance,
        _ => throw new UnreachableException()
    };

    var respondWithPlay = plays[1] switch
    {
        "X" => opponentPlays.LosingPlay,
        "Y" => opponentPlays,
        "Z" => opponentPlays.WinningPlay,
        _ => throw new UnreachableException()
    };

    var points = respondWithPlay.PointsForPlay;

    if (opponentPlays.WinningPlay == respondWithPlay)
    {
        points += 6;
    }
    else if (opponentPlays == respondWithPlay)
    {
        points += 3;
    }
    else
    {
        points += 0;
    }

    return (opponentPlays, respondWithPlay, points);
}

static async Task<FileStream> GetInputStream(int day)
{
    const string FileName = "Input.txt";

    if (File.Exists(FileName))
    {
        return File.OpenRead(FileName);
    }

    var configuration = new ConfigurationBuilder()
        .AddUserSecrets(typeof(Program).Assembly)
        .Build();

    var sessionCookie = configuration["SessionCookie"];

    var httpClient = new HttpClient();

    httpClient.DefaultRequestHeaders.Add("Cookie", $"session={sessionCookie}");

    var inputResponse = await httpClient.GetAsync($"https://adventofcode.com/2022/day/{day}/input");

    inputResponse.EnsureSuccessStatusCode();

    var inputContentStream = await inputResponse.Content.ReadAsStreamAsync();

    var file = File.Create(FileName);

    inputContentStream.CopyTo(file);

    file.Seek(0, SeekOrigin.Begin);

    return file;
}

public interface IPlay
{
    public IPlay WinningPlay { get; }
    public IPlay LosingPlay { get; }
    public int PointsForPlay { get; }
}

public class Rock : IPlay
{
    public static IPlay Instance = new Rock();
    public IPlay WinningPlay => Paper.Instance;
    public IPlay LosingPlay => Scissors.Instance;
    public int PointsForPlay => 1;
}

public class Paper : IPlay
{
    public static IPlay Instance = new Paper();
    public IPlay WinningPlay => Scissors.Instance;
    public IPlay LosingPlay => Rock.Instance;
    public int PointsForPlay => 2;
}

public class Scissors : IPlay
{
    public static IPlay Instance = new Scissors();
    public IPlay WinningPlay => Rock.Instance;
    public IPlay LosingPlay => Paper.Instance;
    public int PointsForPlay => 3;
}