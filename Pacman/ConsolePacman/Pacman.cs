public class Pacman
{
    private readonly World _world;
    public Position Position { get; set; }
    public Direction Direction { get; set; }

    public Pacman(World world)
    {
        _world = world;
        Position = _world.PacmanStartPosition;
        Direction = Direction.None;
    }
}