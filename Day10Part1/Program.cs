await SolverUtility<Program>.LogAnswer(
    10,

    new Device(),

    (fileReader) => {
        var line = fileReader.ReadLine() ?? "";

        var components = line.Split(" ");

        switch (components[0])
        {
            case "noop":
                return new NoOp();

            case "addx":
                return new AddX
                {
                    Arg = int.Parse(components[1]),
                };

            default:
                throw new NotImplementedException();
        }
    }
);

public class NoOp : IInstruction<Device>
{
    public Device Reduce(Device device)
    {
        device.NoOp();

        return device;
    }
}

public class AddX : IInstruction<Device>
{
    public required int Arg { get; init; }

    public Device Reduce(Device device)
    {
        device.AddX(Arg);

        return device;
    }
}

public class Device : IState
{
    private List<Register> HistoryX { get; set; }

    public Register X { get; set; }

    public Device()
    {
        X = new(1);
        HistoryX = new()
        {
            X
        };
    }

    public void NoOp()
    {
        HistoryX.Add(X);
    }

    public void AddX(int value)
    {
        HistoryX.Add(X);

        X = new(X.Value + value);

        HistoryX.Add(X);
    }

    public string ToAnswer()
    {
        var importantCycles = new[] { 20, 60, 100, 140, 180, 220 };

        var cycleHistory = importantCycles
            .Select(cycle => HistoryX.ElementAt(cycle - 1).Value * cycle);

        return cycleHistory.Sum().ToString();
    }
}

public record Register(int Value);