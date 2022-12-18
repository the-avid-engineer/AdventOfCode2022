﻿public static class SolverUtility<TProgram>
{
    public static async Task LogAnswer
    (
        int day,
        IState initialState,
        Func<StreamReader, IInstruction> getNextInstruction
    )
    {
        var start = DateTime.UtcNow;

        using var file = await InputUtility<TProgram>.GetInputStream(day);
        using var fileReader = new StreamReader(file);

        var state = initialState;

        while (!fileReader.EndOfStream)
        {
            state = getNextInstruction.Invoke(fileReader).Reduce(state);
        }

        Console.WriteLine($"Answer: {state.ToAnswer()}");

        Console.WriteLine($"Time: {DateTime.UtcNow - start}");
    }
}