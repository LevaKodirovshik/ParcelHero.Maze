using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;

namespace ParcelHero.Maze.API;

/// <summary>
/// Cache assumes that if a maze is solvable, it will be solvable by all algorithms
/// </summary>
public class PathfindingCache : IPathfindingCache
{
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<PathfindingAlgorithmName, string>> _pathfindingCache = new();

    public PathfindingCacheResponse Get(PathfindingAlgorithmName algorithm, string maze)
    {
        if (!_pathfindingCache.TryGetValue(maze.ToString(), out var algorithmCache))
        {
            return new PathfindingCacheMiss();
        }

        if (algorithmCache == null)
        {
            return new PathfindingCacheHit();
        }

        if (algorithmCache.TryGetValue(algorithm, out var path))
        {
            return new PathfindingCacheHit(path);
        }
        return new PathfindingCacheMiss();
    }

    /// <summary>
    /// Add a path to the cache
    /// </summary>
    /// <param name="algorithm"></param>
    /// <param name="maze"></param>
    /// <param name="path">null if maze is unsolvable</param>
    public void AddToCache(PathfindingAlgorithmName algorithm, string maze, string path)
    {
        if (path == null)
        {
            _pathfindingCache[maze] = null;
            return;
        }

        var algorithmCache = _pathfindingCache.GetOrAdd(maze, _ => new ConcurrentDictionary<PathfindingAlgorithmName, string>());
        algorithmCache[algorithm] = path;
    }

    public IDictionary<string, IDictionary<PathfindingAlgorithmName, string>> GetAll()
        => _pathfindingCache.ToDictionary(kvp => kvp.Key, kvp => (IDictionary<PathfindingAlgorithmName, string>)kvp.Value?.ToDictionary());

}

