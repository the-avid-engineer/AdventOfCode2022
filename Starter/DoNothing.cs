public class DoNothing : IInstruction
{
    public static readonly DoNothing Instance = new();

    public IState Reduce(IState previousState)
    {
        return previousState;
    }
}