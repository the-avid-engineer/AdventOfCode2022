using Microsoft.Extensions.Configuration;

public static class InputUtility<TProgram>
{
    public static async Task<FileStream> GetInputStream(int day)
    {
        const string FileName = "Input.txt";

        if (File.Exists(FileName))
        {
            return File.OpenRead(FileName);
        }

        var configuration = new ConfigurationBuilder()
            .AddUserSecrets(typeof(TProgram).Assembly)
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
}