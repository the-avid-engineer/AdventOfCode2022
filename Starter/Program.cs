using Utility;

using var file = await InputUtility<Program>.GetInputStream(0);
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