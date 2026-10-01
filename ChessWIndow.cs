using System;
using Gtk;

class ChessWindow : Window
{
    private GameState game;
    private Button[,] buttons;

    private int selectedRow = -1;
    private int selectedCol = -1;

    private Label statusLabel;

    public ChessWindow(GameState game) : base("Chess Game")
    {
        this.game = game;
        buttons = new Button[8, 8];

        SetDefaultSize(650, 700);
        WindowPosition = WindowPosition.Center;

        DeleteEvent += delegate
        {
            Application.Quit();
        };

        AddStyles();

        Box mainBox = new Box(Orientation.Vertical, 5);
        Add(mainBox);

        statusLabel = new Label("White's turn");
        mainBox.PackStart(statusLabel, false, false, 5);

        Grid boardGrid = new Grid();
        boardGrid.RowHomogeneous = true;
        boardGrid.ColumnHomogeneous = true;

        mainBox.PackStart(boardGrid, true, true, 5);

        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                Button button = new Button();

                button.WidthRequest = 70;
                button.HeightRequest = 70;
                button.Hexpand = true;
                button.Vexpand = true;

                int capturedRow = row;
                int capturedCol = col;

                button.Clicked += delegate
                {
                    OnSquareClicked(capturedRow, capturedCol);
                };

                buttons[row, col] = button;
                boardGrid.Attach(button, col, row, 1, 1);
            }
        }

        RefreshBoard();
    }

    private void AddStyles()
    {
        CssProvider provider = new CssProvider();

        provider.LoadFromData(@"
            button.light-square {
                background: #f0d9b5;
                font-size: 30px;
            }

            button.dark-square {
                background: #b58863;
                font-size: 30px;
            }

            button.selected-square {
                background: #f6f669;
                font-size: 30px;
            }
        ");

        StyleContext.AddProviderForScreen(
            Gdk.Screen.Default,
            provider,
            600
        );
    }

    private void OnSquareClicked(int row, int col)
    {
        if (game.IsCheckmate(game.WhiteTurn))
        {
            return;
        }

        Piece clickedPiece = game.Board[row, col];

        // If no piece is selected yet
        if (selectedRow == -1)
        {
            if (clickedPiece == Piece.Empty)
            {
                statusLabel.Text = "Select a piece first.";
                return;
            }

            if (!IsCurrentPlayersPiece(clickedPiece))
            {
                statusLabel.Text = game.WhiteTurn ? "It is White's turn." : "It is Black's turn.";
                return;
            }

            selectedRow = row;
            selectedCol = col;

            statusLabel.Text = "Piece selected. Choose destination.";
            RefreshBoard();
            return;
        }

        // If player clicks another own piece, change selection
        if (clickedPiece != Piece.Empty && IsCurrentPlayersPiece(clickedPiece))
        {
            selectedRow = row;
            selectedCol = col;

            statusLabel.Text = "Piece selected. Choose destination.";
            RefreshBoard();
            return;
        }

        bool moveWorked = game.MovePiece(selectedRow, selectedCol, row, col);

        selectedRow = -1;
        selectedCol = -1;

        RefreshBoard();

        if (!moveWorked)
        {
            statusLabel.Text = "Illegal move. Try again.";
            return;
        }

        UpdateStatus();

        // Computer plays black
        if (!game.WhiteTurn && !game.IsCheckmate(game.WhiteTurn))
        {
            statusLabel.Text = "Computer is thinking...";

            while (Application.EventsPending())
            {
                Application.RunIteration();
            }

            ComputerPlayer.MakeMove(game);

            RefreshBoard();
            UpdateStatus();
        }
    }

    private void RefreshBoard()
    {
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                Button button = buttons[row, col];

                button.Label = GetPieceSymbol(game.Board[row, col]);

                StyleContext style = button.StyleContext;

                style.RemoveClass("light-square");
                style.RemoveClass("dark-square");
                style.RemoveClass("selected-square");

                if ((row + col) % 2 == 0)
                {
                    style.AddClass("light-square");
                }
                else
                {
                    style.AddClass("dark-square");
                }

                if (row == selectedRow && col == selectedCol)
                {
                    style.AddClass("selected-square");
                }
            }
        }
    }

    private void UpdateStatus()
    {
        if (game.IsCheckmate(game.WhiteTurn))
        {
            statusLabel.Text = game.WhiteTurn
                ? "Checkmate! Black wins!"
                : "Checkmate! White wins!";
        }
        else if (game.IsKingInCheck(game.WhiteTurn))
        {
            statusLabel.Text = game.WhiteTurn
                ? "White king is in check!"
                : "Black king is in check!";
        }
        else
        {
            statusLabel.Text = game.WhiteTurn
                ? "White's turn"
                : "Black's turn";
        }
    }

    private bool IsCurrentPlayersPiece(Piece piece)
    {
        if (piece == Piece.Empty)
        {
            return false;
        }

        bool pieceIsWhite = piece >= Piece.WhitePawn && piece <= Piece.WhiteKing;

        return game.WhiteTurn == pieceIsWhite;
    }

    private string GetPieceSymbol(Piece piece)
    {
        switch (piece)
        {
            case Piece.WhitePawn: return "♙";
            case Piece.WhiteRook: return "♖";
            case Piece.WhiteKnight: return "♘";
            case Piece.WhiteBishop: return "♗";
            case Piece.WhiteQueen: return "♕";
            case Piece.WhiteKing: return "♔";

            case Piece.BlackPawn: return "♟";
            case Piece.BlackRook: return "♜";
            case Piece.BlackKnight: return "♞";
            case Piece.BlackBishop: return "♝";
            case Piece.BlackQueen: return "♛";
            case Piece.BlackKing: return "♚";

            default: return "";
        }
    }
}