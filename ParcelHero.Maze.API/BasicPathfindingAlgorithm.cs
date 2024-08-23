using System.Drawing;

namespace ParcelHero.Maze.API;

public class BasicPathfindingAlgorithm : IPathfindingAlgorithm
{
    public static BasicPathfindingAlgorithm Instance { get; } = new BasicPathfindingAlgorithm();

    public bool TryFindPath(Maze maze, out IList<Point>? path)
    {
        if (maze is null)
        {
            throw new ArgumentNullException(nameof(maze));
        }

        var possiblePath = new List<Point> { maze.Start };
        var traveledPoints = new HashSet<Point>();
        if (TryFindPathRecursive(maze, maze.Start, possiblePath, traveledPoints))
        {
            path = possiblePath;
            return true;
        }
        path = null;
        return false;
    }

    private bool TryFindPathRecursive(Maze maze, Point currentPoint, List<Point> path, HashSet<Point> traveledPoints)
    {
        // if we reached the goal, we are done
        if (currentPoint == maze.Goal)
        {
            return true;
        }
        // make sure we don't go in circles
        traveledPoints.Add(currentPoint);

        var directions = new[]
        {
            // right
            new Point(0, 1),
            // down
            new Point(1, 0),
            // left
            new Point(0, -1),
            // up
            new Point(-1, 0)
        };
        // check all directions
        foreach (var direction in directions)
        {
            var nextPoint = new Point(currentPoint.X + direction.X, currentPoint.Y + direction.Y);
            // check that the next point is inside the maze
            if (nextPoint.X < 0 || nextPoint.X >= maze.Width || nextPoint.Y < 0 || nextPoint.Y >= maze.Height)
            {
                continue;
            }
            // check that we haven't been there before and that it is not a wall
            if (traveledPoints.Contains(nextPoint) || !maze[nextPoint])
            {
                continue;
            }
            // add the point to the path and try to find the path from there
            path.Add(nextPoint);
            // point is added to traveledPoints inside the TryFindPathRecursive method
            if (TryFindPathRecursive(maze, nextPoint, path, traveledPoints))
            {
                // we found the path to the goal return
                return true;
            }
            // we didn't find the path to the goal return, remove the point from the path and check next direction
            path.RemoveAt(path.Count - 1);
        }
        // there is no path from this point, return false
        // this will cause the caller to backtrack and check for other directions from the previous point(s)
        return false;
    }

}
