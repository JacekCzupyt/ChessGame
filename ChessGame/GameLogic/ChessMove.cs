using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessGame.GameLogic.ChessPieces;

namespace ChessGame.GameLogic
{
    enum SpecialState
    {
        None = 0,
        FirstMoveKing = 1,
        FirstMoveRook = 2,
        FirstMovePawn = 3,
        ChargePawn = 4, //Pawn that has just made a double move forward, enabeling "En passant"
        PawnMorphQueen = 5,
        PawnMorphRook = 6,
        PawnMorphBishop = 7,
        PawnMorphKnight = 8 
    }

    class ChessMove
    {
        ChessBoard Board;
        int MoveDate;

        ChessPiece[] Piece = new ChessPiece[2];
        ChessTile?[] Origin = new ChessTile?[2];
        ChessTile?[] Destination = new ChessTile?[2];
        public SpecialState[] State = new SpecialState[2];

        ChessPiece MorphedPiece = null;

        public ChessMove(
            ChessBoard board, 
            ChessPiece movingPiece, 
            ChessTile? origin, 
            ChessTile? destination,
            SpecialState state = SpecialState.None,
            ChessPiece secondaryPiece = null, 
            ChessTile? secondaryOrigin = null, 
            ChessTile? secondaryDestination = null,
            SpecialState secondaryState = SpecialState.None)
        {
            Board = board;
            MoveDate = Board.MoveHistory.Count;
            Piece[0] = movingPiece;
            Origin[0] = origin;
            Destination[0] = destination;
            State[0] = state;
            Piece[1] = secondaryPiece;
            Origin[1] = secondaryOrigin;
            Destination[1] = secondaryDestination;
            State[1] = secondaryState;
        }

        public bool IsCapeture { get { return Piece[1] != null && Piece[1].Color != Piece[0].Color; } }

        public void Execute()
        {
            //Test validity
            if (Board == null)
                throw new ArgumentNullException("Board", "Board can not be null");
            if (Piece[0] == null)
                throw new ArgumentNullException("Piece", "There must be a moving piece");
            if (Piece[0].Board != Board || (Piece[1] != null && Piece[1].Board != Board))
                throw new ArgumentException("Piece", "Piece belongs to a different board");
            //add more exceptions?

            if (Piece[1] != null)
                SubExecute(1);

            SubExecute(0);

            Board.MoveHistory.Add(this);

            Board.CurrentColor = Board.CurrentColor == ChessColor.White ? ChessColor.Black : ChessColor.White;


            //Board.PropertyChange();
        }

