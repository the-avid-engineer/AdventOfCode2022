using Microsoft.Extensions.Configuration;

using var file = await GetInputStream(3);
using var fileReader = new StreamReader(file);

int answer = 0;

while (!fileReader.EndOfStream)
{
    var entry = GetEntry(fileReader);

    var first = entry[..(entry.Length / 2)];

    var second = entry[(entry.Length / 2)..];

    foreach (var @char in first)
    {
        if (second.IndexOf(@char) > -1)
        {
            int x;

            if ($"{@char}" == $"{@char}".ToLowerInvariant())
            {
                x = (@char - 'a') + 1;
            }
            else
            {
                x = (@char - 'A') + 1 + 26;
            }

            Console.WriteLine($"{@char}: {x}");

            answer += x;
            break;
        }
    }
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