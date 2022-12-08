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

public class AddTreeRow : IInstruction
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

        void Search(ref int blockedAtCount, int originalRowNum, int originalColNum, int rowNumDelta, int colNumDelta)
        {
            var originalHeight = Trees[width * originalRowNum + originalColNum];

            var checkRowNum = originalRowNum + rowNumDelta;
            var checkColNum = originalColNum + colNumDelta;

            while (checkRowNum >= 0 && checkRowNum < length && checkColNum >= 0 && checkColNum < width)
            {
                blockedAtCount += 1;

                var checkHeight = Trees[width * checkRowNum + checkColNum];

                if (checkHeight >= originalHeight)
                {
                    return;
                }

                checkRowNum += rowNumDelta;
                checkColNum += colNumDelta;
            }
        }

        int GetScore(int rowNum, int colNum)
        {
            var upBlockedAt = 0;
            var downBlockedAt = 0;
            var leftBlockedAt = 0;
            var rightBlockedAt = 0;

            Search(ref upBlockedAt, rowNum, colNum, -1, 0);
            Search(ref downBlockedAt, rowNum, colNum, +1, 0);
            Search(ref leftBlockedAt, rowNum, colNum, 0, -1);
            Search(ref rightBlockedAt, rowNum, colNum, 0, +1);

            return upBlockedAt * leftBlockedAt * downBlockedAt * rightBlockedAt;
        }

        var rowNum = 0;
        var colNum = 0;

        var bestScore = 0;

        foreach (var tree in Trees)
        {
            var score = GetScore(rowNum, colNum);

            if (score > bestScore)
            {
                bestScore = score;
            }

            colNum += 1;

            if (colNum == width)
            {
                colNum = 0;
                rowNum += 1;
            }
        }

        return bestScore.ToString();
    }
}