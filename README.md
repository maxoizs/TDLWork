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

73 tests covering unit, edge case, and integration scenarios.

## Technical Choices

**C# with record types** — `RobotState` is an immutable record. Each command returns a new state rather than mutating, making the simulation easy to reason about and test.

**Dictionary-based command dispatch** — commands (`L`, `R`, `F`) are registered in a `Dictionary<char, Func<...>>`. Adding a new command type requires one dictionary entry with no changes to the simulation loop.

**`HashSet<(int, int)>` for scents** — O(1) add and lookup. The grid is at most 50×50 so the maximum possible scents is 2,500 — no complex data structure needed.

**xUnit** — straightforward test framework with `[Theory]` / `[InlineData]` for data-driven tests across all orientations and boundary cases.

## Approach

Built test-first in layers: `Direction` → `Grid` → `Simulation` → `InputParser` → integration. Each layer was committed separately once all its tests passed.

## If This Were Going Further

- **API**: wrap in a minimal ASP.NET Core endpoint accepting input as a request body
- **Persistence**: store grid state and scent positions between requests (useful if robots are submitted one at a time)
- **Input modes**: accept a file path argument in addition to stdin
- **Deployment**: single `dotnet publish` producing a self-contained binary; Dockerfile for containerised runs

## AI Tooling

Claude was used to help structure the TDD plan, generate boilerplate, and suggest edge cases for tests. All logic, design decisions, and test assertions were reviewed and understood before accepting. The scent mechanic and command extensibility pattern were reasoned through independently before implementation.
