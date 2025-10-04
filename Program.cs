using System;
using System.Threading;
using Program;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

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
                        else Cells[y, x] = PieceType.Black;
                    }
                    else
                    {
                        Cells[y, x] = PieceType.White;
                    }
                }
            }
        }
    }
    static void Main()
    {
        Console.WriteLine("Hello!");
    }
}