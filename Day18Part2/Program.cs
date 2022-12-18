using System.Linq;

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
        var (minX, minY, minZ) = Positions.First();

        var maxX = minX;
        var maxY = minY;
        var maxZ = minZ;

        foreach (var (x, y, z) in Positions.Skip(1))
        {
            if (x < minX) minX = x;
            if (x > maxX) maxX = x;
            if (y < minY) minY = y;
            if (y > maxY) maxY = y;
            if (z < minZ) minZ = z;
            if (z > maxZ) maxZ = z;
        }

        // bounding box contains all cubes initially
        var boundingBox = new List<(int, int, int)>();

        for (var x = minX; x <= maxX; x++)
        {
            for (var y = minY; y <= maxY; y++)
            {
                for (var z = minZ; z <= maxZ; z++)
                {
                    boundingBox.Add((x, y, z));
                }
            }
        }

        (int, int, int) GetNeighborPosition((int, int, int) position, (int, int, int) delta)
        {
            var (x, y, z) = position;
            var (deltaX, deltaY, deltaZ) = delta;

            var neighborX = x + deltaX;
            var neighborY = y + deltaY;
            var neighborZ = z + deltaZ;

            return (neighborX, neighborY, neighborZ);
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

        var noNeighborFaces = 0;

        foreach (var position in Positions)
        {
            // remove cubes from bounding box
            boundingBox.Remove(position);

            foreach (var delta in deltas)
            {
                if (!Positions.Contains(GetNeighborPosition(position, delta)))
                {
                    noNeighborFaces++;
                }
            }
        }

        // Any position that is connected to the edge is not a bubble
        // Remove all positions connected to the edge
        // Any remaining positions are bubbles

        void PruneBoundingBox((int, int, int) position)
        {
            if (boundingBox.Contains(position))
            {
                boundingBox.Remove(position);

                foreach (var delta in deltas)
                {
                    var neighborPosition = GetNeighborPosition(position, delta);

                    PruneBoundingBox(neighborPosition);
                }
            }
        }

        for (var x = minX; x <= maxX; x++)
        {
            for (var y = minY; y <= maxY; y++)
            {
                for (var z = minZ; z <= maxZ; z++)
                {
                    if (x == minX || x == maxX || y == minY || y == maxY || z == minZ || z == maxZ)
                    {
                        var edgePosition = (x, y, z);

                        PruneBoundingBox(edgePosition);
                    }
                }
            }
        }

        var bubbleFaces = 0;

        foreach (var position in boundingBox)
        {
            foreach (var delta in deltas)
            {
                if (Positions.Contains(GetNeighborPosition(position, delta)))
                {
                    bubbleFaces++;
                }
            }
        }

        return (noNeighborFaces - bubbleFaces).ToString();
    }
}