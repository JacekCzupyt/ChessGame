using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.GameLogic.ChessPieces
{
    class Knight : AbstractChessPiece
    {
        public Knight(ChessBoard board, ChessColor color, ChessTile position, bool onBoard = true) : base(board, color, position, onBoard) { }


        private List<(int, int)> Directions { get; } =
            new List<(int, int)> { (2, 1), (2, -1), (-2, 1), (-2, -1), (1, 2), (1, -2), (-1, 2), (-1, -2) };

        public override IEnumerable<ChessMove> PossibleMoves { get
            {
                List<ChessMove> Moves = new List<ChessMove>();
                if(InPlay)
                {
                    foreach ((int, int) dir in Directions)
                    {
                        var pos = Position + dir;
                        if (pos.InBound())
                        {
                            if (Board[pos] == null)
                            {
                                Moves.Add(new ChessMove(Board, this, Position, pos, GetState));
                            }
                            else
                            {
                                if (Board[pos].Color != Color && !(Board[pos] is King))
                                {
                                    Moves.Add(new ChessMove(Board, this, Position, pos, GetState, Board[pos], pos, null, Board[pos].GetState));
                                }
                            }
                        }
                    }
                }
                return Moves;
            } }

        public override float Value { get { return 3; } }
    }
}
