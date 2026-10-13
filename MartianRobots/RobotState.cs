namespace MartianRobots;

/// <summary>Immutable snapshot of a robot's position, orientation, and lost status.</summary>
/// <param name="X">Grid x-coordinate.</param>
/// <param name="Y">Grid y-coordinate.</param>
/// <param name="Facing">Direction the robot is currently facing.</param>
/// <param name="IsLost">True if the robot has fallen off the grid.</param>
public record RobotState(int X, int Y, Direction Facing, bool IsLost = false);
