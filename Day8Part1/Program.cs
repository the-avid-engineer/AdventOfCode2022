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

        var visibleTreeCoordinates = new HashSet<(int, int)>();

        void Search(int highestRowNum, int highestColNum, int rowNum, int colNum, int rowNumDelta, int colNumDelta)
        {
            var compareRowNum = rowNum + rowNumDelta;
            var compareColNum = colNum + colNumDelta;

            if (compareRowNum < 0 || compareRowNum == numRows || compareColNum < 0 || compareColNum == numCols)
            {
                return;
            }

            var compareHeight = TreeRows[compareRowNum][compareColNum];
            var highestHeight = TreeRows[highestRowNum][highestColNum];

            if (compareHeight > highestHeight)
            {
                visibleTreeCoordinates.Add((compareRowNum, compareColNum));

                Search(compareRowNum, compareColNum, compareRowNum, compareColNum, rowNumDelta, colNumDelta);

                return;
            }

            Search(highestRowNum, highestColNum, compareRowNum, compareColNum, rowNumDelta, colNumDelta);
        }

        for (var rowNum = 0; rowNum < numRows; rowNum++)
        {
            for (var colNum = 0; colNum < numCols; colNum++)
            {
                var isLeftEdge = colNum == 0;
                var isTopEdge = rowNum == 0;
                var isRightEdge = colNum == numCols - 1;
                var isBottomEdge = rowNum == numRows - 1;

                var coordinate = (rowNum, colNum);

                if (isTopEdge)
                {
                    visibleTreeCoordinates.Add(coordinate);

                    Search(rowNum, colNum, rowNum, colNum, +1, 0);
                }
                else if (isLeftEdge)
                {
                    visibleTreeCoordinates.Add(coordinate);

                    Search(rowNum, colNum, rowNum, colNum, 0, +1);
                }
                else if (isBottomEdge)
                {
                    visibleTreeCoordinates.Add(coordinate);

                    Search(rowNum, colNum, rowNum, colNum, -1, 0);
                }
                else if (isRightEdge)
                {
                    visibleTreeCoordinates.Add(coordinate);

                    Search(rowNum, colNum, rowNum, colNum, 0, -1);
                }
            }
        }

        return visibleTreeCoordinates
            .Count()
            .ToString();
    }
}