using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.GameLogic.ChessPieces
{
    class King : AbstractChessPiece
    {
        private List<(int, int)> Directions { get; } =
            new List<(int, int)> { (1, 0), (0, 1), (-1, 0), (0, -1), (1, 1), (-1, 1), (-1, -1), (1, -1) };

        public override IEnumerable<ChessMove> PossibleMoves { get
            {
                List<ChessMove> Moves = new List<ChessMove>();
                if(InPlay)
                {
                    foreach((int, int) dir in Directions)
                    {
                        var pos = Position + dir;
                        if(pos.InBound())
                        {
                            if(Board[pos] == null)
                            {
                                Moves.Add(new ChessMove(Board, this, Position, pos, GetState));
                            }
                            else
                            {
                                if(Board[pos].Color != Color && !(Board[pos] is King))
                                {
                                    Moves.Add(new ChessMove(Board, this, Position, pos, GetState, Board[pos], pos, null, Board[pos].GetState));
                                }
                            }
                        }
                    }
                    
                    //Castling
                    if(!HasMoved)
                    {
                        var r1 = Board[(0, Position.y)];
                        var r2 = Board[(7, Position.y)];
                        if (r1 is Rook && r1.Color == Color && !(r1 as Rook).HasMoved &&
                            Enumerable.Range(1, 3).ToList().TrueForAll(i => Board[(i, Position.y)] == null) &&
                            Enumerable.Range(Position.x-2, 3).ToList().TrueForAll(i => !Board.TileIsThreatened(new ChessTile(i, Position.y), Color)))
                        {
                            Moves.Add(new ChessMove(Board, this, Position, Position - (2, 0), GetState, r1, r1.Position, Position - (1, 0), r1.GetState));
                        }
                        if (r2 is Rook && r2.Color == Color && !(r2 as Rook).HasMoved &&
                            Enumerable.Range(Position.x + 1, 2).ToList().TrueForAll(i => Board[(i, Position.y)] == null) &&
                            Enumerable.Range(Position.x, 3).ToList().TrueForAll(i => !Board.TileIsThreatened(new ChessTile(i, Position.y), Color)))
                        {
                            Moves.Add(new ChessMove(Board, this, Position, Position + (2, 0), GetState, r2, r2.Position, Position + (1, 0), r2.GetState));
                        }
                    }
                    
                }
                return Moves;
            } }

        public bool HasMoved;

        public King(ChessBoard board, ChessColor color, ChessTile position, bool onBoard = true, bool hasMoved = false) : base(board, color, position, onBoard)
        {
            HasMoved = hasMoved;
        }

        public override SpecialState GetState { get { return HasMoved ? SpecialState.None : SpecialState.FirstMoveKing; } }

        public override float Value { get { return 0; } }
    }
}
