using System;
using Microsoft.Extensions.Configuration;

using var file = await GetInputStream(3);
using var fileReader = new StreamReader(file);

var groups = new List<Group>()
{ new Group() };

while (!fileReader.EndOfStream)
{
    var entry = GetEntry(fileReader);

    var lastGroup = groups.Last();

    if (lastGroup.Rucksacks.Count == 3)
    {
        lastGroup = new Group();

        groups.Add(lastGroup);
    }

    lastGroup.Rucksacks.Add(entry);
}

var answer = 0;

foreach (var group in groups)
{
    answer += group.GetPriority();
}

Console.WriteLine($"Answer: {answer}");

static string GetEntry(StreamReader streamReader)
{
    var line = streamReader.ReadLine() ?? "";

    return line;
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

public class Group
{
    public List<string> Rucksacks { get; init; } = new();

    public int GetPriority()
    {
        var @char = GetChar();

        if ($"{@char}" == $"{@char}".ToLowerInvariant())
        {
            return (@char - 'a') + 1;
        }
        else
        {
            return (@char - 'A') + 1 + 26;
        }
    }

    private char GetChar()
    {
        return Rucksacks[0].Intersect(Rucksacks[1]).Intersect(Rucksacks[2]).Single();
    }
}