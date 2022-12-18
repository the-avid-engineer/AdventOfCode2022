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

        var fillers = new Dictionary<(int, int), Filler?>();

        for (var x = minX; x <= maxX; x++)
        {
            for (var y = minY; y <= maxY; y++)
            {
                fillers.Add((x, y), null);
            }
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
            Fillers = fillers,
        };
    }

    public string ToAnswer()
    {
        var cave = GetCave();

        //cave.Print();

        int grainsOfSand = 0;

        while (!cave.NextGrainOfSandFallsIntoAbyss())
        {
            grainsOfSand++;

            //Console.WriteLine($"== Grains Of Sand: {grainsOfSand} ==");

            //cave.Print();
        }

        cave.Print();

        return grainsOfSand.ToString();
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
    public required int MinX { get; init; }
    public required int MaxX { get; init; }
    public required int MinY { get; init; }
    public required int MaxY { get; init; }
    public Dictionary<(int, int), Filler?> Fillers { get; init; } = new();

    public void Print()
    {
        for (var y = MinY; y <= MaxY; y++)
        {
            for (var x = MinX; x <= MaxX; x++)
            {
                switch (Fillers[(x, y)])
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

    private (int, int)? GetNextSandPosition((int, int) position)
    {
        var (x, y) = position;

        var down = GetNextPosition(x, y, 0, +1);

        if (!Fillers.TryGetValue(down, out var downFiller) || downFiller == null)
        {
            return down;
        }

        var diagonalLeft = GetNextPosition(x, y, -1, +1);
        
        if (!Fillers.TryGetValue(diagonalLeft, out var diagonalLeftFiller) || diagonalLeftFiller == null)
        {
            return diagonalLeft;
        }

        var diagonalRight = GetNextPosition(x, y, +1, +1);
        
        if (!Fillers.TryGetValue(diagonalRight, out var diagonalRightFiller) || diagonalRightFiller == null)
        {
            return diagonalRight;
        }

        return null;
    }

    public bool NextGrainOfSandFallsIntoAbyss()
    {
        var sandPosition = SourceOfSand;
        var nextSandPosition = GetNextSandPosition(sandPosition);

        if (nextSandPosition == null)
        {
            // This means that the source of sand is blocked?
            throw new NotSupportedException();
        }

        while (nextSandPosition != null)
        {
            var (_, nextY) = nextSandPosition.Value;

            if (nextY > MaxY)
            {
                return true;
            }

            sandPosition = nextSandPosition.Value;
            nextSandPosition = GetNextSandPosition(sandPosition);


            //Fillers[sandPosition] = Filler.Sand;
            //Print();
            //Fillers[sandPosition] = null;
        }

        Fillers[sandPosition] = Filler.Sand;

        return false;
    }
}