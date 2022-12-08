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

        void Search(int highestRowNum, int highestColNum, int rowNumDelta, int colNumDelta)
        {
            var checkRowNum = highestRowNum + rowNumDelta;
            var checkColNum = highestColNum + colNumDelta;

            while (checkRowNum >= 0 && checkRowNum < length && checkColNum >= 0 && checkColNum < width)
            {
                var compareHeight = Trees[width * checkRowNum + checkColNum];
                var highestHeight = Trees[width * highestRowNum + highestColNum];

                if (compareHeight > highestHeight)
                {
                    visibleTreeCoordinates.Add((checkRowNum, checkColNum));

                    highestRowNum = checkRowNum;
                    highestColNum = checkColNum;
                }

                checkRowNum += rowNumDelta;
                checkColNum += colNumDelta;
            }
        }

        const int topEdgeRowNum = 0;
        const int leftEdgeColNum = 0;

        var bottomEdgeRowNum = length - 1;
        var rightEdgeColNum = width - 1;

        foreach (var colNum in Enumerable.Range(0, width))
        {
            visibleTreeCoordinates.Add((topEdgeRowNum, colNum));
            visibleTreeCoordinates.Add((bottomEdgeRowNum, colNum));

            Search(topEdgeRowNum, colNum, +1, 0);
            Search(bottomEdgeRowNum, colNum, -1, 0);
        }

        foreach (var rowNum in Enumerable.Range(1, length - 2))
        {
            visibleTreeCoordinates.Add((rowNum, leftEdgeColNum));
            visibleTreeCoordinates.Add((rowNum, rightEdgeColNum));

            Search(rowNum, leftEdgeColNum, 0, +1);
            Search(rowNum, rightEdgeColNum, 0, -1);
        }

        return visibleTreeCoordinates.Count.ToString();
    }
}