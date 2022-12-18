await SolverUtility<Program>.LogAnswer(
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
public class InstructionTypeA : IInstruction<StateTypeA>
{
    public StateTypeA Reduce(StateTypeA state)
    {
        return state;
    }
}

//TODO: Define States
public class StateTypeA : IState
{
    public string ToAnswer() => "Answer Not Set";
}