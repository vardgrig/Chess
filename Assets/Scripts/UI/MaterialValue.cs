using Chess;
using Chess.Game;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MaterialValue : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI bottomMaterialValueText;
    [SerializeField] TextMeshProUGUI topMaterialValueText;
    [SerializeField] GameManager gameManager;
    private Board board;

    List<PieceType> pieceTypes = new() { PieceType.Pawn, PieceType.Bishop, PieceType.Knight, PieceType.Rook , PieceType.Queen};
    int whitePieceValueCount = 0;
    int blackPieceValueCount = 0;

    private void OnEnable()
    {
        gameManager.onPositionLoaded += Init;
        gameManager.onMoveMade += CompareMaterialValues;
    }
    private void Init()
    {
        board = gameManager.board;
        ClearText();
    }

    void ClearText()
    {
        bottomMaterialValueText.text = "";
        topMaterialValueText.text = "";
    }
    void CompareMaterialValues(Move move)
    {
        GetPieceValues();
        int materialDifferene = Mathf.Abs(whitePieceValueCount - blackPieceValueCount);
        
        if(whitePieceValueCount > blackPieceValueCount)
        {
            bottomMaterialValueText.text = $"+{materialDifferene}";
            topMaterialValueText.text = "";
        }
        else if(blackPieceValueCount > whitePieceValueCount)
        {
            topMaterialValueText.text = $"+{materialDifferene}";
            bottomMaterialValueText.text = "";
        }
        else
        {
            topMaterialValueText.text = "";
            bottomMaterialValueText.text = "";
        }
    }
    void GetPieceValues()
    {
        whitePieceValueCount = 0;
        blackPieceValueCount = 0;
        foreach (PieceType pieceType in pieceTypes)
        {
            switch (pieceType)
            {
                case PieceType.Pawn:
                    whitePieceValueCount += 1 * board.GetPieceListCount(Piece.Pawn, Board.WhiteIndex);
                    blackPieceValueCount += 1 * board.GetPieceListCount(Piece.Pawn, Board.BlackIndex);
                    break;
                case PieceType.Bishop:
                    whitePieceValueCount += 3 * board.GetPieceListCount(Piece.Bishop, Board.WhiteIndex);
                    blackPieceValueCount += 3 * board.GetPieceListCount(Piece.Bishop, Board.BlackIndex);
                    break;
                case PieceType.Knight:
                    whitePieceValueCount += 3 * board.GetPieceListCount(Piece.Knight, Board.WhiteIndex);
                    blackPieceValueCount += 3 * board.GetPieceListCount(Piece.Knight, Board.BlackIndex);
                    break;
                case PieceType.Rook:
                    whitePieceValueCount += 5 * board.GetPieceListCount(Piece.Rook, Board.WhiteIndex);
                    blackPieceValueCount += 5 * board.GetPieceListCount(Piece.Rook, Board.BlackIndex);
                    break;
                case PieceType.Queen:
                    whitePieceValueCount += 9 * board.GetPieceListCount(Piece.Queen, Board.WhiteIndex);
                    blackPieceValueCount += 9 * board.GetPieceListCount(Piece.Queen, Board.BlackIndex);
                    break;
            }
        }
    }
}
