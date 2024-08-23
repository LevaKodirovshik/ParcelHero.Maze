using System.Text.Json.Serialization;

namespace ParcelHero.Maze.API;

public class GetAllMazesResponse
{
    public MazeWithPaths[] Mazes { get; set; }
}

public class MazeWithPaths
{
    public string Maze { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AlgorithmPath[] Paths { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Unsolvable { get; set; }
}

public class AlgorithmPath
{
    public PathfindingAlgorithmName Algorithm { get; set; }
    public string Path { get; set; }
}
