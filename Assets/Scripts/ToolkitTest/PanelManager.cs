using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PanelManager : MonoBehaviour
{
    [SerializeField] PanelView mainMenuPanel;
    [SerializeField] PanelView botselectionPanel;
    List<string> panelNames = new();

    private void OnEnable()
    {
        mainMenuPanel.GetComponent<MainMenuToolkitManager>().OnBotSelectionMenuEnter += ShowBotSelectionMenu;
        botselectionPanel.GetComponent<BotSelectionViewManager>().OnMainMenuBack += ShowMainMenu;
    }

    private void OnDestroy()
    {
        mainMenuPanel.GetComponent<MainMenuToolkitManager>().OnBotSelectionMenuEnter -= ShowBotSelectionMenu;
        botselectionPanel.GetComponent<BotSelectionViewManager>().OnMainMenuBack -= ShowMainMenu;
    }
    void Start()
    {
        panelNames.Add(mainMenuPanel.name);
        panelNames.Add(botselectionPanel.name);
        ShowPanel(mainMenuPanel);
    }
    private void ShowMainMenu()
    {
        ShowPanel(mainMenuPanel);
    }

    void ShowBotSelectionMenu()
    {
        ShowPanel(botselectionPanel);
    }
    void ShowPanel(PanelView panel)
    {
        foreach(string panelName in panelNames)
        {
            if(panelName == panel.name)
            {
                panel.GetUIDocument().rootVisualElement.style.display = DisplayStyle.Flex;
            }
            else
            {
                panel.GetUIDocument().rootVisualElement.style.display = DisplayStyle.None;
            }
        }
    }
}
