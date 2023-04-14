using System.Collections;
using TMPro;
using UnityEngine;

namespace Chess.Game
{
    public class BoardUI : MonoBehaviour
    {
        [SerializeField] PieceTheme pieceTheme;
        [SerializeField] BoardTheme boardTheme;
        [SerializeField] bool showLegalMoves;

        [SerializeField] TextMeshProUGUI[] ranks;
        [SerializeField] TextMeshProUGUI[] files;
        
        public bool whiteIsBottom = true;

        SpriteRenderer[,] squareRenderers;
        SpriteRenderer[,] squarePieceRenderers;
        SpriteRenderer[,] squareHighlightsRenderers;
        Move lastMadeMove;
        MoveGenerator moveGenerator;

        const float pieceDepth = -0.1f;
        const float pieceDragDepth = -0.2f;
        (bool, Coord) inCheck;

        void Awake()
        {
            moveGenerator = new MoveGenerator();
            CreateBoardUI();
        }

        public void InCheckColor(Board board, Coord checkSquare)
        {
            SetSquareColor(checkSquare, boardTheme.inCheck);
            inCheck = (true, checkSquare);
        }

        public void HighlightLegalMoves(Board board, Coord fromSquare)
        {
            if (showLegalMoves)
            {
                var moves = moveGenerator.GenerateMoves(board);

                for (int i = 0; i < moves.Count; i++)
                {
                    Move move = moves[i];
                    if (move.StartSquare == BoardRepresentation.IndexFromCoord(fromSquare))
                    {
                        Coord coord = BoardRepresentation.CoordFromIndex(move.TargetSquare);
                        int squareIndex = BoardRepresentation.IndexFromCoord(coord);
                        SetHighlightSprite(coord, board, squareIndex);
                    }
                }
            }
        }

        private void SetHighlightSprite(Coord coord, Board board, int squareIndex)
        {
            int file = coord.fileIndex;
            int rank = coord.rankIndex;
            int piece = board.GetPieceAt(squareIndex);
            if (piece == Piece.None)
            {
                squareHighlightsRenderers[file, rank].sprite = boardTheme.legal;
            }
            else
            {
                squareHighlightsRenderers[file, rank].sprite = boardTheme.canAttackHighlight;
            }
        }

        public void DragPiece(Coord pieceCoord, Vector2 mousePos)
        {
            squarePieceRenderers[pieceCoord.fileIndex, pieceCoord.rankIndex].transform.position = new Vector3(mousePos.x, mousePos.y, pieceDragDepth);
        }

        public void ResetPiecePosition(Coord pieceCoord)
        {
            Vector3 pos = PositionFromCoord(pieceCoord.fileIndex, pieceCoord.rankIndex, pieceDepth);
            squarePieceRenderers[pieceCoord.fileIndex, pieceCoord.rankIndex].transform.position = pos;
        }

        public void SelectSquare(Coord coord)
        {
            SetSquareColor(coord, boardTheme.selected);
        }

        public void DeselectSquare()
        {
            ResetSquareColours();
        }

        public bool TryGetSquareUnderMouse(Vector2 mouseWorld, out Coord selectedCoord)
        {
            int file = (int)(mouseWorld.x + 4);
            int rank = (int)(mouseWorld.y + 4);
            if (!whiteIsBottom)
            {
                file = 7 - file;
                rank = 7 - rank;
            }
            selectedCoord = new Coord(file, rank);
            return file >= 0 && file < 8 && rank >= 0 && rank < 8;
        }

