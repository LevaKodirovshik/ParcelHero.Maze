namespace ParcelHero.Maze.API;

public class PathfindingAlgorithmProvider : IPathfindingAlgorithmProvider
{
    private readonly IServiceProvider _serviceProvider;

    public PathfindingAlgorithmProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IPathfindingAlgorithm GetAlgorithm(PathfindingAlgorithmName algorithm) => _serviceProvider.GetKeyedService<IPathfindingAlgorithm>(algorithm);
}
