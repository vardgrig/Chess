using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Chess.Game
{
    public class GameManager : MonoBehaviour
    {
        public enum Result { Playing, WhiteIsMated, BlackIsMated, Stalemate, Repetition, FiftyMoveRule, InsufficientMaterial, WhiteLosesInTime, BlackLosesInTime }

        public event Action onPositionLoaded;
        public event Action<Move> onMoveMade;
        public event Action<Result> OnGameEnds;

        public enum PlayerType { Human, AI }

        private PlayerType whitePlayerType;
        private PlayerType blackPlayerType;
        private bool whiteIsBottom;

        [SerializeField] GameSettings gameSettings;
        [SerializeField] bool useClocks;
        [SerializeField] Clock whiteClock;
        [SerializeField] Clock blackClock;
        [SerializeField] TMPro.TMP_Text evalValueText;
        [SerializeField] TMPro.TMP_Text forceMateText;
        [SerializeField] Slider evalSlider;
        [SerializeField] TMPro.TMP_Text resultUI;
        [SerializeField] int evaluationValue = 0;
        [SerializeField] AISettings aiSettings;
        [SerializeField] BoardUI boardUI;
        Result gameResult;

        Player whitePlayer;
        Player blackPlayer;
        Player playerToMove;
        List<Move> gameMoves;

        public ulong zobristDebug;
        public Board board { get; private set; }

        void Start()
        {
            Application.targetFrameRate = 60;

            whitePlayerType = gameSettings.whitePlayerType;
            blackPlayerType = gameSettings.blackPlayerType;
            whiteIsBottom = gameSettings.WhiteIsBottom;

            if (useClocks)
            {
                whiteClock.isTurnToMove = false;
                blackClock.isTurnToMove = false;
            }

            gameMoves = new List<Move>();
            board = new Board();

            NewGame(whitePlayerType, blackPlayerType, whiteIsBottom);

        }

        void Update()
        {
            zobristDebug = board.ZobristKey;

            if (gameResult == Result.Playing)
            {
                playerToMove.Update();

                if (useClocks)
                {
                    whiteClock.isTurnToMove = board.WhiteToMove;
                    whiteClock.OnTimerEndsEvent += OnTimerEnds;
                    blackClock.isTurnToMove = !board.WhiteToMove;
                    blackClock.OnTimerEndsEvent += OnTimerEnds;
                }
            }
        }

        private void OnTimerEnds()
        {
            bool isWhite = board.ColourToMove == Board.WhiteIndex;
            if (isWhite)
            {
                gameResult = Result.WhiteLosesInTime;
            }
            else
            {
                gameResult = Result.BlackLosesInTime;
            }
        }

        void OnMoveChosen(Move move)
        {
            StartCoroutine(OnMoveChosenDelayed(move));
        }
        IEnumerator OnMoveChosenDelayed(Move move)
        {
            bool animateMove = playerToMove is AIPlayer;
            board.MakeMove(move);

            gameMoves.Add(move);
            onMoveMade?.Invoke(move);
            boardUI.OnMoveMade(board, move, animateMove);

            yield return new WaitForSeconds(0.5f);
            NotifyPlayerToMove();
        }

        private void EvaluationValueChanged(int evalValue)
        {
            evaluationValue = evalValue;
            SetEvalValueTextAndSlider();
        }
        private void SetEvalValueTextAndSlider()
        {
            evalValueText.gameObject.SetActive(true);
            forceMateText.gameObject.SetActive(false);

            float evalFloat = evaluationValue;
            evalFloat /= 100f;

            evalSlider.value = evalFloat;
            evalValueText.text = (-evalFloat).ToString();
        }

        void NewGame(PlayerType whitePlayerType, PlayerType blackPlayerType, bool whiteIsBottom)
        {
            boardUI.SetPerspective(whiteIsBottom);
            gameMoves.Clear();
            board.LoadStartPosition();
            onPositionLoaded?.Invoke();
            boardUI.UpdatePosition(board);
            boardUI.ResetSquareColours();

            CreatePlayer(ref whitePlayer, whitePlayerType);
            CreatePlayer(ref blackPlayer, blackPlayerType);

            gameResult = Result.Playing;
            PrintGameResult(gameResult);

            NotifyPlayerToMove();
        }
        public void QuitGame()
        {
            Application.Quit();
        }

        void NotifyPlayerToMove()
        {
            gameResult = GetGameState();
            PrintGameResult(gameResult);

            if (gameResult == Result.Playing)
            {
                playerToMove = (board.WhiteToMove) ? whitePlayer : blackPlayer;
                playerToMove.NotifyTurnToMove();
            }
            else
            {
                Debug.Log("Game Over");
            }
        }

        void PrintGameResult(Result result)
        {
            float subtitleSize = resultUI.fontSize * 0.75f;
            string subtitleSettings = $"<color=#787878> <size={subtitleSize}>";

            if (result == Result.Playing)
            {
                resultUI.text = "";
                return;
            }
            if (result == Result.WhiteIsMated || result == Result.BlackIsMated)
            {
                resultUI.text = "Checkmate!";
            }
            else if (result == Result.FiftyMoveRule)
            {
                resultUI.text = "Draw";
                resultUI.text += subtitleSettings + "\n(50 move rule)";
            }
            else if (result == Result.Repetition)
            {
                resultUI.text = "Draw";
                resultUI.text += subtitleSettings + "\n(3-fold repetition)";
            }
            else if (result == Result.Stalemate)
            {
                resultUI.text = "Draw";
                resultUI.text += subtitleSettings + "\n(Stalemate)";
            }
            else if (result == Result.InsufficientMaterial)
            {
                resultUI.text = "Draw";
                resultUI.text += subtitleSettings + "\n(Insufficient material)";
            }
            else if (result == Result.WhiteLosesInTime || result == Result.BlackLosesInTime)
            {
                Debug.Log("Time is expired");
            }
            OnGameEnds?.Invoke(result);
        }

        Result GetGameState()
        {
            MoveGenerator moveGenerator = new();
            var moves = moveGenerator.GenerateMoves(board);
            bool inCheck = moveGenerator.InCheck();
            if (inCheck)
            {
                boardUI.InCheckColor(board, BoardRepresentation.CoordFromIndex(board.KingSquare[board.ColourToMoveIndex]));
            }


            // Look for mate/stalemate
            if (moves.Count == 0)
            {
                if (moveGenerator.InCheck())
                {
                    return (board.WhiteToMove) ? Result.WhiteIsMated : Result.BlackIsMated;
                }
                return Result.Stalemate;
            }

            // Fifty move rule
            if (board.fiftyMoveCounter >= 100)
            {
                return Result.FiftyMoveRule;
            }

            // Threefold repetition
            int repCount = board.RepetitionPositionHistory.Count((x => x == board.ZobristKey));
            if (repCount == 3)
            {
                return Result.Repetition;
            }

            // Look for insufficient material
            int numPawns = board.pawns[Board.WhiteIndex].Count + board.pawns[Board.BlackIndex].Count;
            int numRooks = board.rooks[Board.WhiteIndex].Count + board.rooks[Board.BlackIndex].Count;
            int numQueens = board.queens[Board.WhiteIndex].Count + board.queens[Board.BlackIndex].Count;
            int numKnights = board.knights[Board.WhiteIndex].Count + board.knights[Board.BlackIndex].Count;
            int numBishops = board.bishops[Board.WhiteIndex].Count + board.bishops[Board.BlackIndex].Count;

            if (numPawns + numRooks + numQueens == 0)
            {
                if ((numKnights == 1 && numBishops == 0) ||
                    (numKnights == 0 && numBishops == 1) ||
                    (numKnights == 0 && numBishops == 0))
                {
                    return Result.InsufficientMaterial;
                }
                else if (numKnights == 1 && numBishops == 1)
                {
                    // Check if the bishops are on the same color square (not implemented yet)
                }
            }

            return Result.Playing;
        }

        void CreatePlayer(ref Player player, PlayerType playerType)
        {
            if (player != null)
            {
                player.OnMoveChosen -= OnMoveChosen;
                player.OnEvaluationValueChanged -= EvaluationValueChanged;
                player.OnForceMateDetected -= ForceMateDetected;
            }

            if (playerType == PlayerType.Human)
            {
                player = new HumanPlayer(board);
            }
            else
            {
                player = new AIPlayer(board, aiSettings);
            }
            player.OnMoveChosen += OnMoveChosen;
            player.OnEvaluationValueChanged += EvaluationValueChanged;
            player.OnForceMateDetected += ForceMateDetected;
        }

        private void ForceMateDetected(int mateIn)
        {
            evalValueText.gameObject.SetActive(false);
            forceMateText.gameObject.SetActive(true);

            forceMateText.text = $"M{mateIn}";
        }
    }
}