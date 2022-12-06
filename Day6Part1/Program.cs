await SolverUtility<Program>.LogAnswer(
    6,

    new CharStream(),

    (fileReader) => {
        return new AddChar
        {
            Char = (char)fileReader.Read()
        };
    }
);

public class AddChar : IInstruction
{
    public required char Char { get; set; }

    public IState Reduce(IState state)
    {
        switch (state)
        {
            case CharStream charStream:
                if (charStream.SignalDetectedAt.HasValue)
                {
                    return charStream;
                }

                charStream.CharsReceived += 1;

                if (charStream.Queue.Count == 4)
                {
                    charStream.Queue.Dequeue();
                }

                charStream.Queue.Enqueue(Char);

                if (charStream.Queue.Distinct().Count() == 4)
                {
                    charStream.SignalDetectedAt = charStream.CharsReceived;
                }

                return charStream;

            default:
                throw new NotImplementedException();
        }
    }
}

public class CharStream : IState
{
    public int CharsReceived { get; set; }
    public int? SignalDetectedAt { get; set; }
    public Queue<char> Queue { get; set; } = new();

    public string ToAnswer() => SignalDetectedAt?.ToString() ?? "Answer Not Found";
}