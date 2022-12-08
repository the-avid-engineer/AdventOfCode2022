using System.Diagnostics;

public interface IInstruction
{
    IState Reduce(IState previousState);
}

public interface IInstruction<TState> : IInstruction
    where TState : IState
{
    IState IInstruction.Reduce(IState previousState)
    {
        if (previousState is not TState state)
        {
            throw new UnreachableException();
        }

        return state;
    }

    TState Reduce(TState previousState);
}
