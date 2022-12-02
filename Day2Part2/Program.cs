using Microsoft.Extensions.Configuration;

using var file = await GetInputStream(2);
using var fileReader = new StreamReader(file);

object answer = "Answer Not Set";

while (!fileReader.EndOfStream)
{
    var entry = GetEntry(fileReader);

    //TODO: Process the entry
}

Console.WriteLine($"Answer: {answer}");

// Change object? to whatever is needed
static object? GetEntry(StreamReader streamReader)
{
    //var character = (char)streamReader.Read();
    var line = streamReader.ReadLine() ?? "";

    //TODO: Parse the line

    return default;
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