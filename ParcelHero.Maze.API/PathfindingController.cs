using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Net.Mime;
using System.Security.Cryptography;

namespace ParcelHero.Maze.API;

[ApiController]
[Route("api/[controller]")]
public class PathfindingController : ControllerBase
{
    private readonly IPathfindingAlgorithmProvider _pathfindingAlgorithmProvider;
    private readonly IPathfindingCache _pathfindingCache;

    public PathfindingController(IPathfindingAlgorithmProvider pathfindingAlgorithmProvider, IPathfindingCache pathfindingCache)
    {
        _pathfindingAlgorithmProvider = pathfindingAlgorithmProvider;
        _pathfindingCache = pathfindingCache;
    }

    [Consumes(MediaTypeNames.Text.Plain)]
    [Produces(MediaTypeNames.Text.Plain)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost("findPath/{algorithm}")]
    public ActionResult Post([FromBody] string maze, [FromRoute] PathfindingAlgorithmName algorithm)
    {
        if (!Enum.IsDefined<PathfindingAlgorithmName>(algorithm))
        {
            return BadRequest("Invalid algorithm");
        }

        if (!Maze.TryParse(maze, out var parsedMaze, out var error))
        {
            return BadRequest(error);
        }

        var sanitizedMaze = parsedMaze.ToString();

        var cacheResponse = _pathfindingCache.Get(algorithm, sanitizedMaze);
        if (cacheResponse is PathfindingCacheHit hit)
        {
            if (!hit.PathFound)
            {
                return NotFound("No path found");
            }

            return Ok(hit.Path);
        }

        var pathfindingAlgorithm = _pathfindingAlgorithmProvider.GetAlgorithm(algorithm);
        pathfindingAlgorithm.TryFindPath(parsedMaze, out var path);


        if (path is null)
        {
            _pathfindingCache.AddToCache(algorithm, sanitizedMaze, null);
            return NotFound("No path found");
        }

        var renderedPath = parsedMaze.RenderPath(path);
        _pathfindingCache.AddToCache(algorithm, sanitizedMaze, renderedPath);
        return Ok(renderedPath);
    }

    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [Produces<GetAllMazesResponse>()]
    [HttpGet("getAllMazes")]
    public ActionResult Get()
    {
        var allMazes = _pathfindingCache.GetAll();
        if (allMazes.Count == 0)
        {
            return NoContent();
        }

        return Ok(new GetAllMazesResponse
        {
            Mazes = allMazes.Select(mazeAlgorithms =>
            new MazeWithPaths
            {
                Maze = mazeAlgorithms.Key,
                Unsolvable = mazeAlgorithms.Value is null,
                Paths = mazeAlgorithms.Value?.Select(algorithmPaths =>
                    new AlgorithmPath
                    {
                        Algorithm = algorithmPaths.Key,
                        Path = algorithmPaths.Value
                    }).ToArray(),
            }
            ).ToArray()
        });
    }

}
