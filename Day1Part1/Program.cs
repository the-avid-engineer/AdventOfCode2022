using var file = File.OpenRead("Input.txt");
using var fileReader = new StreamReader(file);

var elves = new List<Elf>
{
    new Elf(),
};

while (!fileReader.EndOfStream)
{
    var entry = GetEntry(fileReader);

    if (entry.HasValue)
    {
        elves.Last().Entries.Add(entry.Value);
    }
    else
    {
        elves.Add(new Elf());
    }
}

Console.WriteLine(elves.MaxBy(elf => elf.TotalCalories)!.TotalCalories);

int? GetEntry(StreamReader streamReader)
{
    var line = streamReader.ReadLine() ?? "";

    if (line == "")
    {
        return null;
    }

    return int.Parse(line);
}

public class Elf
{
    public List<int> Entries { get; init; } = new List<int>();

    public int TotalCalories => Entries.Sum();
}