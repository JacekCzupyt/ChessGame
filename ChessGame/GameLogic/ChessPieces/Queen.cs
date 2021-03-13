using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.GameLogic.ChessPieces
{
    class Queen : AbstractSimpleChessPiece
    {
        protected override List<(int, int)> Directions { get; } = 
            new List<(int, int)> { (1, 0), (0, 1), (-1, 0), (0, -1), (1, 1), (-1, 1), (-1, -1), (1, -1) };

        public override float Value { get { return 9; } }

        public Queen(ChessBoard board, ChessColor color, ChessTile position, bool onBoard = true) : base(board, color, position, onBoard) { }
    }
}
