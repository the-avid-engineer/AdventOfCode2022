using System.Diagnostics;

await SolverUtility<Program>.LogAnswer(
    9,

    State.Initialize(9),

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

public class Up : IInstruction<State>
{
    public required int Delta { get; init; }

    public State Reduce(State state)
    {
        state.Move(Delta, 0, +1);

        return state;
    }
}

public class Down : IInstruction<State>
{
    public required int Delta { get; init; }

    public State Reduce(State state)
    {
        state.Move(Delta, 0, -1);

        return state;
    }
}

public class Right : IInstruction<State>
{
    public required int Delta { get; init; }

    public State Reduce(State state)
    {
        state.Move(Delta, +1, 0);

        return state;
    }
}

public class Left : IInstruction<State>
{
    public required int Delta { get; init; }

    public State Reduce(State state)
    {
        state.Move(Delta, -1, 0);

        return state;
    }
}

public class State : IState
{
    public List<Dictionary<char, (int, int)>> PositionSnapshots { get; set; } = new();

    public static State Initialize(int numberOfJoints)
    {
        var head = new Graph
        {
            Label = '0'
        };

        var parent = head;

        for (var i = 1; i <= numberOfJoints; i++)
        {
            var nextGraph = new Graph
            {
                Label = i.ToString()[0]
            };

            parent.Tail = nextGraph;
            parent = nextGraph;
        }

        var state = new State
        {
            Head = head
        };

        state.LogPositionSnapshot();

        return state;
    }

    public required Graph Head { get; init; }

    public void Move(int times, int deltaX, int deltaY)
    {
        Head.Move(times, deltaX, deltaY, this);
    }

    public void LogPositionSnapshot()
    {
        var positionSnapshot = new Dictionary<char, (int, int)>();

        var graph = Head;

        while (graph != null)
        {
            positionSnapshot.Add(graph.Label, (graph.X, graph.Y));

            graph = graph.Tail;
        }

        PositionSnapshots.Add(positionSnapshot);

        Console.WriteLine();
    }

    public void PrintDebug()
    {
        var minX = 0;
        var maxX = 0;
        var minY = 0;
        var maxY = 0;

        foreach (var (x, y) in PositionSnapshots.SelectMany(positionSnapshot => positionSnapshot.Values))
        {
            if (x < minX) minX = x;
            if (x > maxX) maxX = x;

            if (y < minY) minY = y;
            if (y > maxY) maxY = y;
        }

        Console.WriteLine(minX);
        Console.WriteLine(maxX);
        Console.WriteLine(minY);
        Console.WriteLine(maxY);

        foreach (var positionSnapshot in PositionSnapshots.Distinct())
        {
            var reverseLookup = positionSnapshot
                .ToLookup(x => x.Value, x => x.Key);

            for (var y = maxY; y >= minY; y--)
            {
                for (var x = minX; x <= maxX; x++)
                {
                    if (reverseLookup.Contains((x, y)))
                    {
                        Console.Write(reverseLookup[(x, y)].Min());
                    }
                    else
                    {
                        Console.Write(".");
                    }
                }

                Console.WriteLine();
            }

            Console.WriteLine();
        }

        Console.WriteLine();
    }

    public string ToAnswer()
    {
        //PrintDebug();

        var parent = Head;

        while (parent.Tail != null && parent.Tail.Tail != null)
        {
            parent = parent.Tail;
        }

        return parent.PositionsVisitedByTail.Count.ToString();
    }
}

public class Graph
{
    public HashSet<(int, int)> PositionsVisitedByTail { get; set; } = new()
    {
        (0, 0)
    };

    public required char Label { get; init; }
    public int X { get; set; }
    public int Y { get; set; }
    public Graph? Tail { get; set; }

    public bool Touching(Graph otherGraph)
    {
        return Math.Abs(X - otherGraph.X) <= 1 && Math.Abs(Y - otherGraph.Y) <= 1;
    }

    public void Move(int times, int deltaX, int deltaY, State state)
    {
        if (times == 0)
        {
            //state.LogPositionSnapshot();
            return;
        }

        MoveRelative(deltaX, deltaY, state);

        //state.LogPositionSnapshot();

        Move(times - 1, deltaX, deltaY, state);
    }

    public void MoveRelative(int deltaX, int deltaY, State state)
    {
        var previousX = X;
        var previousY = Y;

        X += deltaX;
        Y += deltaY;

        //state.LogPositionSnapshot();

        if (Tail == null || Touching(Tail))
        {
            return;
        }

        if (Tail.X == X)
        {
            Tail.MoveRelative(deltaX, deltaY, state);
        }
        else
        {
            Tail.MoveTo(previousX, previousY, state);
        }

        if (!Touching(Tail))
        {
            throw new Exception("Should be touching tail");
        }

        Tail.LogPosition(this);
    }

    public void MoveTo(int x, int y, State state)
    {
        var previousX = X;
        var previousY = Y;

        X = x;
        Y = y;

        //state.LogPositionSnapshot();

        var deltaX = X - previousX;
        var deltaY = Y - previousY;

        if (Tail == null || Touching(Tail))
        {
            return;
        }

        // Degenerate Cases
        if (Tail.X != X && Tail.Y != Y && (deltaX == 0 || deltaY == 0))
        {
            if (deltaX == 0)
            {
                deltaX = X - Tail.X;
            }
            else
            {
                deltaY = Y - Tail.Y;
            }
        }
        else if ((Tail.X == X || Tail.Y == Y) && (deltaX != 0 && deltaY != 0))
        {
            if (Tail.X == X)
            {
                deltaX = 0;
            }
            else
            {
                deltaY = 0;
            }
        }

        Tail.MoveTo(Tail.X + deltaX, Tail.Y + deltaY, state);

        if (!Touching(Tail))
        {
            throw new Exception("Should be touching tail");
        }

        Tail.LogPosition(this);
    }

    void LogPosition(Graph parent)
    {
        parent.PositionsVisitedByTail.Add((X, Y));
    }
}
