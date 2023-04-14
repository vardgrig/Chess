using Chess;
using Chess.Game;
using UnityEngine;
using UnityEngine.UI;

public class MaterialBar : MonoBehaviour
{
    public GameManager gameManager;
    public Image piecePrefab; // The prefab for the individual pieces
    public float spacing; // The spacing between the pieces
    public float maxWidth;
    private Board board;
    [SerializeField] RectTransform rectTransform;
    private float[] pieceWidths = new float[5];
    private int[] pieceCounts = new int[5];

    void Start()
    {
        board = gameManager.board;
        rectTransform = GetComponent<RectTransform>();
        CalculatePieceWidths();
        UpdateMaterialBar();
    }

    void CalculatePieceWidths()
    {
        float totalWidth = maxWidth - (4 * spacing);

        // Get the number of pieces of each type
        pieceCounts[0] = board.pawns.Length;
        pieceCounts[1] = board.rooks.Length;
        pieceCounts[2] = board.bishops.Length;
        pieceCounts[3] = board.knights.Length;
        pieceCounts[4] = board.queens.Length;

        // Calculate the total number of pieces
        int totalPieces = 0;
        for(int i = 0; i < pieceCounts.Length; ++i)
        {
            totalPieces += pieceCounts[i];
        }

        for (int i = 0; i < 5; i++)
        {
            if (totalPieces == 0)
            {
                pieceWidths[i] = 0f;
            }
            else
            {
                float percentage = pieceCounts[i] / totalPieces;
                pieceWidths[i] = Mathf.RoundToInt(percentage * totalWidth);
            }
        }
    }

    void UpdateMaterialBar()
    {
        float x = 0f;

        for (int i = 0; i < 5; i++)
        {
            Transform pieceContainer = transform.GetChild(i);
            pieceContainer.GetComponent<HorizontalLayoutGroup>().spacing = spacing;
            float width = pieceWidths[i];
            pieceContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(width, 0f);
            float containerHeight = pieceContainer.GetComponent<RectTransform>().rect.height;
            float y = -containerHeight / 2f;
            foreach (Transform child in pieceContainer)
            {
                child.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
                y += containerHeight / pieceCounts[i];
            }
            x += width + spacing;
        }

        float totalWidth = x - spacing;
        rectTransform.sizeDelta = new Vector2(totalWidth, rectTransform.sizeDelta.y);
    }
}