        public void UpdatePosition(Board board)
        {
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    Coord coord = new(file, rank);
                    int piece = board.Square[BoardRepresentation.IndexFromCoord(coord.fileIndex, coord.rankIndex)];
                    squarePieceRenderers[file, rank].sprite = pieceTheme.GetPieceSprite(piece);
                    squarePieceRenderers[file, rank].transform.position = PositionFromCoord(file, rank, pieceDepth);
                    squareHighlightsRenderers[file, rank].sprite = null;
                }
            }

        }

        public void OnMoveMade(Board board, Move move, bool animate = false)
        {
            lastMadeMove = move;

            if (inCheck.Item1)
                inCheck.Item1 = false;

            if (animate)
            {
                StartCoroutine(AnimateMove(move, board));
            }
            else
            {
                UpdatePosition(board);
                ResetSquareColours();
            }
        }

        IEnumerator AnimateMove(Move move, Board board)
        {
            float t = 0;
            const float moveAnimDuration = 0.15f;
            Coord startCoord = BoardRepresentation.CoordFromIndex(move.StartSquare);
            Coord targetCoord = BoardRepresentation.CoordFromIndex(move.TargetSquare);
            Transform pieceT = squarePieceRenderers[startCoord.fileIndex, startCoord.rankIndex].transform;
            Vector3 startPos = PositionFromCoord(startCoord);
            Vector3 targetPos = PositionFromCoord(targetCoord);
            SetSquareColor(BoardRepresentation.CoordFromIndex(move.StartSquare), boardTheme.moveFromHighlight);

            while (t <= 1)
            {
                yield return null;
                t += Time.deltaTime * 1 / moveAnimDuration;
                pieceT.position = Vector3.Lerp(startPos, targetPos, t);
            }
            UpdatePosition(board);
            ResetSquareColours();
            pieceT.position = startPos;
        }

        void HighlightMove(Move move)
        {
            SetSquareColor(BoardRepresentation.CoordFromIndex(move.StartSquare), boardTheme.moveFromHighlight);
            SetSquareColor(BoardRepresentation.CoordFromIndex(move.TargetSquare), boardTheme.moveToHighlight);
        }

        void CreateBoardUI()
        {
            squareRenderers = new SpriteRenderer[8, 8];
            squarePieceRenderers = new SpriteRenderer[8, 8];
            squareHighlightsRenderers = new SpriteRenderer[8, 8];

            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    // Create Square
                    Transform square = new GameObject().GetComponent<Transform>();
                    square.parent = transform;
                    square.name = BoardRepresentation.SquareNameFromCoordinate(file, rank);
                    square.position = PositionFromCoord(file, rank, 0);
                    square.gameObject.AddComponent<SpriteRenderer>();

                    squareRenderers[file, rank] = square.gameObject.GetComponent<SpriteRenderer>();

                    // Create piece sprite renderer for current square
                    SpriteRenderer pieceRenderer = new GameObject("Piece").AddComponent<SpriteRenderer>();
                    pieceRenderer.transform.parent = square;
                    pieceRenderer.transform.position = PositionFromCoord(file, rank, pieceDepth);
                    pieceRenderer.transform.localScale = Vector3.one * 100 / (2000 / 6f);
                    squarePieceRenderers[file, rank] = pieceRenderer;

                    // Create Highlights sprite for current square
                    SpriteRenderer highlightRenderer = new GameObject("Highlight").AddComponent<SpriteRenderer>();
                    highlightRenderer.transform.parent = square;
                    highlightRenderer.transform.position = PositionFromCoord(file, rank, pieceDepth);
                    highlightRenderer.transform.localScale = Vector3.one;
                    highlightRenderer.color = new Color(0.4f, 0.4f, 0.4f, 0.8f);
                    squareHighlightsRenderers[file, rank] = highlightRenderer;
                }
            }
            ResetSquareColours();
        }

        void ResetSquarePositions()
        {
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    if (file == 0 && rank == 0)
                    {
                        //Debug.Log (squarePieceRenderers[file, rank].gameObject.name + "  " + PositionFromCoord (file, rank, pieceDepth));
                    }
                    //squarePieceRenderers[file, rank].transform.position = PositionFromCoord (file, rank, pieceDepth);
                    squareRenderers[file, rank].transform.position = PositionFromCoord(file, rank, 0);
                    squarePieceRenderers[file, rank].transform.position = PositionFromCoord(file, rank, pieceDepth);
                }
            }

            if (!lastMadeMove.IsInvalid)
            {
                HighlightMove(lastMadeMove);
            }
        }

        void SetRankFileText(bool whitePOV)
        {
            if (whitePOV)
            {
                for (int i = 0; i < 8; ++i)
                {
                    if(i%2 == 0)
                    {
                        ranks[i].color = Color.black;
                        files[i].color = Color.white;
                    }
                    else
                    {
                        ranks[i].color = Color.white;
                        files[i].color = Color.black;
                    }
                    ranks[i].text = (8 - i).ToString();
                    files[i].text = ((char)(i + 'a')).ToString();
                }
            }
            else
            {
                for (int i = 0; i < 8; ++i)
                {
                    if (i % 2 == 0)
                    {
                        ranks[i].color = Color.black;
                        files[i].color = Color.white;
                    }
                    else
                    {
                        ranks[i].color = Color.white;
                        files[i].color = Color.black;
                    }
                    ranks[i].text = (i + 1).ToString();
                    files[i].text = ((char)(7 - i + 'a')).ToString();
                }
            }
        }

        public void SetPerspective(bool whitePOV)
        {
            whiteIsBottom = whitePOV;
            SetRankFileText(whitePOV);
            ResetSquarePositions();
        }

        public void ResetSquareColours(bool highlight = true)
        {
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    SetSquareSprite(new Coord(file, rank), boardTheme.lightSquares.normal, boardTheme.darkSquares.normal);
                    squareHighlightsRenderers[file, rank].sprite = null;
                }
            }
            if (highlight)
            {
                if (!lastMadeMove.IsInvalid)
                {
                    HighlightMove(lastMadeMove);
                }
            }
        }

        void SetSquareSprite(Coord square, Sprite lightSprite, Sprite darkSprite)
        {
            squareRenderers[square.fileIndex, square.rankIndex].sprite = (square.IsLightSquare()) ? lightSprite : darkSprite;
            if (inCheck == (true, square))
            {
                return;
            }
            squareRenderers[square.fileIndex, square.rankIndex].color = Color.white;
        }

        void SetSquareColor(Coord square, Color color)
        {
            if (inCheck == (true, square))
            {
                return;
            }
            squareRenderers[square.fileIndex, square.rankIndex].color = color;
        }

        public Vector3 PositionFromCoord(int file, int rank, float depth = 0)
        {
            if (whiteIsBottom)
            {
                return new Vector3(-3.5f + file, -3.5f + rank, depth);
            }
            return new Vector3(-3.5f + 7 - file, 7 - rank - 3.5f, depth);

        }

        public Vector3 PositionFromCoord(Coord coord, float depth = 0)
        {
            return PositionFromCoord(coord.fileIndex, coord.rankIndex, depth);
        }
    }
}