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

                Search(compareRowNum, compareColNum, compareRowNum, compareColNum, rowNumDelta, colNumDelta);

                return;
            }

            Search(highestRowNum, highestColNum, compareRowNum, compareColNum, rowNumDelta, colNumDelta);
        }

        int rowNum = 0;
        int colNum = 0;

        foreach (var tree in Trees)
        {
            if (rowNum == 0)
            {
                visibleTreeCoordinates.Add((rowNum, colNum));

                Search(rowNum, colNum, rowNum, colNum, +1, 0);
            }
            else if (colNum == 0)
            {
                visibleTreeCoordinates.Add((rowNum, colNum));

                Search(rowNum, colNum, rowNum, colNum, 0, +1);
            }
            else if (rowNum == length - 1)
            {
                visibleTreeCoordinates.Add((rowNum, colNum));

                Search(rowNum, colNum, rowNum, colNum, -1, 0);
            }
            else if (colNum == width - 1)
            {
                visibleTreeCoordinates.Add((rowNum, colNum));

                Search(rowNum, colNum, rowNum, colNum, 0, -1);
            }

            colNum += 1;

            if (colNum == width)
            {
                colNum = 0;
                rowNum += 1;
            }
        }

        return visibleTreeCoordinates
            .Count()
            .ToString();
    }
}