using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    private void Awake()
    {
        // Check if an instance of GameController already exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy this instance if one already exists
            return;
        }

        // Set the instance to this instance and mark it to persist across scenes
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    [Header("Timer")]
    public float startTime = 60f; // Time in seconds for the countdown
    public TextMeshProUGUI timerText; // UI Text element to display the timer

    [Header("Players & Ball")]
    public Ball ball;
    public PlayerController playerOne;
    public PlayerController playerTwo;

    public Vector2 ballRandomVariationX;
    public Vector2 ballRandomVariationY;

    [Header("Event Text")]
    public Animation EventTextAnimation;
    public TextMeshProUGUI EventText;

    [Header ("Goals")]
    public Goal playerOneGoal;
    public Goal playerTwoGoal;
    public ParticleSystem goalScored;

    public TextMeshProUGUI playerOneScoreText;
    public TextMeshProUGUI playerTwoScoreText;

    private Vector3 ballResetPoint;
    private Vector3 playerOneResetPoint;
    private Vector3 playerOneResetScale;
    private Vector3 playerTwoResetPoint;
    private Vector3 playerTwoResetScale;
    public bool playerOneStartDirection;
    public bool playerTwoStartDirection;

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

        StopPlay();

        playerOneGoal.SetGoalScoredAction(GoalScored);
        playerTwoGoal.SetGoalScoredAction(GoalScored);
         
        ballResetPoint = ball.transform.position;
        playerOneResetPoint = playerOne.transform.position;
        playerTwoResetPoint = playerTwo.transform.position;

        playerOneResetScale = playerOne.transform.localScale;
        playerTwoResetScale = playerTwo.transform.localScale;

        playerOneStartDirection = playerOne.isFacingRight;
        playerTwoStartDirection = playerTwo.isFacingRight;

        ApplyRandomBallStartVariation();

        EventText.gameObject.SetActive(false);

        ball.SetSimulated(false);

        StartCoroutine(WaitForEventTextAnimation(EventTextAnimation, "Kickoff!", StartPlay));
    }

    void ApplyRandomBallStartVariation()
    {
        float randomX = Random.Range(ballRandomVariationX.x, ballRandomVariationX.y);
        float randomY = Random.Range(ballRandomVariationY.x, ballRandomVariationY.y);

        ball.transform.position += new Vector3(randomX, randomY);

        ball.SetSimulated(false);
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

    void StartPlay()
    {
        timerIsRunning = true;
        ball.SetSimulated(true);
    }

    void StopPlay()
    {
        timerIsRunning = false;
        ball.SetSimulated(false);
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

    private IEnumerator WaitForParticleEffect(ParticleSystem particleEffect, Action argOnComplete)
    {
        goalScored.Play();
        yield return new WaitForSeconds(particleEffect.main.duration);
        argOnComplete.Invoke();
    }

    private IEnumerator WaitForEventTextAnimation(Animation animationClip, string argText, Action argOnComplete)
    {

        EventText.gameObject.SetActive(true);
        EventText.text = argText;
        animationClip.Play();

        yield return new WaitForSeconds(animationClip.clip.length);
        argOnComplete.Invoke();
        EventText.gameObject.SetActive(false);
    }

    private void ResetGamePositions()
    {
        ball.transform.position = ballResetPoint;
        playerOne.transform.position = playerOneResetPoint;
        playerTwo.transform.position = playerTwoResetPoint;

        playerOne.transform.localScale = playerOneResetScale;
        playerTwo.transform.localScale = playerTwoResetScale;

        playerOne.isFacingRight = playerOneStartDirection;
        playerTwo.isFacingRight = playerTwoStartDirection;

        ApplyRandomBallStartVariation();
    }

    void GoalScored(int playerNumber)
    {
        StopPlay();

        if (playerNumber == 1)
        {
            playerOneScore++;
        }
        else if (playerNumber == 2)
        {
            playerTwoScore++;
        }

        SetScoreTexts(playerOneScore.ToString(), playerTwoScore.ToString());

        StartCoroutine(WaitForParticleEffect(goalScored, ResetGamePositions));


        string goalText = "Goal";

        StartCoroutine(WaitForEventTextAnimation(EventTextAnimation, goalText, StartPlay));
    }

    void SetScoreTexts(string argPlayerOneScoreText, string argPlayerTwoScoreText)
    {
        playerOneScoreText.text = argPlayerOneScoreText;
        playerTwoScoreText.text = argPlayerTwoScoreText;
    }

    void TimerEnded()
    {
        string whoWonText = "Tie Game";
        
        if(playerOneScore > playerTwoScore)
        {
            whoWonText = "Player One Wins";
        }
        else if (playerTwoScore > playerOneScore)
        {
            whoWonText = "Player Two Wins";
        }

        DisplayTime(0);


        StartCoroutine(WaitForEventTextAnimation(EventTextAnimation, whoWonText, RestartGame));
    }

    void RestartGame()
    {
        timeRemaining = startTime;

        playerOneScore = 0;
        playerTwoScore = 0;

        ResetGamePositions();
        StartCoroutine(WaitForEventTextAnimation(EventTextAnimation, "Kickoff", StartPlay));
    }

}
