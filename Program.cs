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
    enum MoveResult
    {
        Invalid,
        Moved,                  
        CapturedAndFinished,   
        CapturedAndContinue 
    }
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
        private bool IsInside(int x, int y) => x >= 0 && x < 8 && y >= 0 && y < 8;
        private bool IsOpponent(PieceType me, PieceType other)
        {
            if (other == PieceType.None) return false;
            if (me == PieceType.White || me == PieceType.WhiteKing)
                return other == PieceType.Black || other == PieceType.BlackKing;
            if (me == PieceType.Black || me == PieceType.BlackKing)
                return other == PieceType.White || other == PieceType.WhiteKing;
            return false;
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

        public bool AnyCaptureAvailable(Player player)
        {
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    var piece = Cells[y, x];
                    if (piece == PieceType.None) continue;
                    if (player == Player.White && (piece == PieceType.White || piece == PieceType.WhiteKing))
                    {
                        if (CanCaptureFrom(x, y))
                        {
                            return true;
                        }
                    }
                    if (player == Player.Black && (piece == PieceType.Black || piece == PieceType.BlackKing))
                    {
                        if (CanCaptureFrom(x, y))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool CanCaptureFrom(int x, int y)
        {
            var piece = Cells[y, x];
            if (piece == PieceType.None) return false;
            if (piece == PieceType.White || piece == PieceType.Black)
            {
                int[] dx = { 2, -2, 2, -2 };
                int[] dy = { -2, 2, 2, -2 };
                for (int i = 0; i < 4; i++)
                {
                    int fx = x + dx[i], fy = y + dy[i], ex = x + dx[i] / 2, ey = y + dy[i] / 2;
                    if (!IsInside(fx, fy) || !IsInside(ex, ey)) continue;
                    if (Cells[fy, fx] != PieceType.None) continue;
                    var mid = Cells[ey, ex];
                    if (IsOpponent(piece, mid)) return true;
                }
                return false;
            }
            int[] vx = { 1, -1, 1, -1 };
            int[] vy = { -1, 1, 1, -1 };
            for (int i = 0; i < 4; i++)
            {
                int cx = x + vx[i], cy = y + vy[i];
                while (IsInside(cx, cy) && Cells[cy, cx] == PieceType.None)
                {
                    cx += vx[i];
                    cy += vy[i];
                }
                if (!IsInside(cx, cy)) continue;
                if (IsOpponent(piece, Cells[cy, cx]))
                {
                    int tx = cx + vx[i], ty = cy + vx[i];
                    
                    while (IsInside(tx, ty))
                    {
                        if (Cells[ty, tx] == PieceType.None) return true;
                        if (Cells[cy + vy[i], cx + vx[i]] != PieceType.None) return false;
                        tx += vx[i];
                        ty += vy[i];
                    }
                }
            }
            return false;
        }
    }

    class Game
    {
        private Board board;
        private Player currentPlayer;
        private int cursorX = 0;
        private int cursorY = 0;
        private (int X, int Y)? selectedPiece = null;
        private bool inCaptureSequence = false;
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
                    case ConsoleKey.Enter: HandleEnter(); break;
                }
            }
        }
        private void HandleEnter()
        {
            if (selectedPiece == null)
            {
                var piece = board.Cells[cursorY, cursorX];
                if (piece == PieceType.None)
                {
                    Console.WriteLine("В клетке нет шашки");
                    return;
                }
                if (currentPlayer == Player.White && (piece == PieceType.White || piece == PieceType.WhiteKing)
                     || currentPlayer == Player.Black && (piece == PieceType.Black || piece == PieceType.BlackKing))
                {
                    selectedPiece = (cursorX, cursorY);
                }
                else
                {
                    Console.WriteLine("Это не Ваша шашка");
                }
            }
            else
            {
                var from = selectedPiece.Value;
                bool captureExists = board.AnyCaptureAvailable(currentPlayer);
                string err;
                MoveResult res = board.Move(from.X, from.Y, cursorX, cursorY, out err);
                if (res == MoveResult.Invalid)
                {
                    Console.WriteLine("Ошибка: " + err);
                    return;
                }

                if (res == MoveResult.Moved)
                {
                    if (captureExists)
                    {
                        Console.WriteLine("У Вас обязательное съедание шашки, ходить нельзя");
                        return;
                    }
                    selectedPiece = null;
                    currentPlayer = (currentPlayer == Player.Black) ? Player.White : Player.Black;
                    inCaptureSequence = false;
                    return;
                }

                if (res == MoveResult.CapturedAndFinished)
                {
                    selectedPiece = null;
                    inCaptureSequence = false;
                    currentPlayer = currentPlayer == Player.White ? Player.Black : Player.White;
                    return;
                }

                if (res == MoveResult.CapturedAndContinue)
                {
                    selectedPiece = (cursorX, cursorY);
                    inCaptureSequence = true;
                    return;
                }
                selectedPiece = null;
            }
        }
        private bool Move(int fromX, int fromY, int toX, int toY, )
        {
            
            if (fromX < 0 || fromX > 7 || fromY < 0 || fromY > 7 || toX < 0 || toX > 7 || toY < 0 || toY > 7)
            {
                Console.WriteLine("Пожалуйста, оставайтесь в рамках доски");
                return false;
            }
            var piece = board.Cells[fromX, fromY];
            if (piece == PieceType.None) return false;
            if (piece == PieceType.White && currentPlayer != Player.White && piece != PieceType.WhiteKing)
            {
                Console.WriteLine("Пожалуйста, ходите только своими шашками");
                return false;
            }
            if (piece == PieceType.Black && currentPlayer != Player.Black && piece != PieceType.BlackKing)
            {
                Console.WriteLine("Пожалуйста, ходите только своими шашками");
                return false;
            }
            int absDx = Math.Abs(toX - fromX);
            int absDy = Math.Abs(toY - fromY);
            if (absDx != absDy)
            {
                Console.WriteLine("Пожалуйста, ходите только по диагонали");
                return false;
            }
        }
    }
        static void Main()
    {
        var board = new Board();
        board.Draw();
        Console.WriteLine("Hello!");
    }
}