using Chess;
using Chess.Game;
using UnityEngine;
using UnityEngine.UI;

public enum PieceType
{
    Pawn,
    Bishop,
    Knight,
    Rook,
    Queen
}
public class PieceTypeMaterialBar : MonoBehaviour
{
    [SerializeField] PieceType pieceType;
    [SerializeField] RectTransform rect;
    [SerializeField] BoardUI boardUI;

    private int pieceCount;
    [SerializeField] float pieceSize;
    [SerializeField] Sprite pieceSprite;
    [SerializeField] float spacing;
    [SerializeField] int maxPieceCount;
    [SerializeField] GameManager gameManager;
    [SerializeField] bool isCapturingWhitePieces;
    private int color;
    private int type;
    private Board board;

    private int oldCount = 0;

    private void OnEnable()
    {
        gameManager.onPositionLoaded += Init;
        gameManager.onMoveMade += OnMove;
    }
    void Init()
    {
        board = gameManager.board;
        color = isCapturingWhitePieces ? Board.WhiteIndex : Board.BlackIndex;
        type = GePieceTypeIndex();
        rect.sizeDelta = Vector2.zero;
        Clear();
    }
    void Clear()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private int GePieceTypeIndex()
    {
        return pieceType switch
        {
            PieceType.Pawn => Piece.Pawn,
            PieceType.Bishop => Piece.Bishop,
            PieceType.Knight => Piece.Knight,
            PieceType.Rook => Piece.Rook,
            PieceType.Queen => Piece.Queen,
            _ => 0,
        };
    }

    private void OnMove(Move move)
    {
        pieceCount = maxPieceCount - board.GetPieceListCount(type, color);

        if (pieceCount == oldCount)
            return;

        var newObject = new GameObject("alo");
        newObject.transform.parent = transform;
        var image = newObject.AddComponent<Image>();
        image.sprite = pieceSprite;
        newObject.GetComponent<RectTransform>().sizeDelta = new Vector2(pieceSize, pieceSize);
        newObject.GetComponent<RectTransform>().localScale = Vector3.one;
        var newWidth = CalculateWidth();
        rect.sizeDelta = new Vector2(newWidth, pieceSize);
        oldCount = pieceCount;
    }

    private float CalculateWidth()
    {
        return pieceSize + (pieceCount - 1) * (pieceSize + spacing);
    }
}
