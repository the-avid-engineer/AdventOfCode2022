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

    private void Cycle()
    {
        HistoryX.Add(X);
    }

    public void NoOp()
    {
        Cycle();
    }

    public void AddX(int value)
    {
        Cycle();

        X = new(X.Value + value);

        Cycle();
    }

    public string ToAnswer()
    {
        var rowLength = 40;

        for (var row = 1; row <= HistoryX.Count / rowLength; row++)
        {
            var offset = (row - 1) * rowLength;
            var rowPositions = Enumerable.Range(0, rowLength);

            foreach (var rowPosition in rowPositions)
            {
                var cycle = rowPosition + offset;
                var offsetX = HistoryX[cycle].Value - rowPosition;

                if (offsetX is -1 or 0 or +1)
                {
                    Console.Write("#");
                }
                else
                {
                    Console.Write(".");
                }
            }

            Console.WriteLine();
        }

        return "Check Console CTR Emulator";
    }
}

public record Register(int Value);