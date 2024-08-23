using System.Drawing;

namespace ParcelHero.Maze.API;

public interface IPathfindingAlgorithm
{
    public bool TryFindPath(Maze maze, out IList<Point>? path);
}
