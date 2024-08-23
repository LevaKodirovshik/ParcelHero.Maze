using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Text;

namespace ParcelHero.Maze.API;

public class Maze
{
    // private constructor to prevent creation of invalid mazes
    private Maze(Point start, Point goal, bool[,] grid)
    {
        Start = start;
        Goal = goal;
        Grid = grid;
    }

    public bool this[int x, int y] => Grid[y, x];
    public bool this[Point point] => Grid[point.Y, point.X];

    public int Height => Grid.GetLength(0);

    public int Width => Grid.GetLength(1);

    public Point Start { get; }
    public Point Goal { get; }
    // private to prevent modification
    private bool[,] Grid { get; }

    public static bool TryParse(string input, [NotNullWhen(true)] out Maze? maze, [NotNullWhen(false)] out string? error)
    {
        // accept any line terminator
        var lines = input?.Split(['\r', '\n'], options: StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (!(lines?.Length > 0))
        {
            error = "Input is empty, after clearing white space and new lines";
            maze = null;
            return false;
        }

        if (lines.Length > 20 || lines[0].Length > 20)
        {
            error = "Input is too large, max size is 20x20";
            maze = null;
            return false;
        }

        if (lines.Any(x => x.Length != lines[0].Length))
        {
            error = "Input is not a rectangle";
            maze = null;
            return false;
        }

        var grid = new bool[lines.Length, lines[0].Length];
        Point? start = null;
        Point? goal = null;
        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                var currentPoint = new Point(x, y);
                switch (lines[y][x])
                {
                    case 'S':
                        if (start is not null)
                        {
                            error = $"Multiple start points found: {start}, {currentPoint}";
                            maze = null;
                            return false;
                        }
                        start = currentPoint;
                        grid[y, x] = true;
                        break;
                    case 'G':
                        if (goal is not null)
                        {
                            error = $"Multiple goal points found: {goal}, {currentPoint}";
                            maze = null;
                            return false;
                        }
                        goal = currentPoint;
                        grid[y, x] = true;
                        break;
                    case '_':
                        grid[y, x] = true;
                        break;
                    case 'X':
                        grid[y, x] = false;
                        break;
                    default:
                        maze = null;
                        error = $"Invalid character '{lines[y][x]}' at ({x}, {y})";
                        return false;
                }
            }
        }

        if (start is null)
        {
            error = "No start point found";
            maze = null;
            return false;
        }

        if (goal is null)
        {
            error = "No goal point found";
            maze = null;
            return false;
        }

        maze = new Maze(start.Value, goal.Value, grid);
        error = null;
        return true;
    }

    public override string ToString() => RenderPath(this, []);
    public string RenderPath(IEnumerable<Point> path) => RenderPath(this, path);
    public static string RenderPath(Maze maze, IEnumerable<Point> path)
    {
        var pathHashSet = (path as HashSet<Point>) ?? path.ToHashSet();
        var sb = new StringBuilder();
        sb.AppendLine();
        for (int y = 0; y < maze.Grid.GetLength(0); y++)
        {
            for (int x = 0; x < maze.Grid.GetLength(1); x++)
            {
                if (maze.Start == new Point(x, y))
                {
                    sb.Append('S');
                }
                else if (maze.Goal == new Point(x, y))
                {
                    sb.Append('G');
                }
                else if (pathHashSet.Contains(new Point(x, y)))
                {
                    sb.Append('*');
                }
                else
                {
                    sb.Append(maze.Grid[y, x] ? '_' : 'X');
                }
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }
}