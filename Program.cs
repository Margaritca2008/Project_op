using System;
using System.Threading;
using Program;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Security;
namespace Program;

class Program
{
    enum Player
    {
        Black,
        White
    }
    enum PieceType
    {
        None,
        White,
        Black,
        WhiteKing,
        BlackKing
    }
    class Board
    {
        public PieceType[,] Cells { get; private set; }

        public Board()
        {
            Cells = new PieceType[8, 8];
            InitializeBoard();
        }
        private void InitializeBoard()
        {
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if ((x + y) % 2 == 1)
                    {
                        if (y < 3) Cells[y, x] = PieceType.Black;
                        else if (y > 4) Cells[y, x] = PieceType.White;
                        else Cells[y, x] = PieceType.None;
                    }
                    else
                    {
                        Cells[y, x] = PieceType.None;
                    }
                }
            }
        }
        public void Draw()
        {
            Console.Clear();
            Console.WriteLine("  1 2 3 4 5 6 7 8");
            for (int y = 0; y < 8; y++)
            {
                Console.Write((y + 1) + " ");
                for (int x = 0; x < 8; x++)
                {
                    bool isDarkCell = (x + y) % 2 == 1;
                    Console.BackgroundColor = isDarkCell ? ConsoleColor.DarkMagenta : ConsoleColor.White;
                    var piece = Cells[y, x];
                    if (piece == PieceType.None)
                    {
                        Console.Write("  ");
                    }
                    else
                    {
                        switch (piece)
                        {
                            case PieceType.Black:
                                Console.ForegroundColor = ConsoleColor.DarkBlue;
                                Console.Write("● ");
                                break;
                            case PieceType.White:
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Write("● ");
                                break;
                            case PieceType.BlackKing:
                                Console.ForegroundColor = ConsoleColor.DarkBlue;
                                Console.Write("♚ ");
                                break;
                            case PieceType.WhiteKing:
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Write("♔ ");
                                break;
                        }
                    }
                    Console.ResetColor();

                }
                Console.WriteLine();
            }
        }
    }

    class Game
    {
        private Board board;
        private Player currentPlayer;
        private int cursorX = 0;
        private int cursorY = 0;
        private (int X, int Y)? selectedPiece = null;
        public Game()
        {
            board = new Board();
            currentPlayer = Player.White;
        }

        public void Start()
        {
            while (true)
            {
                Console.Clear();
                board.Draw();
                ConsoleKeyInfo key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.LeftArrow: if (cursorX > 0) cursorX--; break;
                    case ConsoleKey.RightArrow: if (cursorX < 7) cursorX++; break;
                    case ConsoleKey.UpArrow: if (cursorY > 0) cursorY--; break;
                    case ConsoleKey.DownArrow: if (cursorY < 7) cursorY++; break;
                    case ConsoleKey.Enter: SelectingChecker(); break;
                }
            }
        }
        private void SelectingChecker()
        {
            if (selectedPiece == null)
            {
                var piece = board.Cells[cursorY, cursorX];
                if (currentPlayer == Player.White && (piece == PieceType.White || piece == PieceType.WhiteKing)
                 || currentPlayer == Player.Black && (piece == PieceType.Black || piece == PieceType.BlackKing))
                {
                    selectedPiece = (cursorX, cursorY);
                }
            }
            else
            {
                var from = selectedPiece.Value;
                if (board.Move(from.X, from.Y, cursorX, cursorY))
                {
                    if (currentPlayer == Player.Black)
                    {
                        currentPlayer = Player.White;
                    }
                    else
                    {
                        currentPlayer = Player.Black;
                    }
                }
                selectedPiece = null;
            }
        }
        private bool Move(int fromX, int fromY, int toX, int toY)
        {
            if (fromX < 0 || fromX > 7 || fromY < 0 || fromY > 7 || toX < 0 || toX > 7 || toY < 0 || toY > 7)
            {
                return false;
            }
            var piece = board.Cells[fromX, fromY];
            if (piece == PieceType.None) return false;

        }
    }
        static void Main()
    {
        var board = new Board();
        board.Draw();
        Console.WriteLine("Hello!");
    }
}