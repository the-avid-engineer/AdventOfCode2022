﻿await SolverUtility<Program>.LogSolution(
    5,

    "Answer Not Set", //TODO: Set initial answer

    (fileReader) => {
        //TODO: Read an entry from the file

        var line = fileReader.ReadLine() ?? "";
        //var character = (char)streamReader.Read();

        return default(object?);
    },

    (previousAnswer, entry) => {
        //TODO: Process the entry

        return previousAnswer;
    }
);