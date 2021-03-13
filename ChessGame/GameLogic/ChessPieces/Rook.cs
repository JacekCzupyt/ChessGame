using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.GameLogic.ChessPieces
{

    class Rook : AbstractSimpleChessPiece
    {
        protected override List<(int, int)> Directions {
            get { return new List<(int, int)> { (1, 0), (0, 1), (-1, 0), (0, -1) }; } }

        public bool HasMoved;

        public Rook(ChessBoard board, ChessColor color, ChessTile position, bool onBoard = true, bool hasMoved = false) : base(board, color, position, onBoard)
        {
            HasMoved = hasMoved;
        }

        public override SpecialState GetState { get { return HasMoved ? SpecialState.None : SpecialState.FirstMoveRook; } }

        public override float Value { get { return 5; } }
    }
}
