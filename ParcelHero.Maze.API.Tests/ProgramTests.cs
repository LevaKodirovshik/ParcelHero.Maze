using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ParcelHero.Maze.API.Tests;

public class ProgramTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ProgramTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PostMaze_ReturnsOk()
    {
        var mazeString = @"
S____
_____
XXX__
_____
____G
";
        Maze.TryParse(mazeString, out var parsedMaze, out _);
        BasicPathfindingAlgorithm.Instance.TryFindPath(parsedMaze, out var path);
        var expectedPath = parsedMaze.RenderPath(path);

        var client = _factory.CreateClient();
        var content = new StringContent(mazeString, Encoding.UTF8, "text/plain");
        var response = await client.PostAsync($"api/Pathfinding/findPath/{PathfindingAlgorithmName.Basic}", content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var stringResponse = await response.Content.ReadAsStringAsync();
        Assert.Equal(expectedPath, stringResponse);

    }
}
