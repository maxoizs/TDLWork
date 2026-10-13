using MartianRobots;

var input = Console.In.ReadToEnd();
var (grid, robots) = InputParser.Parse(input);
var sim = new Simulation();

foreach (var robot in robots)
{
    var final = sim.Execute(robot.State, robot.Instructions, grid);
    var lost = final.IsLost ? " LOST" : "";
    Console.WriteLine($"{final.X} {final.Y} {final.Facing}{lost}");
}
