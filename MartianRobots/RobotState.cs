namespace MartianRobots;

public record RobotState(int X, int Y, Direction Facing, bool IsLost = false);
