await SolverUtility<Program>.LogSolution(
    0, //TODO: Set day

    new StateTypeA(),

    (fileReader) => {
        var line = fileReader.ReadLine() ?? "";
        //var character = (char)streamReader.Read();

        //TODO: Parse the line into an instruction
        return new InstructionTypeA();
    }
);

//TODO: Define Instructions
public class InstructionTypeA : IInstruction
{
    public IState Reduce(IState state)
    {
        switch (state)
        {
            case StateTypeA stateTypeA:
                throw new NotImplementedException();

            default:
                throw new NotImplementedException();
        }
    }
}

//TODO: Define States
public class StateTypeA : IState
{
    public string ToAnswer() => "Answer Not Set";
}