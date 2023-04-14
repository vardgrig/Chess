using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Chess.Game.GameManager;

public class MenuManager : MonoBehaviour
{
    const string GAME_VIEW_SCENE_KEY = "ChessGame";
    [Header("Global")]
    [SerializeField] Button githubLink;
    [Header("Main Menu")]
    [SerializeField] Button versusBot;
    [SerializeField] Button versusPlayer;
    [SerializeField] Button exit;
    [Header("BotSelection Menu")]
    [SerializeField] TextMeshProUGUI playAsText;
    [SerializeField] Button whiteColor;
    [SerializeField] Button blackColor;
    [SerializeField] Button startBotGame;
    [SerializeField] Button backMain;
    [Header("View")]
    [SerializeField] GameObject mainMenuView;
    [SerializeField] GameObject botSelectionView;

    [SerializeField] GameSettings gameSettings;


    void OnEnable()
    {
        versusBot.onClick.AddListener(VersusBot);
        versusPlayer.onClick.AddListener(VersusPlayer);
        exit.onClick.AddListener(Exit);
        githubLink.onClick.AddListener(GoToGithub);
        startBotGame.onClick.AddListener(StartVersusBot);
        whiteColor.onClick.AddListener(() => SelectColor("white"));
        blackColor.onClick.AddListener(() => SelectColor("black"));
        backMain.onClick.AddListener(BackToMainMenu);
    }

    void OnDisable()
    {
        versusBot.onClick.RemoveListener(VersusBot);
        versusPlayer.onClick.RemoveListener(VersusPlayer);
        exit.onClick.RemoveListener(Exit);
        githubLink.onClick.RemoveListener(GoToGithub);
    }

    void Start()
    {
        SelectView(mainMenuView, botSelectionView);
    }

    void SelectView(GameObject viewToShow, GameObject viewToHide)
    {
        viewToShow.SetActive(true);
        viewToHide.SetActive(false);
    }

    void VersusPlayer()
    {
        gameSettings.WhiteIsBottom = true;
        gameSettings.whitePlayerType = PlayerType.Human;
        gameSettings.blackPlayerType = PlayerType.Human;
        StartGame();
    }

    void VersusBot()
    {
        SelectView(botSelectionView, mainMenuView);
        SelectColor("white");
    }

    void StartGame()
    {
        SceneManager.LoadScene(GAME_VIEW_SCENE_KEY);
    }
    void StartVersusBot()
    {
        StartGame();
    }

    void SelectColor(string color)
    {
        if(color == "white")
        {
            playAsText.text = "Play as White";
            gameSettings.WhiteIsBottom = true;
            gameSettings.whitePlayerType = PlayerType.Human;
            gameSettings.blackPlayerType = PlayerType.AI;
        }
        else
        {
            playAsText.text = "Play as Black";
            gameSettings.WhiteIsBottom = false;
            gameSettings.whitePlayerType = PlayerType.AI;
            gameSettings.blackPlayerType = PlayerType.Human;
        }
    }
    void BackToMainMenu()
    {
        SelectView(mainMenuView, botSelectionView);
    }
    void Exit()
    {
        Application.Quit();
    }
    void GoToGithub()
    {
        Application.OpenURL("https://github.com/vardgrig/Chessity");
    }
}
