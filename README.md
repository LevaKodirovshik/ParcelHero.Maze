# ParcelHero.Maze

## Running from Visual Studio

Open `.\ParcelHero.Maze.sln` is Visual Studio 2022 and run the solution.
Visual Studio will automatically open a browser with the Swagger page.

## From command line

In the project root directory run the following command:

```bash
dotnet run --project ./ParcelHero.Maze.API/ParcelHero.Maze.API.csproj
```

Then open this uri in your browser: <http://localhost:5192/swagger/index.html>

## Through Docker

In the project root directory run the following commands:

```bash
docker build -t parcelhero.maze -f ./ParcelHero.Maze.API/Dockerfile .
docker run parcelhero.maze
```

Then open this uri in your browser: <http://localhost:8080/swagger/index.html>