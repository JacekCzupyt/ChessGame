using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.GameLogic
{
    interface ChessPiece
    {
        ChessBoard Board { get; }
        ChessColor Color { get; }
        ChessTile Position { get; set; }
        IEnumerable<ChessMove> PossibleMoves { get; }
        IEnumerable<ChessMove> LegalMoves { get; }
        bool InPlay { get; set; }
        SpecialState GetState { get; }
        float Value { get; }
    }
}
