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

    public OutOfBounds outOfBoundsOne;
    public OutOfBounds outOfBoundsTwo;

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

    public Image P1Arrow;
    public Image P2Arrow;

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

    CharacterData P1Character;
    CharacterData P2Character;


    private void SetUpGameForPlayers()
    {
        // Set up players and game field
        P1Character = CharacterSelectionManager.Instance.Player1Character;
        P2Character = CharacterSelectionManager.Instance.Player2Character;

        playerOne.characterData = P1Character;
        playerTwo.characterData = P2Character;

        playerOne.headCollider.GetComponent<SpriteRenderer>().sprite = P1Character.headSprite;
        playerTwo.headCollider.GetComponent<SpriteRenderer>().sprite = P2Character.headSprite;

        playerOne.headCollider = P1Character.collider;
        playerTwo.headCollider = P2Character.collider;

        playerOne.cleatsSprite.color = P1Character.shoeColor;
        playerTwo.cleatsSprite.color = P2Character.shoeColor;

        playerTwo.cleatsSprite.color = P2Character.shoeColor;
        playerTwo.cleatsSprite.color = P2Character.shoeColor;

        playerOneScoreText.color = P1Character.mainColor;
        playerTwoScoreText.color = P2Character.mainColor;

        playerOne.cooldownImage.color = P1Character.mainColor;
        playerTwo.cooldownImage.color = P2Character.mainColor;

        P1Arrow.color = P1Character.mainColor;
        P2Arrow.color = P2Character.mainColor;

        playerOne.faceSprite.sprite = P1Character.face;
        playerTwo.faceSprite.sprite = P2Character.face;
    }


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


    void Start()
    {
        timeRemaining = startTime;

        StopPlay();

        if(CharacterSelectionManager.Instance != null)
        {
            SetUpGameForPlayers();
        }

        playerOneGoal.SetGoalScoredAction(GoalScored);
        playerTwoGoal.SetGoalScoredAction(GoalScored);

        outOfBoundsOne.SetOutOfBoundsAction(OutOfBoundsTriggered);
        outOfBoundsTwo.SetOutOfBoundsAction(OutOfBoundsTriggered);

        ballResetPoint = ball.transform.position;
        playerOneResetPoint = playerOne.transform.position;
        playerTwoResetPoint = playerTwo.transform.position;

        playerOneResetScale = playerOne.transform.localScale;
        playerTwoResetScale = playerTwo.transform.localScale;

        playerOneStartDirection = playerOne.isFacingRight;
        playerTwoStartDirection = playerTwo.isFacingRight;

        ApplyRandomBallStartVariation();

        EventText.gameObject.SetActive(false);

        StartCoroutine(WaitForEventTextAnimation(EventTextAnimation, "Kickoff!", StartPlay));
    }

    void ApplyRandomBallStartVariation()
    {
        float randomX = Random.Range(ballRandomVariationX.x, ballRandomVariationX.y);

        float randomY = Random.Range(ballRandomVariationY.x, ballRandomVariationY.y);

        ball.transform.position += new Vector3(randomX, randomY);
    }


    private float soundTimer;

    void Update()
    {
        if (timerIsRunning)
        {
            // update the player timers when the game is running
            playerOne.UpdateTimers();
            playerTwo.UpdateTimers();

            if (timeRemaining > 0)
            {
                if (timeRemaining <= 5.0f)
                {
                    // Play countdown sound every second
                    soundTimer += Time.deltaTime;
                    float timeBetweenSounds = 1f;
                    if (soundTimer >= timeBetweenSounds)
                    {
                        SoundManager.Instance.PlayCountdownSound(); // Use your SoundManager to play sound
                        soundTimer = 0f; // Reset sound timer for the next second
                    }
                }

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
        playerOne.SetAllowedMovement(true);
        playerTwo.SetAllowedMovement(true);
    }

    void StopPlay()
    {
        timerIsRunning = false;
        ball.SetSimulated(false);
        playerOne.SetAllowedMovement(false);
        playerTwo.SetAllowedMovement(false);
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

    private void ResetPositionsForGoalScored()
    {
        ball.transform.position = ballResetPoint;
        playerOne.transform.position = playerOneResetPoint;
        playerTwo.transform.position = playerTwoResetPoint;

        playerOne.transform.localScale = playerOneResetScale;
        playerTwo.transform.localScale = playerTwoResetScale;

        playerOne.isFacingRight = playerOneStartDirection;
        playerTwo.isFacingRight = playerTwoStartDirection;

        playerOne.faceSprite.sprite = playerOne.characterData.face;
        playerTwo.faceSprite.sprite = playerTwo.characterData.face;

        ApplyRandomBallStartVariation();
    }

    void OutOfBoundsTriggered(int outOfBoundsNumber)
    {
        StopPlay();

        // Display "Out of Bounds" event text
        StartCoroutine(WaitForEventTextAnimation(EventTextAnimation, "Out of Bounds", () =>
        {
            // Reset positions based on which out-of-bounds triggered
            if (outOfBoundsNumber == 1)
            {
                ResetPositionsForOutofBounds(outOfBoundsOne.resetPositionOne, outOfBoundsOne.resetPositionTwo, outOfBoundsOne.ballResetPosition);
            }
            else if (outOfBoundsNumber == 2)
            {
                ResetPositionsForOutofBounds(outOfBoundsTwo.resetPositionOne, outOfBoundsTwo.resetPositionTwo, outOfBoundsTwo.ballResetPosition);
            }

            StartPlay();
        }));
    }

    void ResetPositionsForOutofBounds(Vector3 positionOne, Vector3 positionTwo, Vector3 ballPosition)
    {
        // Reset ball and player positions based on specified positions
        ball.transform.position = ballPosition;
        playerOne.transform.position = positionOne;
        playerTwo.transform.position = positionTwo;

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
            playerOne.faceSprite.sprite = playerOne.characterData.scoredFace;
            playerTwo.faceSprite.sprite = playerTwo.characterData.stunnedFace;
        }
        else if (playerNumber == 2)
        {
            playerTwoScore++;
            playerTwo.faceSprite.sprite = playerTwo.characterData.scoredFace;
            playerOne.faceSprite.sprite = playerOne.characterData.stunnedFace;
        }

        SetScoreTexts(playerOneScore.ToString(), playerTwoScore.ToString());

        StartCoroutine(WaitForParticleEffect(goalScored, ResetPositionsForGoalScored));


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
        DisplayTime(0);

        StopPlay();

        SoundManager.Instance.PlayTimerEndSound();

        string whoWonText = "Tie Game";
        
        if(playerOneScore > playerTwoScore)
        {
            whoWonText = P1Character.name + " Wins";
            EventText.color = P1Character.mainColor;
        }
        else if (playerTwoScore > playerOneScore)
        {
            whoWonText = P2Character.name + " Wins";
            EventText.color = P2Character.mainColor;
        }

        StartCoroutine(WaitForEventTextAnimation(EventTextAnimation, whoWonText, RestartGame));
    }

    void RestartGame()
    {
        timeRemaining = startTime;

        playerOneScore = 0;
        playerTwoScore = 0;

        SetScoreTexts(playerOneScore.ToString(), playerTwoScore.ToString());

        ResetPositionsForGoalScored();

        UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
    }

}
