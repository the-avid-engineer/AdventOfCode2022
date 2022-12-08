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

        void Search(ref int blockedAtCount, int originalRowNum, int originalColNum, int rowNum, int colNum, int rowNumDelta, int colNumDelta)
        {
            var compareRowNum = rowNum + rowNumDelta;
            var compareColNum = colNum + colNumDelta;

            if (compareRowNum < 0 || compareRowNum == numRows || compareColNum < 0 || compareColNum == numCols)
            {
                return;
            }

            blockedAtCount += 1;

            var compareHeight = TreeRows[compareRowNum][compareColNum];
            var height = TreeRows[originalRowNum][originalColNum];

            if (compareHeight >= height)
            {
                return;
            }

            Search(ref blockedAtCount, originalRowNum, originalColNum, compareRowNum, compareColNum, rowNumDelta, colNumDelta);
        }

        int GetScore(int rowNum, int colNum)
        {
            var upBlockedAt = 0;

            Search(ref upBlockedAt, rowNum, colNum, rowNum, colNum, -1, 0);

            var downBlockedAt = 0;

            Search(ref downBlockedAt, rowNum, colNum, rowNum, colNum, +1, 0);

            var leftBlockedAt = 0;

            Search(ref leftBlockedAt, rowNum, colNum, rowNum, colNum, 0, -1);

            var rightBlockedAt = 0;

            Search(ref rightBlockedAt, rowNum, colNum, rowNum, colNum, 0, +1);

            return upBlockedAt * leftBlockedAt * downBlockedAt * rightBlockedAt;
        }

        var bestScore = 0;

        for (var rowNum = 0; rowNum < numRows; rowNum++)
        {
            for (var colNum = 0; colNum < numCols; colNum++)
            {
                var score = GetScore(rowNum, colNum);

                if (score > bestScore)
                {
                    bestScore = score;
                }
            }
        }


        return bestScore.ToString();
    }
}