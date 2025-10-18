namespace PROJECT_OP
{
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

        public bool IsInside(int x, int y)
        {
            return x >= 0 && x < 8 && y >= 0 && y < 8;
        }
        public bool IsOpponent(PieceType me, PieceType other)
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
                    var piece = Cells[y, x];
                    if (piece == PieceType.None)
                    {
                        Console.Write(". ");
                    }
                    else
                    {
                        switch (piece)
                        {
                            case PieceType.Black:
                                Console.ForegroundColor = ConsoleColor.Black;
                                Console.Write("● ");
                                break;
                            case PieceType.White:
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Write("○ ");
                                break;
                            case PieceType.BlackKing:
                                Console.ForegroundColor = ConsoleColor.Black;
                                Console.Write("♚ ");
                                break;
                            case PieceType.WhiteKing:
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Write("♔ ");
                                break;
                        }
                        Console.ResetColor();
                    }
                }
                Console.WriteLine();
            }
        }
        public void SetPiece(int x, int y, PieceType piece)
        {
            if (IsInside(x, y))
                Cells[y, x] = piece;
        }
        public PieceType GetPiece(int x, int y)
        {
            if (!IsInside(x, y))
                return PieceType.None;
            return Cells[y, x];
        }
    }
}