        private void SubExecute(int i)
        {
            //validate state
            if (Piece[i] == null)
                throw new ArgumentNullException();
            if ((Origin[i] == null) == Piece[i].InPlay)
                throw new InvalidOperationException();
            if (Origin[i] != null && Origin[i] != Piece[i].Position)
                throw new InvalidOperationException();

            //main move
            if (Destination[i] != null)
            {
                Piece[i].Position = Destination[i].GetValueOrDefault();
                Piece[i].InPlay = true;
            }
            else
                Piece[i].InPlay = false;

            //update states
            switch(State[i])
            {
                case SpecialState.FirstMoveRook:
                    {
                        if (Piece[i] is ChessPieces.Rook)
                            (Piece[i] as ChessPieces.Rook).HasMoved = true;
                        else
                            throw new InvalidOperationException();
                        break;
                    }
                case SpecialState.FirstMoveKing:
                    {
                        if (Piece[i] is ChessPieces.King)
                            (Piece[i] as ChessPieces.King).HasMoved = true;
                        else
                            throw new InvalidOperationException();
                        break;
                    }
                case SpecialState.FirstMovePawn:
                    {
                        if (Piece[i] is ChessPieces.Pawn)
                            (Piece[i] as ChessPieces.Pawn).HasMoved = true;
                        else
                            throw new InvalidOperationException();
                        break;
                    }
                case SpecialState.ChargePawn:
                    {
                        if (Piece[i] is ChessPieces.Pawn)
                        {
                            (Piece[i] as ChessPieces.Pawn).HasMoved = true;
                            (Piece[i] as ChessPieces.Pawn).ChargeDate = MoveDate;
                        }
                            
                        else
                            throw new InvalidOperationException();
                        break;
                    }
                case SpecialState.PawnMorphRook:
                    {
                        if (Piece[i] is ChessPieces.Pawn)
                        {
                            var pawn = (Piece[i] as Pawn);
                            pawn.InPlay = false;
                            MorphedPiece = new Rook(Board, pawn.Color, pawn.Position, true, true);
                            Board.PieceList.Add(MorphedPiece);
                        }
                        else
                            throw new InvalidOperationException();
                        break;
                    }
                case SpecialState.PawnMorphBishop:
                    {
                        if (Piece[i] is ChessPieces.Pawn)
                        {
                            var pawn = (Piece[i] as Pawn);
                            pawn.InPlay = false;
                            MorphedPiece = new Bishop(Board, pawn.Color, pawn.Position);
                            Board.PieceList.Add(MorphedPiece);
                        }
                        else
                            throw new InvalidOperationException();
                        break;
                    }
                case SpecialState.PawnMorphKnight:
                    {
                        if (Piece[i] is ChessPieces.Pawn)
                        {
                            var pawn = (Piece[i] as Pawn);
                            pawn.InPlay = false;
                            MorphedPiece = new Knight(Board, pawn.Color, pawn.Position);
                            Board.PieceList.Add(MorphedPiece);
                        }
                        else
                            throw new InvalidOperationException();
                        break;
                    }
                case SpecialState.PawnMorphQueen:
                    {
                        if (Piece[i] is ChessPieces.Pawn)
                        {
                            var pawn = (Piece[i] as Pawn);
                            pawn.InPlay = false;
                            MorphedPiece = new Queen(Board, pawn.Color, pawn.Position);
                            Board.PieceList.Add(MorphedPiece);
                        }
                        else
                            throw new InvalidOperationException();
                        break;
                    }
            }
        }

        public void Revert()
        {
            //Test validity
            if (Board == null)
                throw new ArgumentNullException("Board", "Board can not be null");
            if (Piece[0] == null)
                throw new ArgumentNullException("Piece", "There must be a moving piece");
            if (Piece[0].Board != Board || (Piece[1] != null && Piece[1].Board != Board))
                throw new ArgumentException("Piece", "Piece belongs to a different board");
            //add more exceptions?

            SubRevert(0);

            if (Piece[1] != null)
                SubRevert(1);

            Board.MoveHistory.RemoveAt(Board.MoveHistory.Count-1);

            Board.CurrentColor = Board.CurrentColor == ChessColor.White ? ChessColor.Black : ChessColor.White;

            //Board.PropertyChange();
        }

