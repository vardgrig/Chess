namespace Chess.Game
{
    public abstract class Player
    {
        public event System.Action<Move> OnMoveChosen;
        public event System.Action<int> OnEvaluationValueChanged;
        public event System.Action<int> OnForceMateDetected;

        public abstract void Update();

        public abstract void NotifyTurnToMove();

        protected virtual void ChoseMove(Move move)
        {
            OnMoveChosen?.Invoke(move);
        }
        protected virtual void OnForceMate(int mateIn)
        {
            OnForceMateDetected?.Invoke(mateIn);
        }
        protected virtual void OnEvaluationValueChange(int value)
        {
            OnEvaluationValueChanged?.Invoke(value);
        }
    }
}