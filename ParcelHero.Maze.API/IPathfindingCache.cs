using System.Drawing;

namespace ParcelHero.Maze.API;

/// <summary>
/// Cache assumes that if a maze is solvable, it will be solvable by all algorithms
/// </summary>
public interface IPathfindingCache
{

    /// <summary>
    /// Add a path to the cache
    /// </summary>
    /// <param name="algorithm"></param>
    /// <param name="maze"></param>
    /// <param name="path">null if maze is unsolvable</param>
    void AddToCache(PathfindingAlgorithmName algorithm, string maze, string? path);
    PathfindingCacheResponse Get(PathfindingAlgorithmName algorithm, string maze);

    public IDictionary<string, IDictionary<PathfindingAlgorithmName, string>> GetAll();
}