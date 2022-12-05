await SolverUtility<Program>.LogSolution(
    5, //TODO: Set day

    new InitialState(),

    (fileReader) => {
        var line = fileReader.ReadLine() ?? "";
        //var character = (char)streamReader.Read();

        //TODO: Parse the line into an instruction
        return DoNothing.Instance;
    }
);

public class InitialState : IState
{
    public string ToAnswer() => "Answer Not Set";
}

public class Instruction1 : IInstruction
{
    public IState Reduce(IState state)
    {
        switch (state)
        {
            case InitialState initialState:
                return state;

            default:
                throw new NotImplementedException();
        }
    }
}