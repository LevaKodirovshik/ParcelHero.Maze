namespace ParcelHero.Maze.API.Tests
{
    public class MazeTests
    {
        [Fact]
        public void NormalMazeCorrectlyParsed()
        {
            var mazeString = @"
S____
_____
XXX__
_____
____G
";
            var parseResult = Maze.TryParse(mazeString, out var maze, out var error);

            Assert.True(parseResult, $"A normal 5x5 maze must be parsed, produced error: {error ?? "NULL"}");
            Assert.NotNull(maze);
            Assert.Null(error);
            Assert.Equal(5, maze.Width);
            Assert.Equal(5, maze.Height);
            var mazeToString = maze.ToString().Trim();
            // .Trim() to remove the starting and trailing new lines, they are not importaint
            Assert.Equal(mazeString.Trim(), mazeToString.Trim());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("blah")]
        // no goal
        [InlineData(@"
S____
_____
XXX__
_____
_____
")]
        // no start
        [InlineData(@"
G____
_____
XXX__
_____
_____
")]
        // no start and no goal
        [InlineData(@"
_____
_____
XXX__
_____
_____
")]
        // multiple start 
        [InlineData(@"
S___S
_____
XXX__
_____
_____
")]
        // multiple goals
        [InlineData(@"
G___G
_____
XXX__
_____
_____
")]
        // invalid character
        [InlineData(@"
S___*
_____
XXX__
_____
_____
")]
        // not a rectangle
        [InlineData(@"
S____
_____
XXX___
_____
____G
")]
        // too large
        [InlineData(@"
S________________________
_________________________
XXX______________________
_________________________
____G____________________
")]
        public void InvalidInputProducesError(string mazeInput)
        {
            var parseResult = Maze.TryParse(mazeInput, out var maze, out var error);

            Assert.False(parseResult, "Invalid input should not be parsed");
            Assert.Null(maze);
            Assert.NotNull(error);
        }
    }
}