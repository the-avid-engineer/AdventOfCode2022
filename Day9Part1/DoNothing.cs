public class DoNothing : IInstruction
{
    public IState Reduce(IState state) => state;
}