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

//TODO: Define Instructions
public class AddChar : IInstruction
{
    public required char Char { get; set; }

    public IState Reduce(IState state)
    {
        switch (state)
        {
            case CharStream charStream:

                charStream.CharsReceived += 1;

                if (!charStream.SignalDetector.MarkerDetectedAt.HasValue)
                {
                    charStream.SignalDetector.Check(Char, charStream.CharsReceived);
                }
                else if (!charStream.MessageDetecor.MarkerDetectedAt.HasValue)
                {
                    charStream.MessageDetecor.Check(Char, charStream.CharsReceived);
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
    public Detector SignalDetector = new(4);
    public Detector MessageDetecor = new(14);

    public string ToAnswer() => MessageDetecor.MarkerDetectedAt?.ToString() ?? "Answer Not Found";
}

public class Detector
{
    private readonly int _signalSize;

    public int? MarkerDetectedAt { get; private set; }
    private Queue<char> Queue { get; set; } = new();

    public Detector(int signalSize)
    {
        _signalSize = signalSize;
    }

    public void Check(char @char, int position)
    {
        if (Queue.Count == _signalSize)
        {
            Queue.Dequeue();
        }

        Queue.Enqueue(@char);

        if (Queue.Distinct().Count() == _signalSize)
        {
            MarkerDetectedAt = position;
        }
    }
}