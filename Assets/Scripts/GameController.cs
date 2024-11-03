using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("Timer")]
    public float startTime = 60f; // Time in seconds for the countdown
    public TextMeshProUGUI timerText; // UI Text element to display the timer

    [Header ("Goals")]
    public Goal playerOneGoal;
    public Goal playerTwoGoal;

    public TextMeshProUGUI playerOneScoreText;
    public TextMeshProUGUI playerTwoScoreText;

    private int playerOneScore = 0;
    private int playerTwoScore = 0;

    private float timeRemaining;
    private bool timerIsRunning = false;

    public static string BALL_TAG = "Ball";
    public static string GROUND_TAG = "Ground";
    public static string PLAYER_TAG = "Player";

    void Start()
    {
        timeRemaining = startTime;
        timerIsRunning = true;

        playerOneGoal.SetGoalScoredAction(GoalScored);
        playerTwoGoal.SetGoalScoredAction(GoalScored);
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                TimerEnded();
            }
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);

        // If minutes are single digit, display without leading zero
        if (minutes < 10)
        {
            timerText.text = string.Format("{0}:{1:00}", minutes, seconds);
        }
        else
        {
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    void GoalScored(int playerNumber)
    {
        if (playerNumber == 1)
        {
            playerOneScore++;
            Debug.Log("Player One scored! Player One Score: " + playerOneScore);
        }
        else if (playerNumber == 2)
        {
            playerTwoScore++;
            Debug.Log("Player Two scored! Player Two Score: " + playerTwoScore);
        }

        SetScoreTexts(playerOneScore.ToString(), playerTwoScore.ToString());
    }

    void SetScoreTexts(string argPlayerOneScoreText, string argPlayerTwoScoreText)
    {
        playerOneScoreText.text = argPlayerOneScoreText;
        playerTwoScoreText.text = argPlayerTwoScoreText;
    }

    void TimerEnded()
    {
        // Do something when the timer ends
        Debug.Log("Time's up!");
    }
}
