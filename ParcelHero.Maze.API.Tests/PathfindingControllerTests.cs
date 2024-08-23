using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using Xunit;

namespace ParcelHero.Maze.API.Tests;

public class PathfindingControllerTests
{
    private readonly Mock<IPathfindingAlgorithmProvider> _algorithmProviderMock;
    private readonly Mock<IPathfindingCache> _cacheMock;
    private readonly PathfindingController _controller;

    public PathfindingControllerTests()
    {
        _algorithmProviderMock = new Mock<IPathfindingAlgorithmProvider>();
        _algorithmProviderMock.Setup(p => p.GetAlgorithm(PathfindingAlgorithmName.Basic)).Returns(new BasicPathfindingAlgorithm());
        _cacheMock = new Mock<IPathfindingCache>();

        var pathfindingCache = new PathfindingCache();

        _cacheMock.Setup(c => c.Get(It.IsAny<PathfindingAlgorithmName>(), It.IsAny<string>()))
            .Returns((PathfindingAlgorithmName algorithm, string maze) => pathfindingCache.Get(algorithm, maze));

        _cacheMock.Setup(c => c.GetAll()).Returns(() => pathfindingCache.GetAll());
        _cacheMock.Setup(c => c.AddToCache(It.IsAny<PathfindingAlgorithmName>(), It.IsAny<string>(), It.IsAny<string>()))
            .Callback((PathfindingAlgorithmName algorithm, string maze, string path) => pathfindingCache.AddToCache(algorithm, maze, path));

        _controller = new PathfindingController(_algorithmProviderMock.Object, _cacheMock.Object);
    }

    [Fact]
    public void SolvableMaze_ReturnsOkResultWithRenderedPath()
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
        var renderedPath = parsedMaze.RenderPath(path);

        var result = _controller.Post(mazeString, PathfindingAlgorithmName.Basic);

        Assert.IsType<OkObjectResult>(result);
        var objectResult = (OkObjectResult)result;
        Assert.Equal(renderedPath, objectResult.Value);
        _algorithmProviderMock.Verify(p => p.GetAlgorithm(PathfindingAlgorithmName.Basic), Times.Once);
        _algorithmProviderMock.VerifyNoOtherCalls();
        _cacheMock.Verify(cache => cache.Get(PathfindingAlgorithmName.Basic, mazeString), Times.Once);
        _cacheMock.Verify(cache => cache.AddToCache(PathfindingAlgorithmName.Basic, mazeString, renderedPath), Times.Once);
        _cacheMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void UnsolvableMaze_Returns404()
    {
        var mazeString = @"
S____
_____
XXXXX
_____
____G
";
        var result = _controller.Post(mazeString, PathfindingAlgorithmName.Basic);

        Assert.IsType<NotFoundObjectResult>(result);
        _algorithmProviderMock.Verify(p => p.GetAlgorithm(PathfindingAlgorithmName.Basic), Times.Once);
        _algorithmProviderMock.VerifyNoOtherCalls();
        _cacheMock.Verify(cache => cache.Get(PathfindingAlgorithmName.Basic, mazeString), Times.Once);
        _cacheMock.Verify(cache => cache.AddToCache(PathfindingAlgorithmName.Basic, mazeString, null), Times.Once);
        _cacheMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void InvalidMaze_Returns400BadRequest()
    {
        var mazeString = @"blah";
        var result = _controller.Post(mazeString, PathfindingAlgorithmName.Basic);

        Assert.IsType<BadRequestObjectResult>(result);
        _algorithmProviderMock.VerifyNoOtherCalls();
        _cacheMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void InvalidAlgorithm_Returns400BadRequest()
    {
        var mazeString = @"
S____
_____
XXX__
_____
____G
";
        var result = _controller.Post(mazeString, 0);

        Assert.IsType<BadRequestObjectResult>(result);
        _algorithmProviderMock.VerifyNoOtherCalls();
        _cacheMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void GetAllMazes_Returns204WhenEmpty()
    {
        var result = _controller.Get();
        Assert.IsType<NoContentResult>(result);
        _algorithmProviderMock.VerifyNoOtherCalls();
        _cacheMock.Verify(cache => cache.GetAll(), Times.Once);
        _cacheMock.VerifyNoOtherCalls();
    }
    [Fact]
    public void GetAllMazes_Returns200AfterPostingMazes()
    {
        _controller.Post(@"
S____
_____
XXX__
_____
____G
", PathfindingAlgorithmName.Basic);
        _controller.Post(@"
S____
_____
XXXXX
_____
____G
", PathfindingAlgorithmName.Basic);
        var result = _controller.Get();
        Assert.IsType<OkObjectResult>(result);
        var objectResult = (OkObjectResult)result;
        Assert.IsType<GetAllMazesResponse>(objectResult.Value);
        var response = (GetAllMazesResponse)objectResult.Value;
        Assert.Equal(2, response.Mazes.Length);

        _algorithmProviderMock.Verify(p => p.GetAlgorithm(PathfindingAlgorithmName.Basic), Times.Exactly(2));
        _algorithmProviderMock.VerifyNoOtherCalls();
        _cacheMock.Verify(cache => cache.Get(PathfindingAlgorithmName.Basic, It.IsAny<string>()), Times.Exactly(2));
        _cacheMock.Verify(cache => cache.AddToCache(PathfindingAlgorithmName.Basic, It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        _cacheMock.Verify(cache => cache.GetAll(), Times.Once);
        _cacheMock.VerifyNoOtherCalls();
    }
}
