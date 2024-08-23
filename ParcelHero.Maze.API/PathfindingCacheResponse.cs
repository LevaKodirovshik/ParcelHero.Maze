using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace ParcelHero.Maze.API;

public abstract class PathfindingCacheResponse
{ }

/// <summary>
/// Response when a maze is found in the cache, either solved or usolvable
/// </summary>
public class PathfindingCacheHit : PathfindingCacheResponse
{
    /// <summary>
    /// Cache response for an unsolvable maze
    /// </summary>
    public PathfindingCacheHit()
    {
        Path = null;
    }

    /// <summary>
    /// Cache response for a solved maze
    /// </summary>
    public PathfindingCacheHit(string path)
    {
        Path = path;
    }

    /// <summary>
    /// Will be null if the maze is unsolvable
    /// </summary>
    public string? Path { get; }

    [MemberNotNullWhen(true, nameof(Path))]
    public bool PathFound => Path != null;
}

/// <summary>
/// Maze not found in the cache
/// </summary>
public class PathfindingCacheMiss : PathfindingCacheResponse
{ }