        private void SubRevert(int i)
        {
            

            //update states
            switch (State[i])
            {
                case SpecialState.FirstMoveRook:
                    {
                        if (Piece[i] is ChessPieces.Rook)
                            (Piece[i] as ChessPieces.Rook).HasMoved = false;
                        else
                            throw new InvalidOperationException();
                        break;
                    }
                case SpecialState.FirstMoveKing:
                    {
                        if (Piece[i] is ChessPieces.King)
                            (Piece[i] as ChessPieces.King).HasMoved = false;
                        else
                            throw new InvalidOperationException();
                        break;
                    }
                case SpecialState.FirstMovePawn:
                    {
                        if (Piece[i] is ChessPieces.Pawn)
                            (Piece[i] as ChessPieces.Pawn).HasMoved = false;
                        else
                            throw new InvalidOperationException();
                        break;
                    }
                case SpecialState.ChargePawn:
                    {
                        if (Piece[i] is ChessPieces.Pawn)
                        {
                            (Piece[i] as ChessPieces.Pawn).HasMoved = false;
                            (Piece[i] as ChessPieces.Pawn).ChargeDate = null;
                        }

                        else
                            throw new InvalidOperationException();
                        break;
                    }
                case SpecialState.PawnMorphRook:
                    {
                        if (Piece[i] is ChessPieces.Pawn && Board[Destination[i].Value] is ChessPieces.Rook)
                        {
                            var pawn = (Piece[i] as Pawn);
                            var rook = Board[Destination[i].Value];
                            rook.InPlay = false;
                            Board.PieceList.Remove(rook);
                            pawn.InPlay = true;
                        }
                        else
                            throw new InvalidOperationException();
                        break;
                    }
                case SpecialState.PawnMorphBishop:
                    {
                        if (Piece[i] is ChessPieces.Pawn && Board[Destination[i].Value] is ChessPieces.Bishop)
                        {
                            var pawn = (Piece[i] as Pawn);
                            var bishop = Board[Destination[i].Value];
                            bishop.InPlay = false;
                            Board.PieceList.Remove(bishop);
                            pawn.InPlay = true;
                        }
                        else
                            throw new InvalidOperationException();
                        break;
                    }
                case SpecialState.PawnMorphKnight:
                    {
                        if (Piece[i] is ChessPieces.Pawn && Board[Destination[i].Value] is ChessPieces.Knight)
                        {
                            var pawn = (Piece[i] as Pawn);
                            var knight = Board[Destination[i].Value];
                            knight.InPlay = false;
                            Board.PieceList.Remove(knight);
                            pawn.InPlay = true;
                        }
                        else
                            throw new InvalidOperationException();
                        break;
                    }
                case SpecialState.PawnMorphQueen:
                    {
                        if (Piece[i] is ChessPieces.Pawn && Board[Destination[i].Value] is ChessPieces.Queen)
                        {
                            var pawn = (Piece[i] as Pawn);
                            var queen = Board[Destination[i].Value];
                            queen.InPlay = false;
                            Board.PieceList.Remove(queen);
                            pawn.InPlay = true;
                        }
                        else
                            throw new InvalidOperationException();
                        break;
                    }
            }

            //validate state
            if (Piece[i] == null)
                throw new ArgumentNullException();
            if ((Destination[i] == null) == Piece[i].InPlay)
                throw new InvalidOperationException();
            if (Destination[i] != null && Destination[i] != Piece[i].Position)
                throw new InvalidOperationException();

            //main move
            if (Origin[i] != null)
            {
                Piece[i].Position = Origin[i].GetValueOrDefault();
                Piece[i].InPlay = true;
            }
            else
                Piece[i].InPlay = false;

            
        }

        public (int, int) Dest
        {
            get
            {
                if (Destination[0] != null)
                    return (Destination[0].GetValueOrDefault().x, Destination[0].GetValueOrDefault().y);
                else if (Destination[1] != null)
                    return (Destination[1].GetValueOrDefault().x, Destination[1].GetValueOrDefault().y);
                else
                    throw new InvalidOperationException();
            }
        }

        //public bool IsLegal()
        //{
        //    //Includes support for multiple kings... not sure why, but it does.

        //    Execute();

        //    bool result = true;

        //    var kings = Board.PieceList.FindAll(val => (val.Color == Piece[0].Color && val is King && val.InPlay));
        //    foreach(var king in kings)
        //    {
        //        if (Board.TileIsThreatened(king.Position, king.Color))
        //        {
        //            result = false;
        //            break;
        //        }
        //    }

        //    Revert();

        //    return result;
        //}

        private bool? isCheck = null, isLegal = null;

        public bool IsCheck
        {
            get
            {
                if (!isCheck.HasValue)
                    testMove();
                return isCheck.Value;
            }
        }

        public  bool IsLegal
        {
            get
            {
                if (!isLegal.HasValue)
                    testMove();
                return isLegal.Value;
            }
        }

        private void testMove()
        {
            Execute();

            isLegal = !Board.IsInCheck(Piece[0].Color);

            isCheck = Board.IsInCheck(Piece[0].Color == ChessColor.White ? ChessColor.Black : ChessColor.White);

            Revert();
        }
    }
}
