using MartianRobots;

var input = Console.In.ReadToEnd();
var parsed = InputParser.Parse(input);

if (!parsed.IsSuccess)
{
    Console.Error.WriteLine($"Error: {parsed.Error}");
    Environment.Exit(1);
}

var (grid, robots) = parsed.Value;
var sim = new Simulation();

foreach (var robot in robots)
{
    var final = sim.Execute(robot.State, robot.Instructions, grid);
    var lost = final.IsLost ? " LOST" : "";
    Console.WriteLine($"{final.X} {final.Y} {final.Facing}{lost}");
}
