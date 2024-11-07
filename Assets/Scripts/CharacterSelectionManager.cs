using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectionManager : MonoBehaviour
{
    public static CharacterSelectionManager Instance { get; private set; }

    [SerializeField] private List<CharacterData> availableCharacters;  // List of characters
    [SerializeField] private Image player1Display;  // UI display for Player 1
    [SerializeField] private Image player2Display;  // UI display for Player 2

    [SerializeField] private TextMeshProUGUI player1Text;  // UI display for Player 1
    [SerializeField] private TextMeshProUGUI player2Text;  // UI display for Player 2

    private int player1Index = 0;  // Track current character index for Player 1
    private int player2Index = 1;  // Track current character index for Player 2

    public CharacterData Player1Character { get; private set; }
    public CharacterData Player2Character { get; private set; }

    private void Awake()
    {
        // Implement Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        // Set initial characters for each player
        Player1Character = availableCharacters[player1Index];
        Player2Character = availableCharacters[player2Index];
        UpdateUI();
    }

    public void OnPlayer1Next()
    {
        player1Index = GetNextAvailableCharacter(player1Index, player2Index);
        Player1Character = availableCharacters[player1Index];
        UpdateUI();
    }

    public void OnPlayer1Previous()
    {
        player1Index = GetPreviousAvailableCharacter(player1Index, player2Index);
        Player1Character = availableCharacters[player1Index];
        UpdateUI();
    }

    public void OnPlayer2Next()
    {
        player2Index = GetNextAvailableCharacter(player2Index, player1Index);
        Player2Character = availableCharacters[player2Index];
        UpdateUI();
    }

    public void OnPlayer2Previous()
    {
        player2Index = GetPreviousAvailableCharacter(player2Index, player1Index);
        Player2Character = availableCharacters[player2Index];
        UpdateUI();
    }

    private int GetNextAvailableCharacter(int currentIndex, int otherPlayerIndex)
    {
        int nextIndex = (currentIndex + 1) % availableCharacters.Count;
        while (nextIndex == otherPlayerIndex)
        {
            nextIndex = (nextIndex + 1) % availableCharacters.Count;
        }
        return nextIndex;
    }

    private int GetPreviousAvailableCharacter(int currentIndex, int otherPlayerIndex)
    {
        int prevIndex = (currentIndex - 1 + availableCharacters.Count) % availableCharacters.Count;
        while (prevIndex == otherPlayerIndex)
        {
            prevIndex = (prevIndex - 1 + availableCharacters.Count) % availableCharacters.Count;
        }
        return prevIndex;
    }

    private void UpdateUI()
    {
        // Update UI elements with selected character data
        player1Display.sprite = Player1Character.headSprite;
        player2Display.sprite = Player2Character.headSprite;

        player1Text.color = Player1Character.shoeColor;
        player2Text.color = Player2Character.shoeColor;

        player1Text.text = Player1Character.name;
        player2Text.text = Player2Character.name;

        SoundManager.Instance.PlayButtonSound();
    }
}
