var read = Read.CrateRows;

await SolverUtility<Program>.LogSolution<IAnswer, IInstruction>(
    5,

    new CrateRowsAnswer(),

    (fileReader) => {
        var line = fileReader.ReadLine() ?? "";

        // Change what we're reading
        if (read == Read.CrateRows && !line.Contains("["))
        {
            // Time to read crate stack numbers line
            read = Read.CrateStackNumbers;
        }

        switch (read)
        {
            case Read.CrateRows:
                var crates = new List<char?>();

                while (line != "")
                {
                    var box = line.Take(3).ToArray();

                    line = string.Join("", line.Skip(4));

                    if (box[0] == '[')
                    {
                        crates.Add(box[1]);
                    }
                    else
                    {
                        crates.Add(null);
                    }
                }

                return new AddCrateRow
                {
                    Crates = crates
                };

            case Read.CrateStackNumbers:
                // I think we can ignore them?

                // Time to read the empty row
                read = Read.EmptyRow;
                return new DoNothing();

            case Read.EmptyRow:
                // Time to read the crane instructions
                read = Read.CraneInstruction;
                return new TransformToCrateStacks();

            case Read.CraneInstruction:
                //move 1 from 2 to 1

                var numbers = line.Replace("move ", "")
                    .Replace(" from ", " ")
                    .Replace(" to ", " ")
                    .Split(' ', 3)
                    .Select(int.Parse)
                    .ToArray();

                return new MoveCrates
                {
                    Move = numbers[0],
                    From = numbers[1],
                    To = numbers[2],
                };

            default:
                throw new NotImplementedException();
        }
    },

    (previousAnswer, entry) => {
        return previousAnswer.Reduce(entry);
    }
);

enum Read
{
    CrateRows,
    CrateStackNumbers,
    EmptyRow,
    CraneInstruction,
}

public interface IAnswer
{
    IAnswer Reduce(IInstruction instruction);
}

public interface IInstruction
{
}

public class AddCrateRow : IInstruction
{
    public required List<char?> Crates { get; init; }
}

public class DoNothing : IInstruction
{
}

public class TransformToCrateStacks : IInstruction
{
}

public class MoveCrates : IInstruction
{
    public required int Move { get; init; }
    public required int From { get; init; }
    public required int To { get; init; }
}

public class CrateRowsAnswer : IAnswer
{
    public Stack<AddCrateRow> AddCrateRows { get; init; } = new();

    public IAnswer Reduce(IInstruction instruction)
    {
        switch (instruction)
        {
            case AddCrateRow addCrateRow:
                AddCrateRows.Push(addCrateRow);
                return this;

            case DoNothing:
                return this;

            case TransformToCrateStacks:
                var crateStacksAnswer = new CrateStacksAnswer();

                var stackCount = AddCrateRows.Peek().Crates.Count;

                for (var i = 0; i < stackCount; i++)
                {
                    crateStacksAnswer.CrateStacks.Add(new());
                }

                foreach (var addCrateRow in AddCrateRows)
                {
                    for (var i = 0; i < stackCount; i++)
                    {
                        var @char = addCrateRow.Crates[i];

                        if (@char.HasValue)
                        {
                            crateStacksAnswer.CrateStacks[i].Push(@char.Value);
                        }
                    }
                }

                return crateStacksAnswer;

            default:
                throw new NotImplementedException();
        }
    }

    public override string ToString()
    {
        throw new Exception("Not Solved");
    }
}

public class CrateStacksAnswer : IAnswer
{
    public List<Stack<char>> CrateStacks { get; init; } = new();

    public IAnswer Reduce(IInstruction instruction)
    {
        switch (instruction)
        {
            case MoveCrates moveCrates:
                for (var i = 0; i < moveCrates.Move; i++)
                {
                    CrateStacks[moveCrates.To - 1].Push(CrateStacks[moveCrates.From - 1].Pop());
                }
                return this;

            default:
                throw new NotImplementedException();
        }
    }

    public override string ToString()
    {
        return string.Join("", CrateStacks.Select(crateStack => crateStack.Peek()));
    }
}