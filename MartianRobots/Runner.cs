namespace MartianRobots;

/// <summary>
/// Reads the challenge input line by line, runs each robot as soon as its two lines are read,
/// and writes its result. Stops at the first invalid line: results already written stay,
/// the error goes to <c>error</c>, and the exit code is 1.
/// </summary>
public static class Runner
{
    public static int Run(TextReader input, TextWriter output, TextWriter error)
    {
        var lineNumber = 0;

        string? ReadLine()
        {
            lineNumber++;
            return input.ReadLine();
        }

        string? ReadNonBlankLine()
        {
            string? line;
            while ((line = ReadLine()) is not null && string.IsNullOrWhiteSpace(line)) { }
            return line;
        }

        int Fail(string message)
        {
            error.WriteLine($"Error: line {lineNumber}: {message}");
            return 1;
        }

        var gridLine = ReadNonBlankLine();
        if (gridLine is null)
        {
            error.WriteLine("Error: input is empty.");
            return 1;
        }

        var grid = InputParser.ParseGrid(gridLine);
        if (!grid.IsSuccess) return Fail(grid.Error);

        var simulation = new Simulation();

        // Blank lines are skipped between robots. A blank instruction line is an empty instruction.
        while (ReadNonBlankLine() is { } positionLine)
        {
            var start = InputParser.ParseRobotState(positionLine, grid.Value);
            if (!start.IsSuccess) return Fail(start.Error);

            var instructionLine = ReadLine();
            if (instructionLine is null) return Fail("robot has a position but no instruction line.");

            var instructions = InputParser.ParseInstructions(instructionLine);
            if (!instructions.IsSuccess) return Fail(instructions.Error);

            var final = simulation.Execute(start.Value, instructions.Value, grid.Value);
            output.WriteLine($"{final.X} {final.Y} {final.Facing}{(final.IsLost ? " LOST" : "")}");
        }

        return 0;
    }
}
