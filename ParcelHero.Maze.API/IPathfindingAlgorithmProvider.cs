namespace ParcelHero.Maze.API;

public interface IPathfindingAlgorithmProvider
{
    IPathfindingAlgorithm GetAlgorithm(PathfindingAlgorithmName algorithm);
}
