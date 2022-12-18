using Dijkstra.NET.Graph;
using Dijkstra.NET.ShortestPath;

await SolverUtility<Program>.LogAnswer(
    12,

    new Map(),

    (fileReader) => {
        var character = (char)fileReader.Read();

        if (character == '\n')
        {
            return new NextRow();
        }

        var isStart = false;
        var isEnd = false;

        switch (character)
        {
            case 'S':
                character = 'a';
                isStart = true;
                break;

            case 'E':
                character = 'z';
                isEnd = true;
                break;

            default:
                break;
        }

        return new AddReading
        {
            Elevation = character - 'a',
            IsStart = isStart,
            IsEnd = isEnd,
        };
    }
);

public class NextRow : IInstruction<Map>
{
    public Map Reduce(Map map)
    {
        map.ReadY += 1;
        map.ReadX = 0;

        return map;
    }
}

public class AddReading : IInstruction<Map>
{
    public required int Elevation { get; init; }
    public required bool IsStart { get; init; }
    public required bool IsEnd { get; init; }

    public Map Reduce(Map map)
    {
        var position = (map.ReadX, map.ReadY);

        map.ReadX += 1;

        if (IsStart)
        {
            map.StartAt = position;
        }
        else if (IsEnd)
        {
            map.EndAt = position;
        }

        map.ElevationMap.Add(position, Elevation);

        return map;
    }
}

public class Map : IState
{
    public int ReadX { get; set; } = 0;
    public int ReadY { get; set; } = 0;

    public (int, int) StartAt { get; set; }
    public (int, int) EndAt { get; set; }

    public Dictionary<(int, int), int> ElevationMap = new();

    public string ToAnswer()
    {
        int minX = 0;
        int maxX = 0;
        int minY = 0;
        int maxY = 0;

        var graphBuilder = new Graph<int, string>();

        var dict = new Dictionary<(int, int), uint>();

        foreach (var position in ElevationMap.Keys)
        {
            var (x, y) = position;

            dict.Add(position, graphBuilder.AddNode(position.GetHashCode()));

            if (x < minX) minX = x;
            if (x > maxX) maxX = x;
            if (y < minY) minY = y;
            if (y > maxY) maxY = y;
        }

        void TryAddNeighbor(int x, int y, int deltaX, int deltaY)
        {
            // Don't go out of bounds
            if (x + deltaX < minX || x + deltaX > maxX || y + deltaY < minY || y + deltaY > maxY)
            {
                return;
            }

            var currentElevation = ElevationMap[(x, y)];
            var destinationElevation = ElevationMap[(x + deltaX, y + deltaY)];

            if (destinationElevation - currentElevation <= 1)
            {
                var position = (x, y);
                var neighborPosition = (x + deltaX, y + deltaY);

                graphBuilder.Connect(dict[position], dict[neighborPosition], 1, "");
            }
        }

        // Build Edges
        for (var y = minY; y <= maxY; y++)
        {
            for (var x = minX; x <= maxX; x++)
            {
                TryAddNeighbor(x, y, 0, -1);
                TryAddNeighbor(x, y, 0, +1);
                TryAddNeighbor(x, y, -1, 0);
                TryAddNeighbor(x, y, +1, 0);
            }
        }

        return ElevationMap
            .Where(pair => pair.Value == 0)
            .Select(pair => graphBuilder.Dijkstra(dict[pair.Key], dict[EndAt]))
            .MinBy(path => path.Distance)
            .Distance
            .ToString();
    }

}
