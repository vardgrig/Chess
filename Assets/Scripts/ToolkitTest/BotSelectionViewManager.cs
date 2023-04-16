using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public delegate void OnMainMenuBack();
public class BotSelectionViewManager : PanelView
{
    const string CHESS_GAME_KEY = "ChessGame";
    [SerializeField] UIDocument m_UIDocument;
    [SerializeField] GameSettings m_GameSettings;
    Label m_PlayAsLabel;
    Button m_WhitePiece;
    Button m_BlackPiece;
    Button m_StartGame;
    Button m_Back;

    public event OnMainMenuBack OnMainMenuBack;

    private void Start()
    {
        var rootElement = m_UIDocument.rootVisualElement; // doesn't work

        m_PlayAsLabel = rootElement.Q<Label>("PlayAsText");
        m_WhitePiece = rootElement.Q<Button>("WhitePiece");
        m_BlackPiece = rootElement.Q<Button>("BlackPiece");
        m_StartGame = rootElement.Q<Button>("StartGame");
        m_Back = rootElement.Q<Button>("Back");
        AssignButtons();
        OnWhitePieceSelected();
    }
    public override UIDocument GetUIDocument()
    {
        return m_UIDocument;
    }

    void AssignButtons()
    {
        m_WhitePiece.clickable.clicked += OnWhitePieceSelected;
        m_BlackPiece.clickable.clicked += OnBlackPieceSelected;
        m_StartGame.clickable.clicked += OnStartGame;
        m_Back.clickable.clicked += BackToMenu;
    }
    void SetLabelText(bool isWhiteSelected)
    {
        StyleColor labelColor = m_PlayAsLabel.style.color;
        if (isWhiteSelected)
            m_PlayAsLabel.text = "Play as " + "<color=White>White</color>";
        else
            m_PlayAsLabel.text = "Play as " + "<color=Black>Black</color>";
        m_PlayAsLabel.style.color = labelColor;
    }
    void OnWhitePieceSelected()
    {
        m_WhitePiece.Focus();
        m_GameSettings.whitePlayerType = Chess.Game.GameManager.PlayerType.Human;
        m_GameSettings.blackPlayerType = Chess.Game.GameManager.PlayerType.AI;
        m_GameSettings.WhiteIsBottom = true;
        SetLabelText(true);
    }
    void OnBlackPieceSelected()
    {
        m_BlackPiece.Focus();
        m_GameSettings.whitePlayerType = Chess.Game.GameManager.PlayerType.AI;
        m_GameSettings.blackPlayerType = Chess.Game.GameManager.PlayerType.Human;
        m_GameSettings.WhiteIsBottom = false;
        SetLabelText(false);
    }
    void OnStartGame()
    {
        Debug.Log("Start Game");
        SceneManager.LoadScene(CHESS_GAME_KEY);
    }

    void BackToMenu()
    {
        Debug.Log("Back to Main Menu");
        OnMainMenuBack?.Invoke();
    }
}
