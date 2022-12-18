await SolverUtility<Program>.LogAnswer(
    18,

    new Space(),

    (fileReader) => {
        var line = fileReader.ReadLine() ?? "";

        if (line == "")
        {
            return new DoNothing();
        }

        var components = line.Split(',', 3)
            .Select(int.Parse)
            .ToArray();

        var position = (components[0], components[1], components[2]);

        return new AddCube
        {
            Position = position,
        };
    }
);

public class AddCube : IInstruction<Space>
{
    public required (int, int, int) Position { get; init; }

    public Space Reduce(Space space)
    {
        space.Positions.Add(Position);

        return space;
    }
}

public class Space : IState
{
    public List<(int, int, int)> Positions { get; } = new();

    public string ToAnswer()
    {
        bool HasNeighbor((int, int, int) position, (int, int, int) delta)
        {
            var (x, y, z) = position;
            var (deltaX, deltaY, deltaZ) = delta;

            var neighborX = x + deltaX;
            var neighborY = y + deltaY;
            var neighborZ = z + deltaZ;

            return Positions.Contains((neighborX, neighborY, neighborZ));
        }

        var deltas = new[]
        {
            (0, 0, +1),
            (0, 0, -1),
            (0, +1, 0),
            (0, -1, 0),
            (+1, 0, 0),
            (-1, 0, 0)
        };

        var exposedFaces = 0;

        foreach (var position in Positions)
        {
            foreach (var delta in deltas)
            {
                if (!HasNeighbor(position, delta))
                {
                    exposedFaces++;
                }
            }
        }


        return exposedFaces.ToString();
    }
}