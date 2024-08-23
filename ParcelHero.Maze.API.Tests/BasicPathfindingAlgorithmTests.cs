using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParcelHero.Maze.API.Tests;

public class BasicPathfindingAlgorithmTests
{
    [Theory]
    [InlineData(@"
S____
_____
XXX__
_____
____G
")]
    [InlineData(@"
S_________
__________
XXX_______
____XXXXXX
_XXXXXXXXX
__________
____G_____
")]
    [InlineData(@"
S_________
__________
XXX_______
____X____X
XXXXX___XX
_____XX___
____G_____
")]

    public void FindPathInSolvableMaze(string mazeString)
    {
        Maze.TryParse(mazeString, out var maze, out _);
        var pathFound = BasicPathfindingAlgorithm.Instance.TryFindPath(maze, out var path);
        Assert.True(pathFound);
        Assert.NotNull(path);
        Assert.NotEmpty(path);
        Assert.Equal(maze.Start, path.First());
        Assert.Equal(maze.Goal, path.Last());
        Assert.All(path, p => Assert.True(maze[p]));

        // check that all points are one after the other
        // pair each point in the path with the next point, and then calculates the distance between each pair of points. 
        // check that all calculated distances are equal to 1
        Assert.True(path.Zip(path.Skip(1), (a, b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y) == 1).All(x => x));
        // check that there are no duplicates
        Assert.Equal(path.Count, path.Distinct().Count());
        var pathString = Maze.RenderPath(maze, path);
    }
    [Theory]
    [InlineData(@"
S____
_____
XXXXX
_____
____G
")]
    [InlineData(@"
S_________
__________
XXX_______
____XXXXXX
XXXXXXXXXX
__________
____G_____
")]
    [InlineData(@"
S_________
__________
XXX_______
____X____X
XXXXX___XX
_____XXX__
____G_____
")]

    public void DoNotFindPathInSolvableMaze(string mazeString)
    {
        Maze.TryParse(mazeString, out var maze, out _);
        var pathFound = BasicPathfindingAlgorithm.Instance.TryFindPath(maze, out var path);
        Assert.False(pathFound);
        Assert.Null(path);
    }
}
