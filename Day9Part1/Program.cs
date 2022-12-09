using System.Diagnostics;

await SolverUtility<Program>.LogAnswer(
    9,

    new Graph(),

    (fileReader) => {
        var line = fileReader.ReadLine() ?? "";

        var parts = line.Split(' ', 2);

        var direction = parts[0];
        var delta = int.Parse(parts[1]);

        return direction switch
        {
            "U" => new Up { Delta = delta },
            "D" => new Down { Delta = delta },
            "L" => new Left { Delta = delta },
            "R" => new Right { Delta = delta },
            _ => throw new UnreachableException()
        };
    }
);

public class Up : IInstruction<Graph>
{
    public required int Delta { get; init; }

    public Graph Reduce(Graph graph)
    {
        graph.Move(0, Delta);

        return graph;
    }
}

public class Down : IInstruction<Graph>
{
    public required int Delta { get; init; }

    public Graph Reduce(Graph graph)
    {
        graph.Move(0, 0 - Delta);

        return graph;
    }
}

public class Right : IInstruction<Graph>
{
    public required int Delta { get; init; }

    public Graph Reduce(Graph graph)
    {
        graph.Move(Delta, 0);

        return graph;
    }
}

public class Left : IInstruction<Graph>
{
    public required int Delta { get; init; }

    public Graph Reduce(Graph graph)
    {
        graph.Move(0 - Delta, 0);

        return graph;
    }
}

public class Graph : IState
{
    public List<HashSet<(int, int)>> AllDebugPositions { get; set; } = new();
    public HashSet<(int, int)> PositionsVisitedByTail { get; set; } = new()
    {
        (0, 0)
    };

    public int HeadX { get; set; }
    public int HeadY { get; set; }
    public int TailX { get; set; }
    public int TailY { get; set; }

    bool MoveCardinal(ref int x, ref int y, ref int deltaX, ref int deltaY)
    {
        if (deltaX > 0)
        {
            x += 1;
            deltaX -= 1;
            return true;
        }

        if (deltaX < 0)
        {
            x -= 1;
            deltaX += 1;
            return true;
        }

        if (deltaY > 0)
        {
            y += 1;
            deltaY -= 1;
            return true;
        }

        if (deltaY < 0)
        {
            y -= 1;
            deltaY += 1;
            return true;
        }

        return false;
    }

    bool Touching()
    {
        return Math.Abs(HeadX - TailX) <= 1 && Math.Abs(HeadY - TailY) <= 1;
    }

    public void Move(int deltaX, int deltaY)
    {
        var previousHeadX = HeadX;
        var previousHeadY = HeadY;

        var nextHeadX = HeadX;
        var nextHeadY = HeadY;

        var nextDeltaX = deltaX;
        var nextDeltaY = deltaY;

        var headMoved = MoveCardinal(ref nextHeadX, ref nextHeadY, ref nextDeltaX, ref nextDeltaY);

        // Update Head
        HeadX = nextHeadX;
        HeadY = nextHeadY;

        if (!headMoved)
        {
            return;
        }

        if (!Touching())
        {
            var nextTailX = TailX;
            var nextTailY = TailY;

            if (nextTailX == nextHeadX || nextTailY == nextHeadY)
            {
                // Same row or column, tail moves cardinally
                MoveCardinal(ref nextTailX, ref nextTailY, ref deltaX, ref deltaY);
            }
            else
            {
                // Not same row or column, tail moves diagonally

                // I think if the tail has to move diagonally,
                // it can just move to where the head was previously?

                nextTailX = previousHeadX;
                nextTailY = previousHeadY;
            }

            // Update Tail
            TailX = nextTailX;
            TailY = nextTailY;

            PositionsVisitedByTail.Add((nextTailX, nextTailY));

            AllDebugPositions.Add(new HashSet<(int, int)>(PositionsVisitedByTail));
        }

        // Keep Moving Until deltas are zero
        Move(nextDeltaX, nextDeltaY);
    }

    void PrintOneDebug(HashSet<(int, int)> debugPositions)
    {
        var minX = 0;
        var maxX = 0;
        var minY = 0;
        var maxY = 0;

        foreach (var position in debugPositions)
        {
            var (x, y) = position;

            if (x < minX) minX = x;
            if (x > maxX) maxX = x;

            if (y < minY) minY = y;
            if (y > maxY) maxY = y;
        }

        for (var y = maxY; y >= minY; y--)
        {
            for (var x = minX; x <= maxX; x++)
            {
                if (debugPositions.Contains((x, y)))
                {
                    Console.Write("#");
                }
                else
                {
                    Console.Write("-");
                }
            }

            Console.WriteLine();
        }

        Console.WriteLine();
    }

    void PrintDebug()
    {
        foreach (var debugPositions in AllDebugPositions)
        {
            PrintOneDebug(debugPositions);
        }

        Console.WriteLine();

    }

    public string ToAnswer()
    {
        PrintOneDebug(PositionsVisitedByTail);

        return PositionsVisitedByTail.Count.ToString();
    }
}
