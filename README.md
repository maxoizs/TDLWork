# Martian Robots

Solution to the TDL Developer Programming Challenge.

## Running

```bash
dotnet run --project MartianRobots < input.txt
```

Or pipe input directly:

```bash
echo "5 3
1 1 E
RFRFRFRF
3 2 N
FRRFLLFFRRFLL
0 3 W
LLFFFLFLFL" | dotnet run --project MartianRobots
```

Expected output:
```
1 1 E
3 3 N LOST
2 3 S
```

Exit code is `0` on success. On invalid input the program stops at the first bad line, writes `Error: line N: ...` to stderr and exits with `1`. Results for robots processed before the bad line are already written to stdout.

## Running Tests

```bash
dotnet test
```

171 tests covering unit, edge case, and integration scenarios.

## Technical Choices

**C# with record types** — `RobotState` is an immutable record. Each command returns a new state rather than mutating, making the simulation easy to reason about and test.

**Dictionary-based command dispatch** — commands (`L`, `R`, `F`) are registered in a `static readonly Dictionary<char, Command>`. The execution loop does not change when a command is added. The parser asks `Simulation.IsKnownCommand` which characters are valid, so a new command is still one dictionary entry.

**`Result<T>` pattern (no exceptions on bad input)** — `InputParser` returns `Result<T>` on every parse path. Bad input is expected, not exceptional, so errors are values and callers handle them explicitly.

**Line-by-line `Runner`** — `Runner.Run(TextReader, TextWriter, TextWriter)` reads a line at a time, validates it, runs the robot, and writes its result before reading the next. The brief says each robot is processed sequentially, so memory use does not grow with the number of robots. `Program.cs` is a one-line wrapper, and tests drive the same `Runner` with in-memory readers and writers.

**`HashSet<(int, int)>` for scents behind `Grid.TryAddScent`** — O(1) add and lookup. The grid is at most 51×51, so there are at most 2,601 scents. `TryAddScent` returns `true` only for the first robot lost at a point, so check and add are one step with no gap between them.

**Validation** — grid bounds and robot start positions must be integers within 0–50 and on the grid. Directions must be exactly one of N, E, S, W. Instruction lines may contain only registered commands. Lines must have exactly the expected number of tokens, split on any whitespace.

**xUnit** — `[Theory]` / `[InlineData]` for data-driven tests across orientations, corners and malformed input.

## Approach

Built test-first in layers: `Direction` → `Grid` → `Simulation` → `InputParser` → `Result<T>` → `Runner`. Each layer was committed separately once its tests passed.

## Assumptions

- **Scent is per grid point, not per direction.** The brief says scent prohibits robots "dropping off the world at the same grid point", so a scent at a corner protects both of its edges. Corner tests lock this in.
- **Invalid input aborts the run.** The brief defines no error handling. For a robot controller, guessing what a bad line meant is riskier than stopping, so an unknown command, a bad direction or a start position outside the grid stops processing with an error.
- **Instructions and directions are case-insensitive.** Lowercase is accepted and normalised to uppercase.
- **Blank lines.** Blank lines between robots (and before the grid) are skipped. A blank line where an instruction line is expected is an empty instruction, and the robot stays where it is.
- **Instruction string length < 100 characters is taken as given.** The brief states this as a guarantee, so it is not validated. Longer input still processes correctly.
- **Robot start positions must be on the grid.** The brief does not describe this case. The maximum coordinate is 50 and the grid cannot exceed it, so an on-grid check covers both.

## If This Were Going Further

- **API**: wrap `Runner`'s parse and simulate steps in a minimal ASP.NET Core endpoint accepting input as a request body.
- **Persistence**: store grid state and scent positions between requests (useful if robots are submitted one at a time). Scent would need an atomic "first one wins" write, such as a unique-key insert.
- **Concurrency**: scent order matters, so robots on one grid stay sequential. Separate grids could run in parallel. A shared grid would need `TryAddScent` backed by a `ConcurrentDictionary` or a lock.
- **Input modes**: accept a file path argument in addition to stdin.
- **Deployment**: single `dotnet publish` producing a self-contained binary; Dockerfile for containerised runs.

## AI Tooling

Claude (Anthropic) was used throughout: structuring the TDD plan, scaffolding boilerplate, suggesting edge cases, and reviewing design decisions (including the `Result<T>` pattern and command-dispatch approach). A later review of the code against the brief found problems that the first pass missed: `Enum.TryParse` accepting numeric directions, robot start positions not being validated, and blank instruction lines shifting the line pairing. These were fixed test-first.
