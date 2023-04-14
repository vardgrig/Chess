using System;
using UnityEngine;

namespace Chess.Game
{
    public class AIPlayer : Player
    {
        StockfishEngine stockFish;
        AISettings aiSettings;
        Board board;
        Move move;
        bool moveFound = false;
        int eval;

        public int Eval 
        {
            get { return eval; }
        }
        public AIPlayer(Board _board, AISettings _aiSettings)
        {
            board = _board;
            aiSettings = _aiSettings;
            stockFish = new();
            stockFish.Start(aiSettings);
        }
        public override void NotifyTurnToMove()
        {
            string fen = FenUtility.CurrentFen(board);
            (string bestMove, int _eval, int mateIn) = stockFish.GetBestMove(fen);
            eval = _eval;
            if(mateIn == -1)
                OnEvaluationValueChange(eval);
            else
                OnForceMate(mateIn);

            GetMove(bestMove);
            moveFound = true;
        }

        public override void Update()
        {
            if (moveFound)
            {
                moveFound = false;
                ChoseMove(move);
                stockFish.Stop();
            }
        }
        void GetMove(string moveString)
        {
            int startFile = moveString[0] - 'a';
            int startRank = moveString[1] - '1';
            int targetFile = moveString[2] - 'a';
            int targetRank = moveString[3] - '1';

            int startSquare = startRank * 8 + startFile;
            int targetSquare = targetRank * 8 + targetFile;

            // Check if move is castling
            if ((Mathf.Abs(startSquare - targetSquare) == 2 || Mathf.Abs(startSquare - targetSquare) == 3) &&
                (board.GetPieceAt(startSquare) == (Piece.King | Piece.White) || board.GetPieceAt(startSquare) == (Piece.King | Piece.Black)))
            {
                move = new Move(startSquare, targetSquare, Move.Flag.Castling);
            }
            // Checking en passiant move
            else if (startFile == targetFile && Mathf.Abs(startRank - targetRank) == 2 && board.GetPieceAt(startSquare) == Piece.Pawn)
            {
                move = new Move(startSquare, targetSquare, Move.Flag.PawnTwoForward);
            }

            // Check if move is pawn promotion
            else if (moveString.Length == 5)
            {
                int pieceType = Move.Flag.PromoteToQueen; // Default to promoting to queen
                switch (moveString[4])
                {
                    case 'n':
                    case 'N':
                        pieceType = Move.Flag.PromoteToKnight;
                        break;
                    case 'r':
                    case 'R':
                        pieceType = Move.Flag.PromoteToRook;
                        break;
                    case 'b':
                    case 'B':
                        pieceType = Move.Flag.PromoteToBishop;
                        break;
                }
                move = new Move(startSquare, targetSquare, pieceType);
            }
            // Otherwise, assume a normal move
            else
            {
                move = new(startSquare, targetSquare, Move.Flag.None);
            }
        }
    }
}