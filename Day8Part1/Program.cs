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

        return new AddTreeRow
        {
            Heights = line
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

public class AddTreeRow : IInstruction
{
    public required int[] Heights { get; init; }

    public IState Reduce(IState previousState)
    {
        if (previousState is not Forest forest)
        {
            throw new UnreachableException();
        }

        forest.TreeRows.Add(Heights);

        return forest;
    }
}

public class Forest : IState
{
    public List<int[]> TreeRows { get; init; } = new();

    public string ToAnswer()
    {
        var numRows = TreeRows.Count;
        var numCols = TreeRows.First().Length;

        var rowHighestLeftToRight = new Dictionary<int, int>();
        var rowHighestRightToLeft = new Dictionary<int, int>();
        var colHighestTopToBottom = new Dictionary<int, int>();
        var colHighestBottomToTop = new Dictionary<int, int>();

        var visibilityGraph = new Dictionary<(int, int), bool>();

        // Initialize

        for (var rowNum = 0; rowNum < numRows; rowNum++)
        {
            for (var colNum = 0; colNum < numCols; colNum++)
            {
                var isLeftEdge = colNum == 0;
                var isTopEdge = rowNum == 0;
                var isRightEdge = colNum == numCols - 1;
                var isBottomEdge = rowNum == numRows - 1;

                var coordinate = (rowNum, colNum);

                if (isLeftEdge || isTopEdge || isRightEdge || isBottomEdge)
                {
                    visibilityGraph.Add(coordinate, true);
                }
                else
                {
                    visibilityGraph.Add(coordinate, false);
                }

                var height = TreeRows[rowNum][colNum];

                if (isLeftEdge)
                {
                    rowHighestLeftToRight[rowNum] = height;
                }

                if (isRightEdge)
                {
                    rowHighestRightToLeft[rowNum] = height;
                }

                if (isTopEdge)
                {
                    colHighestTopToBottom[colNum] = height;
                }

                if (isBottomEdge)
                {
                    colHighestBottomToTop[colNum] = height;
                }
            }
        }

        // Scan Top To Bottom, Left To Right, Update Highest

        for (var rowNum = 1; rowNum <= numRows - 2; rowNum++)
        {
            for (var colNum = 1; colNum <= numCols - 2; colNum++)
            {
                var coordinate = (rowNum, colNum);
                var height = TreeRows[rowNum][colNum];

                if (height > rowHighestLeftToRight[rowNum])
                {
                    visibilityGraph[coordinate] = true;
                    rowHighestLeftToRight[rowNum] = height;
                }

                if (height > colHighestTopToBottom[colNum])
                {
                    visibilityGraph[coordinate] = true;
                    colHighestTopToBottom[colNum] = height;
                }
            }
        }

        // Scan Bottom To Top, Right To Left, Update Highest

        for (var rowNum = numRows - 2; rowNum >= 1; rowNum--)
        {
            for (var colNum = numCols - 2; colNum >= 1; colNum--)
            {
                var coordinate = (rowNum, colNum);
                var height = TreeRows[rowNum][colNum];

                if (height > rowHighestRightToLeft[rowNum])
                {
                    visibilityGraph[coordinate] = true;
                    rowHighestRightToLeft[rowNum] = height;
                }

                if (height > colHighestBottomToTop[colNum])
                {
                    visibilityGraph[coordinate] = true;
                    colHighestBottomToTop[colNum] = height;
                }
            }
        }

        return visibilityGraph
            .Select(pair => pair.Value)
            .Where(visible => visible)
            .Count()
            .ToString();
    }
}