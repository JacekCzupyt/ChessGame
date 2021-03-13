using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.GameLogic.ChessPieces
{
    class Bishop : AbstractSimpleChessPiece
    {
        protected override List<(int, int)> Directions
        {
            get { return new List<(int, int)> { (1, 1), (-1, 1), (-1, -1), (1, -1) }; }
        }

        public override float Value { get { return 3; } }

        public Bishop(ChessBoard board, ChessColor color, ChessTile position, bool onBoard = true) : base(board, color, position, onBoard) { }
    }
}
