using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum State
    {
        waitingToStart,
        countdownToStart,
        gamePlaying,
        gameOver
    }

    private State state;

    private float countdownToStartTimer = 3f;
    private float gamePlayingTimer;
    private float gamePlayingTimerMax = 60f;

    public static GameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;

    private bool isPaused;

    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;

    private void Awake()
    {
        state = State.waitingToStart;
        Instance = this;

    }

    private void Start()
    {
        GameInput.Instance.OnPauseAction += (sender, args) =>
        {
            TogglePauseGame();
        };
        GameInput.Instance.OnInteractAction += (sender, args) =>
        {
            if (state == State.waitingToStart)
            {
                SetState(State.countdownToStart);
            }
        };
    }
    private void Update()
    {
        switch (state)
        {
            case State.waitingToStart:
                break;
            case State.countdownToStart:
                countdownToStartTimer -= Time.deltaTime;
                if (countdownToStartTimer < 0f)
                {
                    gamePlayingTimer = gamePlayingTimerMax;
                    SetState(State.gamePlaying);
                }
                break;
            case State.gamePlaying:
                gamePlayingTimer -= Time.deltaTime;
                if (gamePlayingTimer < 0f)
                {
                    SetState(State.gameOver);
                }
                break;
            case State.gameOver:
                break;

        }


    }

    public bool IsGamePlaying()
    {
        return state == State.gamePlaying;
    }
    public bool IsCountdownGameStart()
    {
        return state == State.countdownToStart;
    }

    private void SetState(State state)
    {
        this.state = state;
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }
    public float GetCountdownTimer()
    {
        return countdownToStartTimer;
    }
    public bool IsGameOver()
    {
        return state == State.gameOver;
    }
    public float GetGamePlayingNormalized()
    {
        return 1 - (gamePlayingTimer / gamePlayingTimerMax);
    }
    public void TogglePauseGame()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            Time.timeScale = 0f;
            OnGamePaused?.Invoke(this, EventArgs.Empty);

        }
        else
        {
            Time.timeScale = 1f;
            OnGameUnpaused?.Invoke(this, EventArgs.Empty);

        }
    }
}
