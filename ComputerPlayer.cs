using System;
using System.Collections.Generic;

class ComputerPlayer
{
    private static Random random = new Random();

    public static bool MakeMove(GameState game)
    {
        List<Move> legalMoves = game.GetAllLegalMoves(false);

        if (legalMoves.Count == 0)
        {
            return false;
        }

        Move chosenMove = legalMoves[random.Next(legalMoves.Count)];

        return game.MovePiece(
            chosenMove.FromRow,
            chosenMove.FromCol,
            chosenMove.ToRow,
            chosenMove.ToCol
        );
    }
}