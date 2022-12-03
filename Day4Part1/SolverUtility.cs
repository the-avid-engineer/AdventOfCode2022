public static class SolverUtility<TProgram>
{
    public static async Task LogSolution<TAnswer, TEntry>
    (
        int day,
        TAnswer initialAnswer,
        Func<StreamReader, TEntry> getNextEntry,
        Func<TAnswer, TEntry, TAnswer> getNextAnswer
    )
    {
        using var file = await InputUtility<TProgram>.GetInputStream(day);
        using var fileReader = new StreamReader(file);

        var answer = initialAnswer;

        while (!fileReader.EndOfStream)
        {
            var entry = getNextEntry.Invoke(fileReader);

            answer = getNextAnswer(answer, entry);
        }

        Console.WriteLine($"Answer: {answer}");
    }
}