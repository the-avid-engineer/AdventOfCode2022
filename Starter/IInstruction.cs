public interface IInstruction
{
    IState Reduce(IState previousState);
}
