await SolverUtility<Program>.LogAnswer(
    14,

    new CaveMap(),

    (fileReader) => {
        var line = fileReader.ReadLine() ?? "";

        var pairs = line.Split(" -> ");

        var scan = new Scan();

        foreach (var pair in pairs)
        {
            var coordinates = pair.Split(",", 2).Select(int.Parse).ToArray();

            var x = coordinates[0];
            var y = coordinates[1];

            scan.Path.Add((x, y));
        }

        return new AddScan
        {
            Scan = scan
        };
    }
);

public class AddScan : IInstruction<CaveMap>
{
    public required Scan Scan { get; init; }

    public CaveMap Reduce(CaveMap state)
    {
        state.Scans.Add(Scan);

        return state;
    }
}

public class CaveMap : IState
{
    public List<Scan> Scans { get; init; } = new();

    private Cave GetCave()
    {
        var minX = 500;
        var minY = 0;
        var maxX = minX;
        var maxY = minY;

        foreach (var (x, y) in Scans.SelectMany(scan => scan.Path))
        {
            if (x < minX) minX = x;
            if (x > maxX) maxX = x;
            if (y < minY) minY = y;
            if (y > maxY) maxY = y;
        }

        var floorY = maxY + 2;

        var fillers = new Dictionary<(int, int), Filler?>();

        for (var x = minX; x <= maxX; x++)
        {
            for (var y = minY; y <= maxY; y++)
            {
                fillers.Add((x, y), null);
            }
        }

        for (var x = minX; x <= maxX; x++)
        {
            fillers.Add((x, floorY), Filler.Rock);
        }

        IEnumerable<int> GetSpan(int from, int to)
        {
            if (from < to)
            {
                return Enumerable.Range(from, to - from + 1);
            }
            else
            {
                return Enumerable.Range(to, from - to + 1);
            }
        }

        foreach (var scan in Scans)
        {
            var previousPosition = scan.Path[0];

            foreach (var nextPosition in scan.Path.Skip(1))
            {
                var (previousX, previousY) = previousPosition;
                var (nextX, nextY) = nextPosition;

                if (previousX == nextX)
                {
                    foreach (var y in GetSpan(previousY, nextY))
                    {
                        fillers[(previousX, y)] = Filler.Rock;
                    }
                }
                else
                {
                    foreach (var x in GetSpan(previousX, nextX))
                    {
                        fillers[(x, previousY)] = Filler.Rock;
                    }
                }

                previousPosition = nextPosition;
            }
        }

        var sourceOfSand = (500, 0);

        fillers[sourceOfSand] = Filler.SourceOfSand;

        return new Cave
        {
            SourceOfSand = sourceOfSand,
            MinX = minX,
            MaxX = maxX,
            MinY = minY,
            MaxY = maxY,
            FloorY = floorY,
            Fillers = fillers,
        };
    }

    public string ToAnswer()
    {
        var cave = GetCave();

        //cave.Print();

        int grainsOfSand = 0;

        while (!cave.NextGrainOfSandBlocksSourceOfSand())
        {
            grainsOfSand++;

            //Console.WriteLine($"== Grains Of Sand: {grainsOfSand} ==");

            //cave.Print();
        }

        cave.Print();

        return (grainsOfSand + 1).ToString();
    }
}

public enum Filler
{
    SourceOfSand,
    Rock,
    Sand,
}

public class Scan
{
    public List<(int, int)> Path { get; init; } = new();
}


public class Cave
{
    public required (int, int) SourceOfSand { get; init; }
    public required int MinX { get; set; }
    public required int MaxX { get; set; }
    public required int MinY { get; init; }
    public required int MaxY { get; init; }
    public required int FloorY { get; init; }
    public Dictionary<(int, int), Filler?> Fillers { get; init; } = new();

    public void Print()
    {
        for (var y = MinY; y <= FloorY; y++)
        {
            for (var x = MinX; x <= MaxX; x++)
            {
                switch (Fillers.GetValueOrDefault((x, y)))
                {
                    case Filler.SourceOfSand:
                        Console.Write("+");
                        break;

                    case Filler.Rock:
                        Console.Write("#");
                        break;

                    case Filler.Sand:
                        Console.Write("O");
                        break;

                    default:
                        Console.Write(".");
                        break;
                }
            }

            Console.WriteLine();
        }

        Console.WriteLine();
    }


    private (int, int) GetNextPosition(int x, int y, int deltaX, int deltaY)
    {
        return (x + deltaX, y + deltaY);
    }

    private (int, int) GetNextSandPosition((int, int) position)
    {
        var (x, y) = position;

        var down = GetNextPosition(x, y, 0, +1);
        var downExists = Fillers.TryGetValue(down, out var downFiller);

        if (!downExists || downFiller == null)
        {
            return down;
        }

        var diagonalLeft = GetNextPosition(x, y, -1, +1);
        var diagonalLeftExists = Fillers.TryGetValue(diagonalLeft, out var diagonalLeftFiller);

        if (!diagonalLeftExists || diagonalLeftFiller == null)
        {
            return diagonalLeft;
        }

        var diagonalRight = GetNextPosition(x, y, +1, +1);
        var diagonalRightExists = Fillers.TryGetValue(diagonalRight, out var diagonalRightFiller);

        if (!diagonalRightExists || diagonalRightFiller == null)
        {
            return diagonalRight;
        }

        return position;
    }

    public bool NextGrainOfSandBlocksSourceOfSand()
    {
        var sandPosition = SourceOfSand;
        var nextSandPosition = GetNextSandPosition(sandPosition);

        if (nextSandPosition == SourceOfSand)
        {
            Fillers[sandPosition] = Filler.Sand;
            return true;
        }

        while (nextSandPosition != sandPosition)
        {
            sandPosition = nextSandPosition;

            var (nextX, nextY) = sandPosition;

            if (nextY == MaxY + 1)
            {
                MinX = Math.Min(MinX, nextX);
                MaxX = Math.Max(MaxX, nextX);

                Fillers.TryAdd((nextX, nextY + 1), Filler.Rock);
                nextSandPosition = sandPosition;
            }
            else
            {
                nextSandPosition = GetNextSandPosition(sandPosition);
            }

        }

        Fillers[sandPosition] = Filler.Sand;

        return false;
    }
}