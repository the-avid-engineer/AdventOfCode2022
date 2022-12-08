using System.Diagnostics;

await SolverUtility<Program>.LogAnswer(
    8,

    new Forest(),

    (fileReader) => {
        var line = fileReader.ReadLine() ?? "";

        if (line == "")
        {
            return new DoNothing();
        }

        return new AddTrees
        {
            Trees = line
                .ToCharArray()
                .Select(@char => @char - '0')
                .ToArray()
        };
    }
);

public class DoNothing : IInstruction
{
    public IState Reduce(IState state) => state;
}

public class AddTrees : IInstruction
{
    public required int[] Trees { get; init; }

    public IState Reduce(IState previousState)
    {
        if (previousState is not Forest forest)
        {
            throw new UnreachableException();
        }

        if (!forest.Width.HasValue)
        {
            forest.Width = Trees.Length;
        }

        forest.Length += 1;

        forest.Trees.AddRange(Trees);

        return forest;
    }
}

public class Forest : IState
{
    public int? Width { get; set; }
    public int Length { get; set; }
    public List<int> Trees { get; init; } = new();

    public string ToAnswer()
    {
        var length = Length;
        var width = Width ?? throw new UnreachableException();

        var visibleTreeCoordinates = new HashSet<(int, int)>();

        void Search(int highestRowNum, int highestColNum, int rowNum, int colNum, int rowNumDelta, int colNumDelta)
        {
            while (true)
            {
                var compareRowNum = rowNum + rowNumDelta;
                var compareColNum = colNum + colNumDelta;

                if (compareRowNum < 0 || compareRowNum == length || compareColNum < 0 || compareColNum == width)
                {
                    return;
                }

                var compareHeight = Trees[width * compareRowNum + compareColNum];
                var highestHeight = Trees[width * highestRowNum + highestColNum];

                if (compareHeight > highestHeight)
                {
                    visibleTreeCoordinates.Add((compareRowNum, compareColNum));

                    highestRowNum = compareRowNum;
                    highestColNum = compareColNum;
                }

                rowNum = compareRowNum;
                colNum = compareColNum;
            }
        }

        const int topEdgeRowNum = 0;
        const int leftEdgeColNum = 0;

        var bottomEdgeRowNum = length - 1;
        var rightEdgeColNum = width - 1;

        foreach (var colNum in Enumerable.Range(0, width))
        {
            // Top Edge

            visibleTreeCoordinates.Add((topEdgeRowNum, colNum));

            Search(topEdgeRowNum, colNum, topEdgeRowNum, colNum, +1, 0);


            // Bottom Edge

            Search(bottomEdgeRowNum, colNum, bottomEdgeRowNum, colNum, -1, 0);

            visibleTreeCoordinates.Add((bottomEdgeRowNum, colNum));
        }

        foreach (var rowNum in Enumerable.Range(1, length - 2))
        {
            // Left Edge

            visibleTreeCoordinates.Add((rowNum, leftEdgeColNum));

            Search(rowNum, leftEdgeColNum, rowNum, leftEdgeColNum, 0, +1);


            // Right Edge

            visibleTreeCoordinates.Add((rowNum, rightEdgeColNum));

            Search(rowNum, rightEdgeColNum, rowNum, rightEdgeColNum, 0, -1);
        }

        return visibleTreeCoordinates
            .Count()
            .ToString();
    }
}