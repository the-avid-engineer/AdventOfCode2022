using System;

await SolverUtility<Program>.LogAnswer(
    11,

    new Game(),

    (fileReader) => {
        var line1 = fileReader.ReadLine() ?? "";
        var line2 = fileReader.ReadLine() ?? "";
        var line3 = fileReader.ReadLine() ?? "";
        var line4 = fileReader.ReadLine() ?? "";
        var line5 = fileReader.ReadLine() ?? "";
        var line6 = fileReader.ReadLine() ?? "";
        fileReader.ReadLine();

        var monkeyIndex = int.Parse(line1["Monkey ".Length..^1]);

        var startingItemWorryLevels = line2["  Starting items: ".Length..]
            .Split(",", StringSplitOptions.TrimEntries)
            .Select(int.Parse);

        var operation = GetOperation(line3["  Operation: new = old ".Length..]);
        var test = GetTest(line4["  Test: ".Length..]);

        var trueMonkeyIndex = int.Parse(line5["    If true: throw to monkey ".Length..]);
        var falseMonkeyIndex = int.Parse(line6["    If false: throw to monkey ".Length..]);

        var monkey = new Monkey
        {
            ItemWorryLevels = new Queue<int>(startingItemWorryLevels),
            Operation = operation,
            Test = test,
            TrueMonkeyIndex = trueMonkeyIndex,
            FalseMonkeyIndex = falseMonkeyIndex,
        };

        return new AddMonkey
        {
            MonkeyIndex = monkeyIndex,
            Monkey = monkey,
        };
    }
);

static Func<int, int> GetOperation(string operation)
{
    var @operator = operation.Substring(0, 1);
    var operand = operation.Substring(2);

    return @operator switch
    {
        "*" => operand switch
        {
            "old" => (old) => old * old,
            _ => (old) => old * int.Parse(operand),
        },
        "+" => operand switch
        {
            _ => (old) => old + int.Parse(operand)
        },
        _ => throw new NotImplementedException()
    };
}

static Func<int, bool> GetTest(string test)
{
    if (test.StartsWith("divisible by "))
    {
        var divisor = int.Parse(test.Substring("divisible by ".Length));

        return (worry) => worry % divisor == 0;
    }

    throw new NotImplementedException();
}

public class AddMonkey : IInstruction<Game>
{
    public required int MonkeyIndex { get; init; }
    public required Monkey Monkey { get; init; }

    public Game Reduce(Game game)
    {
        game.Monkeys.Add(MonkeyIndex, Monkey);
        
        return game;
    }
}

public class Game : IState
{
    public int WorryLevel { get; set; }
    public Dictionary<int, Monkey> Monkeys { get; set; } = new();

    private void PrintMonkeys()
    {
        foreach (var (monkeyIndex, monkey) in Monkeys)
        {
            Console.WriteLine($"Monkey {monkeyIndex}: {string.Join(", ", monkey.ItemWorryLevels.Select(i => i.ToString()))}");
        }

        Console.WriteLine();
    }

    public string ToAnswer()
    {
        var monkeyIndices = Monkeys.Keys.Order();

        var numRounds = 20;

        for (var i = 1; i <= numRounds; i++)
        {
            foreach (var monkeyIndex in monkeyIndices)
            {
                var monkey = Monkeys[monkeyIndex];

                while (monkey.ItemWorryLevels.TryDequeue(out var itemWorryLevel))
                {
                    // Monkey Inspects Item
                    itemWorryLevel = monkey.Operation(itemWorryLevel);
                    monkey.ItemsInspected += 1;

                    // Relief decreases because the monkey didn't break it
                    itemWorryLevel /= 3;

                    if (monkey.Test(itemWorryLevel))
                    {
                        Monkeys[monkey.TrueMonkeyIndex].ItemWorryLevels.Enqueue(itemWorryLevel);
                    }
                    else
                    {
                        Monkeys[monkey.FalseMonkeyIndex].ItemWorryLevels.Enqueue(itemWorryLevel);
                    }
                }
            }
        }

        var topMonkeys = Monkeys.Values
            .OrderByDescending(monkey => monkey.ItemsInspected)
            .Take(2)
            .ToArray();

        return (topMonkeys[0].ItemsInspected * topMonkeys[1].ItemsInspected).ToString();
    }
}

public class Monkey
{
    public required Queue<int> ItemWorryLevels { get; init; }
    public required Func<int, int> Operation { get; init; }
    public required Func<int, bool> Test { get; init; }
    public required int TrueMonkeyIndex { get; init; }
    public required int FalseMonkeyIndex { get; init; }

    public int ItemsInspected { get; set; } = 0;
}