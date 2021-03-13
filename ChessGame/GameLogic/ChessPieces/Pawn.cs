using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.GameLogic.ChessPieces
{
    class Pawn : AbstractChessPiece
    {
        public bool HasMoved;
        public int? ChargeDate;
        (int, int) Direction;

        public override IEnumerable<ChessMove> PossibleMoves { get
            {
                List<ChessMove> Moves = new List<ChessMove>();
                if (InPlay)
                {
                    ChessTile Pos = Position + Direction;
                    if(Pos.InBound() && Board[Pos]==null)
                    {
                        //Move forward
                        if (Pos.y == (Color == ChessColor.White ? 7 : 0))
                        {
                            //Morph move
                            for(int i=5;i<9;i++)
                            {
                                Moves.Add(new ChessMove(Board, this, Position, Pos, (SpecialState)i));
                            }
                        }
                        else
                        {
                            //Simple forward move
                            Moves.Add(new ChessMove(Board, this, Position, Pos, HasMoved ? SpecialState.None : SpecialState.FirstMovePawn));

                            if(!HasMoved)
                            {
                                //Charge forward move
                                Pos += Direction;
                                if (!Pos.InBound() || Pos.y == (Color == ChessColor.White ? 7 : 0))
                                    throw new InvalidOperationException();

                                if (Board[Pos]==null)
                                {
                                    Moves.Add(new ChessMove(Board, this, Position, Pos, SpecialState.ChargePawn));
                                }
                            }
                        }
                    }
                    var Diag = new List<ChessTile>() { new ChessTile(1, 0), new ChessTile(-1, 0) };
                    foreach(var d in Diag)
                    {
                        //Diagonal moves
                        Pos = Position + Direction + d;
                        if(Pos.InBound() && Board[Pos]!=null && Board[Pos].Color!=this.Color)
                        {
                            if (Pos.y == (Color == ChessColor.White ? 7 : 0))
                            {
                                //Morph diagonal move
                                for (int i = 5; i < 9; i++)
                                {
                                    Moves.Add(new ChessMove(Board, this, Position, Pos, (SpecialState)i,
                                    Board[Pos], Pos, null, SpecialState.None));
                                }
                            }
                            else
                            {
                                //Simple diagonal move
                                Moves.Add(new ChessMove(Board, this, Position, Pos, HasMoved ? SpecialState.None : SpecialState.FirstMovePawn,
                                    Board[Pos], Pos, null, SpecialState.None));
                            }
                        }

                        //En passant
                        Pos -= Direction;
                        if(Pos.InBound() && Board[Pos] is Pawn && Board[Pos].Color != this.Color && (Board[Pos] as Pawn).ChargeDate == Board.MoveHistory.Count-1)
                        {
                            //En passant can't be a morph move
                            if (Board[Pos + Direction] != null)
                                throw new InvalidOperationException();
                            Moves.Add(new ChessMove(Board, this, Position, Pos + Direction, SpecialState.None, Board[Pos], Pos, null, SpecialState.None));
                        }
                    }


                }
                return Moves;
            } }

        public override float Value { get { return 1; } }

        public Pawn(ChessBoard board, ChessColor color, ChessTile position, bool onBoard = true, bool hasMoved = false, int? chargeDate = null) : base(board, color, position, onBoard)
        {
            HasMoved = hasMoved;
            ChargeDate = chargeDate;
            Direction = (0, Color == ChessColor.White ? 1 : -1);
        }
    }
}
