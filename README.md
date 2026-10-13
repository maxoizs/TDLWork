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

## Running Tests

```bash
dotnet test
```

112 tests covering unit, edge case, and integration scenarios.

## Technical Choices

**C# with record types** — `RobotState` is an immutable record. Each command returns a new state rather than mutating, making the simulation easy to reason about and test.

**Dictionary-based command dispatch** — commands (`L`, `R`, `F`) are registered in a `static readonly Dictionary<char, Func<...>>`. Adding a new command type requires one dictionary entry with no changes to the simulation loop.

**`Result<T>` pattern (no exceptions on bad input)** — `InputParser` returns `Result<T>` on every parse path. No exception-driven flow: all error cases are values, making callers explicit about success/failure without try/catch.

**`HashSet<(int, int)>` for scents** — O(1) add and lookup. The grid is at most 50×50 so the maximum possible scents is 2,500 — no complex data structure needed.

**Coordinate validation** — grid bounds are validated on parse (0–50 per axis per the spec). Malformed input exits with a descriptive error message to stderr and exit code 1.

**xUnit** — straightforward test framework with `[Theory]` / `[InlineData]` for data-driven tests across all orientations and boundary cases.

## Approach

Built test-first in layers: `Direction` → `Grid` → `Simulation` → `InputParser` → `Result<T>` → integration. Each layer was committed separately once all its tests passed.

## Assumptions

- **Unknown commands are silently ignored.** The spec defines L, R, and F; any other character in an instruction string is skipped. This preserves extensibility — new commands can be registered in the dispatch dictionary without changing the parsing or execution loop.
- **Direction parsing is case-insensitive.** The spec shows uppercase (N, S, E, W); lowercase is also accepted as a defensive choice with no downside.
- **Instruction string length < 100 characters is taken as given.** The spec states this as a guarantee, so it is not validated. Input longer than 100 characters will still process correctly.
- **Robot starting positions are not validated against grid bounds.** The spec does not describe this as an error case; a robot starting outside the grid would fall off immediately on its first move.

## If This Were Going Further

- **API**: wrap in a minimal ASP.NET Core endpoint accepting input as a request body
- **Persistence**: store grid state and scent positions between requests (useful if robots are submitted one at a time)
- **Input modes**: accept a file path argument in addition to stdin
- **Deployment**: single `dotnet publish` producing a self-contained binary; Dockerfile for containerised runs

## AI Tooling

Claude (Anthropic) was used throughout: structuring the TDD plan, scaffolding boilerplate, suggesting edge cases, and reviewing design decisions (including the `Result<T>` pattern and command-dispatch approach). All logic and test assertions were reviewed and understood before accepting. The scent mechanic, `Result<T>` error handling strategy, and command extensibility pattern were reasoned through in conversation before implementation.
