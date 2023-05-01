using Chess.Game;
using System;
using UnityEngine;

public delegate void OnTimerEndsDelegate();
public class Clock : MonoBehaviour
{
    public event OnTimerEndsDelegate OnTimerEndsEvent;
    public TMPro.TMP_Text timerUI;
    [SerializeField] GameManager manager;
    public bool isTurnToMove;
    public int startSeconds;
    float secondsRemaining;
    public int lowTimeThreshold = 10;
    [Range(0, 1)]
    public float inactiveAlpha = 0.75f;
    [Range(0, 1)]
    public float decimalFontSizeMultiplier = 0.75f;
    public Color lowTimeCol;
    bool gameIsRunning = true;

    private void OnEnable()
    {
        manager.OnGameEnds += OnGameEnds;
    }

    private void OnGameEnds(GameManager.Result result)
    {
        gameIsRunning = false;
    }

    void Start()
    {
        secondsRemaining = startSeconds;
    }

    void Update()
    {
        if (gameIsRunning)
        {
            RunTimer();
        }
    }
    void RunTimer()
    {
        if (isTurnToMove)
        {
            secondsRemaining -= Time.deltaTime;
            secondsRemaining = Mathf.Max(0, secondsRemaining);
        }
        int numMinutes = (int)(secondsRemaining / 60);
        int numSeconds = (int)(secondsRemaining - numMinutes * 60);

        timerUI.text = $"{numMinutes:00}:{numSeconds:00}";
        if (secondsRemaining <= lowTimeThreshold)
        {
            int dec = (int)((secondsRemaining - numSeconds) * 10);
            float size = timerUI.fontSize * decimalFontSizeMultiplier;
            timerUI.text += $"<size={size}>.{dec}</size>";
        }

        var col = Color.white;
        if ((int)secondsRemaining <= lowTimeThreshold)
        {
            col = lowTimeCol;
        }
        if (!isTurnToMove)
        {
            col = new Color(col.r, col.g, col.b, inactiveAlpha);
        }
        if ((int)secondsRemaining <= 0)
        {
            OnTimerEndsEvent?.Invoke();
        }
        timerUI.color = col;
    }
}