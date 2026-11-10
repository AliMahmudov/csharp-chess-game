# C# Chess Game

A desktop chess game built with **C#, .NET 9 and GtkSharp** as a semester project. Play as White against a computer opponent that selects a random legal move for Black.

The chess rules are implemented in C# without an external chess engine. The project demonstrates board-state modelling, move generation, validation and an event-driven graphical interface.

## Features

- Click a piece and its destination on an interactive 8 by 8 board.
- Validate movement and captures for all six piece types, including blocked paths.
- Reject moves that leave the moving player's king in check.
- Detect check and checkmate.
- Automatically promote pawns to queens.
- Generate legal moves for the computer opponent.
- Highlight the selected square and display turn and game-status messages.

## How it works

`GameState` stores the board in a two-dimensional array of `Piece` values. Before accepting a move, it checks piece ownership, movement rules and the destination, then temporarily applies the move to verify king safety. Invalid moves restore the previous position.

`ChessWindow` handles the GTK interface and click events. `ComputerPlayer` obtains Black's legal moves from `GameState`, chooses one at random and applies it through the same validation method used for the human player.

## Build and run

### Prerequisites

- The .NET 9 SDK, matching the project's `net9.0` target.
- A native GTK 3 runtime available to the application when launching the GUI. GtkSharp is the .NET binding; restoring the NuGet package alone does not guarantee that the native GTK libraries are installed. See the [GtkSharp project and setup resources](https://github.com/GtkSharp/GtkSharp).
- A graphical desktop session and a font that can display Unicode chess symbols.

```bash
git clone https://github.com/AliMahmudov/csharp-chess-game.git
cd csharp-chess-game
dotnet restore ChessGame.csproj
dotnet build ChessGame.csproj --configuration Release --no-restore
dotnet run --project ChessGame.csproj
```

If launching reports a missing GTK, GLib or GDK native library, check the GTK installation and that its libraries match the architecture of your .NET process.

### Playing

1. Select a white piece, then click a destination square.
2. If the move is illegal, select a piece and try again.
3. After a legal move, the computer responds as Black.
4. Close and relaunch the application to start a new game.

## Project structure

| File | Responsibility |
| --- | --- |
| `GameState.cs` | Board state, move validation, king safety, checkmate and promotion |
| `ChessWIndow.cs` | GTK board, selection, rendering and status messages |
| `ComputerPlayer.cs` | Random selection from Black's legal moves |
| `Move.cs` | Source and destination coordinates |
| `Piece.cs` | Piece definitions |
| `Program.cs` | Application startup |
| `ProjectDocumentation.docx` | Original semester-project report |

## Current limitations

- Castling and en passant are not implemented.
- Draw rules, including stalemate, repetition and the fifty-move rule, are not implemented.
- Promotion always produces a queen.
- The computer uses random legal moves; it has no search, position evaluation or difficulty settings.
- There is no restart button or saved-game support.

## Build verification

The GitHub Actions workflow restores dependencies and builds the Release configuration. It checks compilation; it does not launch or test the GTK interface.

## Author

[Ali Mahmudov](https://github.com/AliMahmudov)
