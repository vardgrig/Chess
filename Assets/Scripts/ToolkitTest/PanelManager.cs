using UnityEngine;
using UnityEngine.UIElements;

public class PanelManager : MonoBehaviour
{
    [SerializeField] UIDocument m_UIDocument;
    [SerializeField] PanelView mainMenuPanel;
    [SerializeField] PanelView botSelectionPanel;

    [SerializeField] VisualTreeAsset menuAsset;
    [SerializeField] VisualTreeAsset botSelectionAsset;


    private void Awake()
    {
        ShowMenu();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.B)) 
        {
            ShowBotSelectionMenu();
        }
        else if(Input.GetKeyDown(KeyCode.A))
        {
            ShowMenu();
        }
    }
    public void ShowMenu()
    {
        m_UIDocument.visualTreeAsset = menuAsset;
        mainMenuPanel.enabled = true;
        botSelectionPanel.enabled = false;
    }
    public void ShowBotSelectionMenu()
    {
        m_UIDocument.visualTreeAsset = botSelectionAsset;
        mainMenuPanel.enabled = false;
        botSelectionPanel.enabled = true;
    }
}
