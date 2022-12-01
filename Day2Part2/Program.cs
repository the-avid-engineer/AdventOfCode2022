using var file = File.OpenRead("Input.txt");
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
    var line = streamReader.ReadLine() ?? "";

    //TODO: Parse the line

    return default;
}