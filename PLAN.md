# Martian Robots — TDL Challenge Build Plan

## Problem Summary

Simulate robots moving on a bounded rectangular Mars grid (max 50×50).
Each robot has a position `(x, y)` and an orientation `(N/S/E/W)`.
Commands: `L` (turn left), `R` (turn right), `F` (move forward).

Key mechanic — **Scent**: when a robot falls off the grid it leaves a scent at
its last position. Any future robot that would fall off from a scented position
has that instruction silently ignored instead.

Input:
```
5 3          ← upper-right grid bounds (lower-left is always 0,0)
1 1 E        ← robot start position + orientation
RFRFRFRF     ← instruction string
...          ← repeat for each robot
```

Output: `x y orientation` per robot, with `LOST` appended if it fell off.

---

## C# Project Structure

```
MartianRobots/
  Direction.cs       ← N, E, S, W enum + TurnLeft / TurnRight / Move helpers
  RobotState.cs      ← record (X, Y, Direction, IsLost)
  Grid.cs            ← bounds + scent HashSet
  Simulation.cs      ← runs one robot through its instruction string
  InputParser.cs     ← parses raw string → grid size + list of (position, instructions)
  Program.cs         ← wires it all together, reads stdin, writes stdout

MartianRobots.Tests/
  DirectionTests.cs
  SimulationTests.cs
  InputParserTests.cs
  IntegrationTests.cs
```

---

## Core Data Models

```csharp
enum Direction { N, E, S, W }

record RobotState(int X, int Y, Direction Facing, bool IsLost);

class Grid
{
    public int MaxX { get; }
    public int MaxY { get; }
    private readonly HashSet<(int, int)> _scents = new();

    public bool IsOutOfBounds(int x, int y) => x < 0 || y < 0 || x > MaxX || y > MaxY;
    public bool HasScent(int x, int y) => _scents.Contains((x, y));
    public void AddScent(int x, int y) => _scents.Add((x, y));
}
```

---

## Scent Storage

- `HashSet<(int x, int y)>` — O(1) add and lookup.
- Max possible scents = grid area = 50 × 50 = 2,500. Trivially small.
- Scent is position-only. The "ignore" rule triggers whenever a `F` from that
  position would go out of bounds, regardless of direction.

---

## Command Extensibility

Commands are registered in a dictionary so new types can be added without
touching the simulation loop:

```csharp
var commands = new Dictionary<char, Func<RobotState, Grid, RobotState>>
{
    ['L'] = (state, _)    => state with { Facing = state.Facing.TurnLeft() },
    ['R'] = (state, _)    => state with { Facing = state.Facing.TurnRight() },
    ['F'] = (state, grid) => MoveForward(state, grid),
};
```

Adding a new command = one new dictionary entry.

---

## TDD Build Order

Work outside-in, smallest unit first:

| Step | Class | What to test |
|------|-------|-------------|
| 1 | `Direction` | TurnLeft / TurnRight from all 4 orientations |
| 2 | `Direction` | Move delta for each orientation (N→y+1, E→x+1, etc.) |
| 3 | `Grid` | IsOutOfBounds edge cases; AddScent / HasScent |
| 4 | `Simulation` | Single robot — basic moves, turns |
| 5 | `Simulation` | Robot falls off → LOST + scent added |
| 6 | `Simulation` | Scent prevents second robot falling off same edge |
| 7 | `InputParser` | Parse grid line, robot position line, instruction line |
| 8 | `InputParser` | Parse full multi-robot input block |
| 9 | Integration | Sample input → expected output exactly matches |

Framework: **xUnit**

---

## README Notes (to write at end)

- How to run (`dotnet run`)
- How to run tests (`dotnet test`)
- Technical choices: C#, xUnit, record types for immutable state, dictionary for extensible commands
- What would be built next: REST API wrapper, file/stdin input modes, persistence layer for scent state across sessions
- How AI tooling was used
