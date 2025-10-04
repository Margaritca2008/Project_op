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
namespace Program;

class Program
{
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
        static void Main()
        {
            var board = new Board();
            board.Draw();
            Console.WriteLine("Hello!");
        }
}