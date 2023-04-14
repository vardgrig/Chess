using UnityEngine;
namespace Chess.Game {
	[CreateAssetMenu (menuName = "Theme/Board")]
	public class BoardTheme : ScriptableObject {

		public SquareSprites lightSquares;
		public SquareSprites darkSquares;
        public Sprite legal;
        public Sprite canAttackHighlight;
        public Color  selected;
        public Color moveFromHighlight;
        public Color moveToHighlight;
		public Color inCheck;

        [System.Serializable]
		public struct SquareSprites {
			public Sprite normal;
		}
	}
}