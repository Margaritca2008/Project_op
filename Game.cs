using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace PROJECT_OP
{
    class Game
    {
        private Board board;
        private Player currentPlayer;
        private int cursorX, cursorY;
        private (int x, int y)? selectedPiece = null;
        private List<(int x, int y)> possibleMove = new();
        public Game()
        {
            board = new Board();
            currentPlayer = Player.White;
            cursorX = 0;
            cursorY = 0;
        }
        public void Start()
        {
            while (true)
            {
                Console.Clear();
                if (IsGameOver())
                {
                    Console.WriteLine("\nНажмите любую клавишу для выхода");
                    Console.ReadKey(true);
                    return;
                }
                board.Draw();
                if (selectedPiece != null)
                {
                    ShowPossibleMoves();
                }
                Console.SetCursorPosition(cursorX * 2 + 2, cursorY + 1);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("▣");
                Console.SetCursorPosition(0, 10);
                ShowStatus();
                Console.ResetColor();
                ConsoleKeyInfo key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.LeftArrow: if (cursorX > 0) cursorX--; break;
                    case ConsoleKey.RightArrow: if (cursorX < 7) cursorX++; break;
                    case ConsoleKey.UpArrow: if (cursorY > 0) cursorY--; break;
                    case ConsoleKey.DownArrow: if (cursorY < 7) cursorY++; break;
                    case ConsoleKey.Spacebar: SelectPiece(); break;
                    case ConsoleKey.Enter: MovePiece(); break;
                    case ConsoleKey.Escape: Environment.Exit(0); return;
                }
            }
        }
        private void SelectPiece()
        {
            var piece = board.GetPiece(cursorX, cursorY);
            if (piece == PieceType.None)
            {
                selectedPiece = null;
                possibleMove.Clear();
                return;
            }
            bool isMyPiece =  (currentPlayer == Player.White && (piece == PieceType.White || piece == PieceType.WhiteKing)) || (currentPlayer == Player.Black && (piece == PieceType.Black || piece == PieceType.BlackKing));
            if (!isMyPiece)
            {
                selectedPiece = null;
                possibleMove.Clear();
                return;
            }
            selectedPiece = (cursorX, cursorY);
            possibleMove = GetMoves(selectedPiece.Value.x, selectedPiece.Value.y);
        }
        private void MovePiece() //сделать ход
        {
            if (selectedPiece == null) return;
            var from = selectedPiece.Value;
            foreach (var move in possibleMove)
            {
                if (move.x == cursorX && move.y == cursorY)
                {
                    var piece = board.GetPiece(from.x, from.y);
                    board.SetPiece(move.x, move.y, piece);
                    board.SetPiece(from.x, from.y, PieceType.None);
                    bool fl = false;
                    if (Math.Abs(move.x - from.x) > 1) //если мы съесли
                    {
                        fl = true;
                        int dx = Math.Sign(move.x - from.x);
                        int dy = Math.Sign(move.y - from.y);
                        int cx = from.x + dx;
                        int cy = from.y + dy;
                        while (cx != move.x && cy != move.y)
                        {
                            if (board.IsOpponent(piece, board.GetPiece(cx, cy)))
                            {
                                board.SetPiece(cx, cy, PieceType.None);
                            }
                            cx += dx;
                            cy += dy;
                        }
                    }
                    if (piece == PieceType.White && move.y == 0)
                        board.SetPiece(move.x, move.y, PieceType.WhiteKing);
                    if (piece == PieceType.Black && move.y == 7)
                        board.SetPiece(move.x, move.y, PieceType.BlackKing);
                    if (fl) // фиктивная смена игроков, если мы еще можем съесть этой же шашкой(мы уже съесли, но вдруг есть еще что-то), то мы просто не меняем игрока
                    {
                        var secondJump = GetJumps(move.x, move.y, board.GetPiece(move.x, move.y), new bool[8, 8]);
                        if (secondJump.Any()) //можем ли мы еще съесть
                        {
                            selectedPiece = (move.x, move.y);
                            possibleMove = secondJump;
                            return;
                        }
                    }
                    selectedPiece = null;
                    possibleMove.Clear();
                    SwitchPlayer();
                    return;
                }
            }
        }
        private bool IsKing(PieceType piece)
        {
            if (piece == PieceType.WhiteKing || piece == PieceType.BlackKing)
            {
                return true;
            }
            return false;
        }
        private List<(int x, int y)> GetMoves(int x, int y) // найти просто ходы, без покушать
        {
            var piece = board.GetPiece(x, y);
            var moves = new List<(int x, int y)>();
            int[] dxy = { -1, 1 };
            var jumps = GetJumps(x, y, piece, new bool[8, 8]);
            if (jumps.Any()) // если есть возможность есть, то ешь
            {
                return jumps;
            }
            if (!IsKing(piece))
            {
                int m = (piece == PieceType.White) ? -1 : 1; 
                foreach (int dx in dxy)
                {
                    int nx = x + dx;
                    int ny = y + m;
                    if (board.IsInside(nx, ny) && board.GetPiece(nx, ny) == PieceType.None)
                    {
                        moves.Add((nx, ny));
                    }
                }
            }
            else
            {
                foreach (int dx in dxy)
                {
                    foreach (int dy in dxy)
                    {
                        int nx = x + dx;
                        int ny = y + dy;
                        while (board.IsInside(nx, ny) && board.GetPiece(nx, ny) == PieceType.None)
                        {
                            moves.Add((nx, ny));
                            nx += dx;
                            ny += dy;
                        }
                    }
                }
            }
            return moves;
        }
        private List<(int x, int y)> GetJumps(int x, int y, PieceType piece, bool[,] vis)
        {
            var moves = new List<(int x, int y)>();
            vis[y, x] = true; //были ли мы уже тут (чтоб не было цикличности)
            int[] dxy = { -1, 1 };
            foreach (int dx in dxy)
            {
                foreach (int dy in dxy)
                {
                    int nx = x + dx;
                    int ny = y + dy;//координаты еды
                    while (board.IsInside(nx, ny) && board.GetPiece(nx, ny) == PieceType.None && IsKing(piece)) //для дамки 
                    {
                        nx += dx;
                        ny += dy;
                    }
                    if (!board.IsInside(nx, ny)) continue;
                    int jx = nx + dx;
                    int jy = ny + dy;// на эти координат мы пойдем, если съедим
                    var target = board.GetPiece(nx, ny); // это мы едим
                    if (!board.IsOpponent(piece, target)) continue;
                    while (board.IsInside(jx, jy) && board.GetPiece(jx, jy) == PieceType.None)
                    {
                        if (!vis[jy, jx])
                        {
                            moves.Add((jx, jy));
                            var vis2 = (bool[,])vis.Clone();
                            vis2[jy, jx] = true;
                            var nextMoves = GetJumps(jx, jy, piece, vis2);
                            moves.AddRange(nextMoves);
                        }
                        if (IsKing(piece))
                        {
                            jx += dx;
                            jy += dy;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            return moves.Distinct().ToList(); //возвращаем список без повторений
        }
        private void ShowPossibleMoves()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            foreach (var move in possibleMove)
            {
                Console.SetCursorPosition(move.x * 2 + 2, move.y + 1);
                Console.Write("X");
            }
            Console.ResetColor();
        }
        private void ShowStatus()
        {
            Console.ResetColor();
            Console.WriteLine($"Ходит игрок: {currentPlayer}");
        }
        private void SwitchPlayer()
        {
            currentPlayer = (currentPlayer == Player.White) ? Player.Black : Player.White;
        }
        private bool IsGameOver()
        {
            bool whiteExists = false;
            bool blackExists = false;
            bool whiteCanMove = false;
            bool blackCanMove = false;
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    var piece = board.GetPiece(x, y);
                    if (piece == PieceType.None) continue;
                    if (piece == PieceType.White || piece == PieceType.WhiteKing)
                    {
                        whiteExists = true;
                        if (GetMoves(x, y).Any())
                        {
                            whiteCanMove = true;
                        }
                    }
                    else if (piece == PieceType.Black || piece == PieceType.BlackKing)
                    {
                        blackExists = true;
                        if (GetMoves(x, y).Any())
                        {
                            blackCanMove = true;
                        }
                    }
                }
            }
            if (!whiteExists || !whiteCanMove)
            {
                Console.Clear();
                board.Draw();
                Console.WriteLine("\nПобедили ЧЁРНЫЕ! 🎉✨🥳");
                return true;
            }
            if (!blackExists || !blackCanMove)
            {
                Console.Clear();
                board.Draw();
                Console.WriteLine("\nПобедили БЕЛЫЕ! 🎉✨🥳");
                return true;
            }

            return false;
        }
    }
}