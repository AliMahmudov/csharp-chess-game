# C# Chess Game

A desktop chess application built in C# using GTK.

The project implements chess movement and game-state logic from scratch, including legal move validation, check detection, checkmate detection, pawn promotion, and a computer-controlled opponent.

## Features

- Interactive 8×8 chess board
- Legal move validation for all standard chess pieces
- Turn-based gameplay
- Check detection
- Checkmate detection
- Prevents moves that leave the player's king in check
- Automatic pawn promotion to queen
- Computer opponent
- GTK graphical user interface

## Technologies

- C#
- .NET
- GTK
- Object-Oriented Programming

## Project Structure

- `GameState.cs` — board state, chess rules, move validation, check and checkmate logic
- `ChessWindow.cs` — GTK graphical interface
- `ComputerPlayer.cs` — computer-controlled opponent
- `Move.cs` — represents chess moves
- `Piece.cs` — chess piece definitions
- `Program.cs` — application entry point

## Running the Project

Clone the repository:

```bash
git clone https://github.com/AliMahmudov/csharp-chess-game.git
cd csharp-chess-game
```

Restore dependencies and run the application:

```bash
dotnet restore
dotnet run
```

