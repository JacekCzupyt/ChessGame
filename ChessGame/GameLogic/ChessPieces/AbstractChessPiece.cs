using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.GameLogic.ChessPieces
{
    abstract class AbstractChessPiece : ChessPiece
    {
        public ChessBoard Board { get; private set; }

        public ChessColor Color { get; private set; }

        private ChessTile pos = new ChessTile(0, 0);

        public ChessTile Position
        {
            get { return pos; }
            set
            {
                if (!value.InBound())
                    throw new ArgumentException("Both values should be between 0 and 7", "value");
                if (!InPlay)
                {
                    pos = value;
                }
                else
                {
                    if (Board[pos] != this)
                        throw new InvalidOperationException("Initial position stored in the chess piece does not match the state of the board!");
                    if (Board[value] != null)
                        throw new InvalidOperationException("That board position is already occupied!");
                    Board[pos] = null;
                    Board[value] = this;
                    pos = value;
                }
                
            }
        }

        public abstract IEnumerable<ChessMove> PossibleMoves { get; }

        public IEnumerable<ChessMove> LegalMoves
        {
            get
            {
                if(Board.CurrentColor == Color)
                    return PossibleMoves.Where(move => move.IsLegal);
                return Enumerable.Empty<ChessMove>();
            }
        }

        private bool inPlay = false;

        public bool InPlay
        {
            get { return inPlay; }
            set
            {
                if(inPlay == false && value == true)
                {
                    if(Board[Position] != null)
                        throw new InvalidOperationException("That board position is already occupied!");
                    Board[Position] = this;
                    inPlay = true;
                }
                if(inPlay == true && value == false)
                {
                    if(Board[Position] != this)
                        throw new InvalidOperationException("Initial position stored in the chess piece does not match the state of the board!");
                    Board[Position] = null;
                    inPlay = false;
                }
            }
        }

        public virtual SpecialState GetState { get; } = SpecialState.None;

        public abstract float Value { get; }

        public AbstractChessPiece(ChessBoard board, ChessColor color, ChessTile position, bool onBoard = true)
        {
            Board = board;
            Color = color;
            Position = position;
            InPlay = onBoard;
        }
    }
}
