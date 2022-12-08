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

        int Search(int originRowNum, int originColNum, int rowNumDelta, int colNumDelta)
        {
            var blockedAt = 0;

            var originHeight = Trees[width * originRowNum + originColNum];

            var checkRowNum = originRowNum + rowNumDelta;
            var checkColNum = originColNum + colNumDelta;

            while (checkRowNum >= 0 && checkRowNum < length && checkColNum >= 0 && checkColNum < width)
            {
                blockedAt += 1;

                var checkHeight = Trees[width * checkRowNum + checkColNum];

                if (checkHeight >= originHeight)
                {
                    break;
                }

                checkRowNum += rowNumDelta;
                checkColNum += colNumDelta;
            }

            return blockedAt;
        }

        var rowNum = 0;
        var colNum = 0;

        var bestScore = 0;

        foreach (var tree in Trees)
        {
            var score =
                Search(rowNum, colNum, -1, 0) *
                Search(rowNum, colNum, +1, 0) *
                Search(rowNum, colNum, 0, -1) *
                Search(rowNum, colNum, 0, +1);

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