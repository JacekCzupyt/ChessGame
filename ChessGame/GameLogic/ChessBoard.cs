using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessGame.GameLogic.ChessPieces;

namespace ChessGame.GameLogic
{
    enum ChessColor
    {
        White,
        Black
    }

    struct ChessTile
    {
        public int x;
        public int y;

        public bool InBound()
        {
            return x >= 0 && y >= 0 && x <= 7 && y <= 7;
        }

        public override string ToString()
        {
            return $"{(char)('a' + x)}{y + 1}";
        }

        public ChessTile(int a, int b) { x = a; y = b; }
        public ChessTile((int, int) t) { x = t.Item1; y = t.Item2; }

        public static ChessTile operator+(ChessTile a, ChessTile b) => new ChessTile(a.x + b.x, a.y + b.y);
        public static ChessTile operator +(ChessTile a, (int, int) b) => new ChessTile(a.x + b.Item1, a.y + b.Item2);

        public static ChessTile operator -(ChessTile a, ChessTile b) => new ChessTile(a.x - b.x, a.y - b.y);
        public static ChessTile operator -(ChessTile a, (int, int) b) => new ChessTile(a.x - b.Item1, a.y - b.Item2);

        public static bool operator ==(ChessTile a, ChessTile b) => a.x == b.x && a.y == b.y;
        public static bool operator !=(ChessTile a, ChessTile b) => !(a == b);
    }

    class ChessBoard : System.ComponentModel.INotifyPropertyChanged
    {
        public ChessPiece[,] Board = new ChessPiece[8,8];

        public List<ChessPiece> PieceList = new List<ChessPiece>();

        public List<ChessMove> MoveHistory = new List<ChessMove>();

        public ChessColor CurrentColor = ChessColor.White;

        public ChessPiece this[(int, int) i]
        {
            get { return Board[i.Item1, i.Item2]; }
            set { Board[i.Item1, i.Item2] = value; }
        }

        public ChessPiece this[ChessTile i]
        {
            get { return Board[i.x, i.y]; }
            set { Board[i.x, i.y] = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static ChessBoard DefaultChessSetup()
        {
            ChessBoard b = new ChessBoard();

            b.PieceList.Add(new ChessPieces.Rook(b, ChessColor.White, new ChessTile(0, 0)));
            b.PieceList.Add(new ChessPieces.Rook(b, ChessColor.White, new ChessTile(7, 0)));

            b.PieceList.Add(new Knight(b, ChessColor.White, new ChessTile(1, 0)));
            b.PieceList.Add(new Knight(b, ChessColor.White, new ChessTile(6, 0)));

            b.PieceList.Add(new ChessPieces.Bishop(b, ChessColor.White, new ChessTile(2, 0)));
            b.PieceList.Add(new ChessPieces.Bishop(b, ChessColor.White, new ChessTile(5, 0)));

            b.PieceList.Add(new ChessPieces.Queen(b, ChessColor.White, new ChessTile(3, 0)));
            b.PieceList.Add(new ChessPieces.King(b, ChessColor.White, new ChessTile(4, 0)));



            b.PieceList.Add(new ChessPieces.Rook(b, ChessColor.Black, new ChessTile(0, 7)));
            b.PieceList.Add(new ChessPieces.Rook(b, ChessColor.Black, new ChessTile(7, 7)));

            b.PieceList.Add(new Knight(b, ChessColor.Black, new ChessTile(1, 7)));
            b.PieceList.Add(new Knight(b, ChessColor.Black, new ChessTile(6, 7)));

            b.PieceList.Add(new ChessPieces.Bishop(b, ChessColor.Black, new ChessTile(2, 7)));
            b.PieceList.Add(new ChessPieces.Bishop(b, ChessColor.Black, new ChessTile(5, 7)));

            b.PieceList.Add(new ChessPieces.Queen(b, ChessColor.Black, new ChessTile(3, 7)));
            b.PieceList.Add(new ChessPieces.King(b, ChessColor.Black, new ChessTile(4, 7)));


            for (int i = 0; i < 8; i++)
            {
                b.PieceList.Add(new Pawn(b, ChessColor.White, new ChessTile(i, 1)));
                b.PieceList.Add(new Pawn(b, ChessColor.Black, new ChessTile(i, 6)));
            }

            return b;
        }

        public void PropertyChange()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Board"));
        }

        public bool TileIsThreatened(ChessTile Position, ChessColor Color)
        {
            //Warning! 
            //This function is meant to be used for identifying checks, and does not detect "en passant"

            List<(int, int)> RookDir = new List<(int, int)>() { (1, 0), (0, 1), (-1, 0), (0, -1) };
            List<(int, int)> BishopDir = new List<(int, int)> { (1, 1), (-1, 1), (-1, -1), (1, -1) };
            List<(int, int)> KnightDir = new List <(int, int)> { (2, 1), (2, -1), (-2, 1), (-2, -1), (1, 2), (1, -2), (-1, 2), (-1, -2) };

            //check rook
            foreach ((int, int) dir in RookDir)
            {
                ChessTile pos = Position + dir;
                while (pos.InBound())
                {
                    if(this[pos] != null)
                    {
                        if (this[pos].Color != Color && (this[pos] is Rook || this[pos] is Queen))
                        {
                            return true;
                        }
                        break;
                    }
                    pos += dir;
                }
            }

            //check bishop
            foreach ((int, int) dir in BishopDir)
            {
                ChessTile pos = Position + dir;
                while (pos.InBound())
                {
                    if (this[pos] != null)
                    {
                        if (this[pos].Color != Color && (this[pos] is Bishop || this[pos] is Queen))
                        {
                            return true;
                        }
                        break;
                    }
                    pos += dir;
                }
            }

            //check knight
            foreach ((int, int) dir in KnightDir)
            {
                ChessTile pos = Position + dir;
                if (pos.InBound())
                {
                    if (this[pos] != null)
                    {
                        if (this[pos].Color != Color && this[pos] is Knight)
                        {
                            return true;
                        }
                    }
                }
            }

            //check king
            foreach ((int, int) dir in RookDir.Concat(BishopDir))
            {
                ChessTile pos = Position + dir;
                if (pos.InBound())
                {
                    if (this[pos] != null)
                    {
                        if (this[pos].Color != Color && this[pos] is King)
                        {
                            return true;
                        }
                    }
                }
            }

            //check pawn
            int PawnDir = Color == ChessColor.White ? 1 : -1;
            List<int> PawnDiag = new List<int>() { 1, -1 };
            foreach(var diag in PawnDiag)
            {
                ChessTile pos = Position + (diag, PawnDir);
                if (pos.InBound() && this[pos] != null && this[pos].Color != Color && this[pos] is Pawn)
                    return true;
            }

            return false;
        }

        public EventHandler<ChessMoveEventArgs> AtachPlayer(Action MoveRequest, ChessColor color)
        {
            MoveEvents[color] = MoveRequest;
            return new EventHandler<ChessMoveEventArgs>(ExecuteMove);
        }

        void ExecuteMove(object sender, ChessMoveEventArgs e)
        {
            e.Move.Execute();
            PropertyChange();
            MoveEvents[CurrentColor].Invoke();
        }

        private Dictionary<ChessColor, Action> MoveEvents = new Dictionary<ChessColor, Action>();

        public void Begin() { MoveEvents[CurrentColor].Invoke(); }

        public bool IsInCheck(ChessColor color)
        {
            return TileIsThreatened(PieceList.Find(p => p.InPlay && p is King && p.Color == color).Position, color);
        }
    }

    class ChessMoveEventArgs : EventArgs
    {
        public ChessMove Move { get; set; }
    }
}
