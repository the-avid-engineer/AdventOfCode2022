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
            .Select(ulong.Parse);

        var operation = GetOperation(line3["  Operation: new = old ".Length..]);
        var divisor = GetTest(line4["  Test: ".Length..]);

        var trueMonkeyIndex = int.Parse(line5["    If true: throw to monkey ".Length..]);
        var falseMonkeyIndex = int.Parse(line6["    If false: throw to monkey ".Length..]);

        var monkey = new Monkey
        {
            ItemWorryLevels = new Queue<ulong>(startingItemWorryLevels),
            Operation = operation,
            Divisor = divisor,
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

static Func<ulong, ulong> GetOperation(string operation)
{
    var @operator = operation.Substring(0, 1);
    var operand = operation.Substring(2);

    return @operator switch
    {
        "*" => operand switch
        {
            "old" => (old) => old * old,
            _ => (old) => old * uint.Parse(operand),
        },
        "+" => operand switch
        {
            _ => (old) => old + uint.Parse(operand)
        },
        _ => throw new NotImplementedException()
    };
}

static uint GetTest(string test)
{
    if (test.StartsWith("divisible by "))
    {
        var divisor = uint.Parse(test.Substring("divisible by ".Length));

        return divisor;
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

        game.WorryLevelCap *= Monkey.Divisor;

        return game;
    }
}

public class Game : IState
{
    public Dictionary<int, Monkey> Monkeys { get; set; } = new();
    public ulong WorryLevelCap { get; set; } = 1;

    private void PrintMonkeys()
    {
        foreach (var (monkeyIndex, monkey) in Monkeys)
        {
            Console.WriteLine($"Monkey {monkeyIndex}: {string.Join(", ", monkey.ItemWorryLevels.Select(i => i.ToString()))}");
        }

        Console.WriteLine();
    }

    private void PrintInspected()
    {
        foreach (var (monkeyIndex, monkey) in Monkeys)
        {
            Console.WriteLine($"Monkey {monkeyIndex} inspected items {monkey.ItemsInspected} times");
        }

        Console.WriteLine();
    }

    public string ToAnswer()
    {
        var monkeyIndices = Monkeys.Keys.Order();

        var numRounds = 10000;

        for (var round = 1; round <= numRounds; round++)
        {
            foreach (var monkeyIndex in monkeyIndices)
            {
                var monkey = Monkeys[monkeyIndex];

                while (monkey.ItemWorryLevels.TryDequeue(out var itemWorryLevel))
                {
                    var oldItemWorryLevel = itemWorryLevel;

                    // Monkey Inspects Item
                    itemWorryLevel = monkey.Operation(itemWorryLevel);
                    monkey.ItemsInspected += 1;

                    // Relief decreases because the monkey didn't break it
                    //itemWorryLevel /= 3;
                    // JK

                    // Apply Cap as Optimization
                    // Capping at this level preserves indiviudal modulo checks
                    itemWorryLevel = itemWorryLevel % WorryLevelCap;

                    if (itemWorryLevel % monkey.Divisor == 0)
                    {
                        Monkeys[monkey.TrueMonkeyIndex].ItemWorryLevels.Enqueue(itemWorryLevel);
                    }
                    else
                    {
                        Monkeys[monkey.FalseMonkeyIndex].ItemWorryLevels.Enqueue(itemWorryLevel);
                    }
                }
            }

            if (round % 1000 == 0 || round == 1 || round == 20)
            {
                Console.WriteLine($"== After Round {round} ==");
                PrintInspected();
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
    public required Queue<ulong> ItemWorryLevels { get; init; }
    public required Func<ulong, ulong> Operation { get; init; }
    public required uint Divisor { get; init; }
    public required int TrueMonkeyIndex { get; init; }
    public required int FalseMonkeyIndex { get; init; }

    public ulong ItemsInspected { get; set; } = 0;
}