await SolverUtility<Program>.LogSolution(
    4,

    0,

    (fileReader) => {
        var line = fileReader.ReadLine() ?? "";

        var parts = line.Split(',', 2);

        var leftParts = parts[0].Split('-', 2);
        var rightParts = parts[1].Split('-', 2);

        var left = new SectionRange
        {
            From = int.Parse(leftParts[0]),
            To = int.Parse(leftParts[1]),
        };

        var right = new SectionRange
        {
            From = int.Parse(rightParts[0]),
            To = int.Parse(rightParts[1])
        };

        return new SectionPair
        {
            Left = left,
            Right = right,
        };
    },

    (previousAnswer, entry) => {
        var nextAnswer = previousAnswer;

        if (entry.FullOverlap())
        {
            nextAnswer += 1;
        }

        return nextAnswer;
    }
);

public class SectionRange
{
    public required int From { get; init; }
    public required int To { get; init; }

    public bool FullOverlap(SectionRange b)
    {
        return From <= b.From && To >= b.To;
    }
}

public class SectionPair
{
    public required SectionRange Left { get; init; }
    public required SectionRange Right { get; init; }

    public bool FullOverlap()
    {
        return Left.FullOverlap(Right) || Right.FullOverlap(Left);
    }
}