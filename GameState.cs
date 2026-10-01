using System;
using System.Collections.Generic;
class GameState
{
    public Piece[,] Board { get; private set; }
    public bool WhiteTurn { get; private set; }

    public GameState()
    {
        Board = new Piece[8, 8];
        WhiteTurn = true;
        InitializeBoard();
    }

    public void InitializeBoard()
    {
        // First make every square empty
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                Board[row, col] = Piece.Empty;
            }
        }

        // Black pieces
        Board[0, 0] = Piece.BlackRook;
        Board[0, 1] = Piece.BlackKnight;
        Board[0, 2] = Piece.BlackBishop;
        Board[0, 3] = Piece.BlackQueen;
        Board[0, 4] = Piece.BlackKing;
        Board[0, 5] = Piece.BlackBishop;
        Board[0, 6] = Piece.BlackKnight;
        Board[0, 7] = Piece.BlackRook;

        for (int col = 0; col < 8; col++)
        {
            Board[1, col] = Piece.BlackPawn;
        }

        // White pieces
        for (int col = 0; col < 8; col++)
        {
            Board[6, col] = Piece.WhitePawn;
        }

        Board[7, 0] = Piece.WhiteRook;
        Board[7, 1] = Piece.WhiteKnight;
        Board[7, 2] = Piece.WhiteBishop;
        Board[7, 3] = Piece.WhiteQueen;
        Board[7, 4] = Piece.WhiteKing;
        Board[7, 5] = Piece.WhiteBishop;
        Board[7, 6] = Piece.WhiteKnight;
        Board[7, 7] = Piece.WhiteRook;
    }

    public void PrintBoard()
    {
        Console.WriteLine();
        Console.WriteLine("  0 1 2 3 4 5 6 7");

        for (int row = 0; row < 8; row++)
        {
            Console.Write(row + " ");

            for (int col = 0; col < 8; col++)
            {
                Console.Write(PieceToChar(Board[row, col]) + " ");
            }

            Console.WriteLine();
        }

        Console.WriteLine();
        Console.WriteLine(WhiteTurn ? "White's turn" : "Black's turn");
    }

    private char PieceToChar(Piece piece)
    {
        switch (piece)
        {
            case Piece.WhitePawn: return 'P';
            case Piece.WhiteRook: return 'R';
            case Piece.WhiteKnight: return 'N';
            case Piece.WhiteBishop: return 'B';
            case Piece.WhiteQueen: return 'Q';
            case Piece.WhiteKing: return 'K';

            case Piece.BlackPawn: return 'p';
            case Piece.BlackRook: return 'r';
            case Piece.BlackKnight: return 'n';
            case Piece.BlackBishop: return 'b';
            case Piece.BlackQueen: return 'q';
            case Piece.BlackKing: return 'k';

            default: return '.';
        }
    }

    private bool IsInsideBoard(int row, int col)
    {
        return row >= 0 && row < 8 && col >= 0 && col < 8;
    }

    private bool IsWhitePiece(Piece piece)
    {
        return piece >= Piece.WhitePawn && piece <= Piece.WhiteKing;
    }

    private bool IsBlackPiece(Piece piece)
    {
        return piece >= Piece.BlackPawn && piece <= Piece.BlackKing;
    }

    private bool IsCurrentPlayersPiece(Piece piece)
    {
        if (WhiteTurn)
        {
            return IsWhitePiece(piece);
        }
        else
        {
            return IsBlackPiece(piece);
        }
    }

    private bool IsSameColor(Piece first, Piece second)
    {
        if (first == Piece.Empty || second == Piece.Empty)
        {
            return false;
        }

        return (IsWhitePiece(first) && IsWhitePiece(second)) ||
            (IsBlackPiece(first) && IsBlackPiece(second));
    }
    public bool MovePiece(int fromRow, int fromCol, int toRow, int toCol)
    {
        if (!IsInsideBoard(fromRow, fromCol) || !IsInsideBoard(toRow, toCol))
        {
            Console.WriteLine("Move is outside the board.");
            return false;
        }

        Piece movingPiece = Board[fromRow, fromCol];
        Piece targetPiece = Board[toRow, toCol];

        if (movingPiece == Piece.Empty)
        {
            Console.WriteLine("There is no piece on the starting square.");
            return false;
        }

        if (!IsCurrentPlayersPiece(movingPiece))
        {
            Console.WriteLine("That is not your piece.");
            return false;
        }

        if (IsSameColor(movingPiece, targetPiece))
        {
            Console.WriteLine("You cannot capture your own piece.");
            return false;
        }

        if (IsKingPiece(targetPiece))
        {
            Console.WriteLine("You cannot capture the king.");
            return false;
        }

        if (!IsLegalMove(fromRow, fromCol, toRow, toCol))
        {
            Console.WriteLine("That move is not legal.");
            return false;
        }

        bool movingWhite = IsWhitePiece(movingPiece);

        // Temporarily make the move
        Board[toRow, toCol] = movingPiece;
        Board[fromRow, fromCol] = Piece.Empty;

        // Check if this move leaves the moving player's own king in check
        if (IsKingInCheck(movingWhite))
        {
            // Undo the move
            Board[fromRow, fromCol] = movingPiece;
            Board[toRow, toCol] = targetPiece;

            Console.WriteLine("That move is illegal because it leaves your king in check.");
            return false;
        }
        PromotePawnIfNeeded(toRow, toCol);
        // Move is legal, so now change the turn
        WhiteTurn = !WhiteTurn;

        if (IsCheckmate(WhiteTurn))
        {
            Console.WriteLine(WhiteTurn ? "Checkmate! Black wins!" : "Checkmate! White wins!");
        }
        else if (IsKingInCheck(WhiteTurn))
        {
            Console.WriteLine(WhiteTurn ? "White king is in check!" : "Black king is in check!");
        }

        return true;

    }
    private bool IsLegalPawnMove(int fromRow, int fromCol, int toRow, int toCol)
    {
        Piece movingPiece = Board[fromRow, fromCol];
        Piece targetPiece = Board[toRow, toCol];

        int direction;
        int startingRow;

        if (movingPiece == Piece.WhitePawn)
        {
            direction = -1;
            startingRow = 6;
        }
        else if (movingPiece == Piece.BlackPawn)
        {
            direction = 1;
            startingRow = 1;
        }
        else
        {
            return false;
        }

        int rowDifference = toRow - fromRow;
        int colDifference = toCol - fromCol;

        // Move 1 square forward
        if (colDifference == 0 && rowDifference == direction && targetPiece == Piece.Empty)
        {
            return true;
        }

        // Move 2 squares forward from starting position
        if (colDifference == 0 &&
            fromRow == startingRow &&
            rowDifference == 2 * direction &&
            targetPiece == Piece.Empty &&
            Board[fromRow + direction, fromCol] == Piece.Empty)
        {
            return true;
        }

        // Capture diagonally
        if (Math.Abs(colDifference) == 1 &&
            rowDifference == direction &&
            targetPiece != Piece.Empty &&
            !IsSameColor(movingPiece, targetPiece))
        {
            return true;
        }

        return false;
    }

    private bool IsLegalMove(int fromRow, int fromCol, int toRow, int toCol)
    {
        Piece movingPiece = Board[fromRow, fromCol];

        switch (movingPiece)
        {
            case Piece.WhitePawn:
            case Piece.BlackPawn:
                return IsLegalPawnMove(fromRow, fromCol, toRow, toCol);

            case Piece.WhiteRook:
            case Piece.BlackRook:
                return IsLegalRookMove(fromRow, fromCol, toRow, toCol);

            case Piece.WhiteBishop:
            case Piece.BlackBishop:
                return IsLegalBishopMove(fromRow, fromCol, toRow, toCol);

            case Piece.WhiteQueen:
            case Piece.BlackQueen:
                return IsLegalQueenMove(fromRow, fromCol, toRow, toCol);

            case Piece.WhiteKnight:
            case Piece.BlackKnight:
                return IsLegalKnightMove(fromRow, fromCol, toRow, toCol);

            case Piece.WhiteKing:
            case Piece.BlackKing:
                return IsLegalKingMove(fromRow, fromCol, toRow, toCol);

            default:
                Console.WriteLine("Movement for this piece is not implemented yet.");
                return false;
        }
    }

    private bool IsLegalRookMove(int fromRow, int fromCol, int toRow, int toCol)
    {
        // Rook must move either in the same row or same column
        if (fromRow != toRow && fromCol != toCol)
        {
            return false;
        }

        int rowStep = Math.Sign(toRow - fromRow);
        int colStep = Math.Sign(toCol - fromCol);

        int currentRow = fromRow + rowStep;
        int currentCol = fromCol + colStep;

        // Check every square between start and destination
        while (currentRow != toRow || currentCol != toCol)
        {
            if (Board[currentRow, currentCol] != Piece.Empty)
            {
                return false;
            }

            currentRow += rowStep;
            currentCol += colStep;
        }

        return true;
    }

    private bool IsLegalBishopMove(int fromRow, int fromCol, int toRow, int toCol)
    {
        int rowDifference = toRow - fromRow;
        int colDifference = toCol - fromCol;

        // Bishop must move diagonally
        if (Math.Abs(rowDifference) != Math.Abs(colDifference))
        {
            return false;
        }

        int rowStep = Math.Sign(rowDifference);
        int colStep = Math.Sign(colDifference);

        int currentRow = fromRow + rowStep;
        int currentCol = fromCol + colStep;

        // Check every square between start and destination
        while (currentRow != toRow || currentCol != toCol)
        {
            if (Board[currentRow, currentCol] != Piece.Empty)
            {
                return false;
            }

            currentRow += rowStep;
            currentCol += colStep;
        }

        return true;
    }

    private bool IsLegalQueenMove(int fromRow, int fromCol, int toRow, int toCol)
    {
        return IsLegalRookMove(fromRow, fromCol, toRow, toCol) ||
            IsLegalBishopMove(fromRow, fromCol, toRow, toCol);
    }

    private bool IsLegalKnightMove(int fromRow, int fromCol, int toRow, int toCol)
    {
        int rowDifference = Math.Abs(toRow - fromRow);
        int colDifference = Math.Abs(toCol - fromCol);

        return (rowDifference == 2 && colDifference == 1) ||
               (rowDifference == 1 && colDifference == 2);
    }

    private bool IsLegalKingMove(int fromRow, int fromCol, int toRow, int toCol)
    {
        int rowDifference = Math.Abs(toRow - fromRow);
        int colDifference = Math.Abs(toCol - fromCol);

        return rowDifference <= 1 && colDifference <= 1;
    }

    private bool FindKing(bool whiteKing, out int kingRow, out int kingCol)
    {
        Piece kingPiece = whiteKing ? Piece.WhiteKing : Piece.BlackKing;

        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                if (Board[row, col] == kingPiece)
                {
                    kingRow = row;
                    kingCol = col;
                    return true;
                }
            }
        }

        kingRow = -1;
        kingCol = -1;
        return false;
    }

    private bool IsSquareAttacked(int targetRow, int targetCol, bool byWhite)
    {
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                Piece piece = Board[row, col];

                if (piece == Piece.Empty)
                {
                    continue;
                }

                if (byWhite && !IsWhitePiece(piece))
                {
                    continue;
                }

                if (!byWhite && !IsBlackPiece(piece))
                {
                    continue;
                }

                if (CanPieceAttackSquare(row, col, targetRow, targetCol))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool CanPieceAttackSquare(int fromRow, int fromCol, int targetRow, int targetCol)
    {
        Piece piece = Board[fromRow, fromCol];

        int rowDifference = targetRow - fromRow;
        int colDifference = targetCol - fromCol;

        switch (piece)
        {
            case Piece.WhitePawn:
                return rowDifference == -1 && Math.Abs(colDifference) == 1;

            case Piece.BlackPawn:
                return rowDifference == 1 && Math.Abs(colDifference) == 1;

            case Piece.WhiteRook:
            case Piece.BlackRook:
                return IsLegalRookMove(fromRow, fromCol, targetRow, targetCol);

            case Piece.WhiteBishop:
            case Piece.BlackBishop:
                return IsLegalBishopMove(fromRow, fromCol, targetRow, targetCol);

            case Piece.WhiteQueen:
            case Piece.BlackQueen:
                return IsLegalQueenMove(fromRow, fromCol, targetRow, targetCol);

            case Piece.WhiteKnight:
            case Piece.BlackKnight:
                return IsLegalKnightMove(fromRow, fromCol, targetRow, targetCol);

            case Piece.WhiteKing:
            case Piece.BlackKing:
                return IsLegalKingMove(fromRow, fromCol, targetRow, targetCol);

            default:
                return false;
        }
    }

    public bool IsKingInCheck(bool whiteKing)
    {
        if (!FindKing(whiteKing, out int kingRow, out int kingCol))
        {
            Console.WriteLine("King was not found.");
            return false;
        }

        bool attackedByWhite = !whiteKing;

        return IsSquareAttacked(kingRow, kingCol, attackedByWhite);
    }

    private bool IsKingPiece(Piece piece)
    {
        return piece == Piece.WhiteKing || piece == Piece.BlackKing;
    }

    public bool IsCheckmate(bool whitePlayer)
    {
        if (!IsKingInCheck(whitePlayer))
        {
            return false;
        }

        return !HasAnyLegalMove(whitePlayer);
    }

    private bool HasAnyLegalMove(bool whitePlayer)
    {
        for (int fromRow = 0; fromRow < 8; fromRow++)
        {
            for (int fromCol = 0; fromCol < 8; fromCol++)
            {
                Piece movingPiece = Board[fromRow, fromCol];

                if (movingPiece == Piece.Empty)
                {
                    continue;
                }

                if (whitePlayer && !IsWhitePiece(movingPiece))
                {
                    continue;
                }

                if (!whitePlayer && !IsBlackPiece(movingPiece))
                {
                    continue;
                }

                for (int toRow = 0; toRow < 8; toRow++)
                {
                    for (int toCol = 0; toCol < 8; toCol++)
                    {
                        if (CanMoveWithoutLeavingKingInCheck(fromRow, fromCol, toRow, toCol))
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    private bool CanMoveWithoutLeavingKingInCheck(int fromRow, int fromCol, int toRow, int toCol)
    {
        if (!IsInsideBoard(fromRow, fromCol) || !IsInsideBoard(toRow, toCol))
        {
            return false;
        }

        Piece movingPiece = Board[fromRow, fromCol];
        Piece targetPiece = Board[toRow, toCol];

        if (movingPiece == Piece.Empty)
        {
            return false;
        }

        if (IsSameColor(movingPiece, targetPiece))
        {
            return false;
        }

        if (IsKingPiece(targetPiece))
        {
            return false;
        }

        if (!IsLegalMove(fromRow, fromCol, toRow, toCol))
        {
            return false;
        }

        bool movingWhite = IsWhitePiece(movingPiece);

        // Temporarily make the move
        Board[toRow, toCol] = movingPiece;
        Board[fromRow, fromCol] = Piece.Empty;

        bool kingIsStillInCheck = IsKingInCheck(movingWhite);

        // Undo the move
        Board[fromRow, fromCol] = movingPiece;
        Board[toRow, toCol] = targetPiece;

        return !kingIsStillInCheck;
    }

    private void PromotePawnIfNeeded(int row, int col)
    {
        Piece piece = Board[row, col];

        if (piece == Piece.WhitePawn && row == 0)
        {
            Board[row, col] = Piece.WhiteQueen;
            Console.WriteLine("White pawn promoted to queen!");
        }
        else if (piece == Piece.BlackPawn && row == 7)
        {
            Board[row, col] = Piece.BlackQueen;
            Console.WriteLine("Black pawn promoted to queen!");
        }
    }

    public List<Move> GetAllLegalMoves(bool whitePlayer)
    {
        List<Move> legalMoves = new List<Move>();

        for (int fromRow = 0; fromRow < 8; fromRow++)
        {
            for (int fromCol = 0; fromCol < 8; fromCol++)
            {
                Piece movingPiece = Board[fromRow, fromCol];

                if (movingPiece == Piece.Empty)
                {
                    continue;
                }

                if (whitePlayer && !IsWhitePiece(movingPiece))
                {
                    continue;
                }

                if (!whitePlayer && !IsBlackPiece(movingPiece))
                {
                    continue;
                }

                for (int toRow = 0; toRow < 8; toRow++)
                {
                    for (int toCol = 0; toCol < 8; toCol++)
                    {
                        if (CanMoveWithoutLeavingKingInCheck(fromRow, fromCol, toRow, toCol))
                        {
                            legalMoves.Add(new Move(fromRow, fromCol, toRow, toCol));
                        }
                    }
                }
            }
        }

        return legalMoves;
    }

}