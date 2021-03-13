using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.GameLogic.ChessPieces
{
    abstract class AbstractSimpleChessPiece : AbstractChessPiece
    {
        abstract protected List<(int, int)> Directions { get; }

        public override IEnumerable<ChessMove> PossibleMoves
        {
            get
            {
                List<ChessMove> Moves = new List<ChessMove>();
                if (InPlay)
                {
                    foreach ((int, int) dir in Directions)
                    {
                        ChessTile pos = Position + dir;
                        while (pos.InBound())
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
                                break;
                            }
                            pos += dir;
                        }
                    }
                }
                return Moves;
            }
        }

        public AbstractSimpleChessPiece(ChessBoard board, ChessColor color, ChessTile position, bool onBoard = true):base(board, color, position, onBoard) { }
    }
}
