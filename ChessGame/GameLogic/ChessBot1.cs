using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.GameLogic
{
    class ChessBot1
    {
        public ChessBoard Board;

        const int MaxDepth = 3;
        const int MaxQuiescenceDepth = 6;

        private float RatePosition()
        {
            float ans = 0;
            foreach(var piece in Board.PieceList)
            {
                if(piece.InPlay)
                {
                    ans += (piece.Color == ChessColor.White ? 1 : -1) * piece.Value;
                }
            }
            return ans;
        }

        private (float, ChessMove) Calculate(int depth, float? previousBest = null)
        {
            if(depth >= MaxDepth)
            {
                //return (RatePosition(), null);
                return QuiescenceCalculate(depth, previousBest);
            }
            int mult = Board.CurrentColor == ChessColor.White ? 1 : -1;
            ChessMove bestMove = null;
            float bestScore = -10000 * mult;

            foreach(var piece in Board.PieceList.FindAll(p => p.InPlay))
            {
                foreach(var move in piece.LegalMoves)
                {
                    move.Execute();

                    var res = Calculate(depth + 1, bestScore);

                    if(res.Item1 * mult > bestScore * mult)
                    {
                        bestScore = res.Item1;
                        bestMove = move;
                    }

                    move.Revert();

                    if (previousBest.HasValue && bestScore * mult >= previousBest.Value * mult)
                    {
                        return (bestScore, bestMove);
                    }
                }
            }

            if(bestMove == null)
            {
                if (Board.TileIsThreatened(
                    Board.PieceList.Find(
                        p => p.Color == Board.CurrentColor && p is ChessPieces.King).Position,
                    Board.CurrentColor))//if king in check
                {
                    return (-1000 * mult, null);
                }
                else
                    return (0, null);
            }

            return (bestScore, bestMove);
        }

        private (float, ChessMove) QuiescenceCalculate (int depth, float? previousBest = null)
        {
            if (depth >= MaxQuiescenceDepth)
            {
                return (RatePosition(), null);
            }
            int mult = Board.CurrentColor == ChessColor.White ? 1 : -1;
            ChessMove bestMove = null;
            float bestScore = RatePosition();

            foreach (var piece in Board.PieceList.FindAll(p => p.InPlay))
            {
                foreach (var move in 
                    Board.IsInCheck(Board.CurrentColor) ?
                    piece.LegalMoves :
                    piece.LegalMoves.Where(m => m.IsCapeture || (int)m.State[0] >= 5 || m.IsCheck))
                {
                    move.Execute();

                    var res = QuiescenceCalculate(depth + 1, bestScore);

                    if (res.Item1 * mult > bestScore * mult)
                    {
                        bestScore = res.Item1;
                        bestMove = move;
                    }

                    move.Revert();

                    if (previousBest.HasValue && bestScore * mult >= previousBest.Value * mult)
                    {
                        return (bestScore, bestMove);
                    }
                }
            }

            if (bestMove == null)
            {
                if (Board.IsInCheck(Board.CurrentColor))//if king in check
                {
                    return (-1000 * mult, null);
                }
                else
                    return (RatePosition(), null);
            }

            return (bestScore, bestMove);
        }

        public event EventHandler<ChessMoveEventArgs> CommitMove;

        public void MoveRequest()
        {
            CommitMove(this, new ChessMoveEventArgs() { Move = Calculate(0).Item2 });
        }
    }
}
