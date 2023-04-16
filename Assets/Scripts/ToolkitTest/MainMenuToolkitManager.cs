using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
public class MainMenuToolkitManager : PanelView
{
    [SerializeField] UIDocument m_UIDocument;
    [SerializeField] PanelManager m_PanelManager;
    
    Button m_playBot;
    Button m_playPlayer;
    Button m_exit;

    void OnEnable()
    {
        GetElementReference();
        AssignButtons();
    }
    void OnDisable()
    {
        RemoveButtonListeners();
    }

    void GetElementReference()
    {
        var rootElement = m_UIDocument.rootVisualElement;
        m_playBot = rootElement.Q<Button>("PlayBot");
        m_playPlayer = rootElement.Q<Button>("PlayPlayer");
        m_exit = rootElement.Q<Button>("Exit");
    }
    public override UIDocument GetUIDocument()
    {
        return m_UIDocument;
    }

    void AssignButtons()
    {
        m_playBot.clickable.clicked += OnPlayBot;
        m_playPlayer.clickable.clicked += OnPlayPlayer;
        m_exit.clickable.clicked += OnExit;
    }
    void RemoveButtonListeners()
    {
        m_playBot.clickable.clicked -= OnPlayBot;
        m_playPlayer.clickable.clicked -= OnPlayPlayer;
        m_exit.clickable.clicked -= OnExit;
    }
    void OnPlayBot()
    {
        Debug.Log("Play Bot");
        m_PanelManager.ShowBotSelectionMenu();
    }

    void OnPlayPlayer()
    {
        Debug.Log("Play Player");
    }

    void OnExit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
