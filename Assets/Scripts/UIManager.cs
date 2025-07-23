using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Word UI Settings")]
    public Transform wordUIContainer;
    public GameObject letterUIPrefab;
    private Dictionary<string, Vector3> letterToWorldPositionMap = new();
    private List<string> wordLetters = new();         // Stores the expected letter strings
    private List<Image> letterImages = new();         // Corresponding UI images

    [Header("Health Bar Settings")]
    public Image[] hearts; // UI images for each heart
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Header("Game Over Settings")]
    public GameObject gameOverPanel;
    public GameObject restartButton;
    public GameObject gameOverImage;
    public PlayerLogic player;

    PlayerLogic PlayerLogic;
    void Awake()
    {
        Debug.Log("UIManager Awake called");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
    }

    public Dictionary<string, Vector3> GetLetterWorldPositionDictionary()
    {
        return new Dictionary<string, Vector3>(letterToWorldPositionMap);
    }

    public void AddLetterToWordUI(LetterDataSO letterData)
    {
        string letter = letterData.letterName.ToUpper();
        wordLetters.Add(letter);

        GameObject letterUI = Instantiate(letterUIPrefab, wordUIContainer);
        Image letterImage = letterUI.GetComponent<Image>();
        letterImage.sprite = letterData.upperCase;
        letterImage.color = Color.gray;

        RectTransform rectTransform = letterUI.GetComponent<RectTransform>();
        Vector2 position = new Vector2((wordLetters.Count - 1) * 100, 0);
        rectTransform.anchoredPosition = position;

        letterImages.Add(letterImage);

        // Get the world position of this UI element
        Vector3 worldPosition = rectTransform.position;
        letterToWorldPositionMap[letter] = worldPosition;
    }

    public void RevealLetter(string letter)
    {
        letter = letter.ToUpper();
        int index = wordLetters.IndexOf(letter);
        if (index >= 0 && index < letterImages.Count)
        {
            Image img = letterImages[index];
            if (img.color == Color.gray)
            {
                img.color = Color.white;
                wordLetters.RemoveAt(index);
                letterImages.RemoveAt(index);
                letterToWorldPositionMap.Remove(letter);
            }
        }
    }

    public void UpdateHearts(int lives)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < lives)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;
        }
        PlayerLogic.Instance.playerLives = lives;
    }

    public void ClearWordUI()
    {
        foreach (Transform child in wordUIContainer)
        {
            Destroy(child.gameObject);
        }
        wordLetters.Clear();
        letterImages.Clear();
        letterToWorldPositionMap.Clear();
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        restartButton.SetActive(true);
        gameOverImage.SetActive(true);
        player.enabled = false;
    }

    public void RestartGame()
    {
        // Reset player position and enable controls
        if (player != null)
        {
            player.enabled = true;
        }

        // Hide game over UI
        gameOverPanel.SetActive(false);
        restartButton.SetActive(false);
        gameOverImage.SetActive(false);

        // Reset hearts
        PlayerLogic.Instance.playerLives = 3;
        UpdateHearts(3);

        // Clear existing word UI
        ClearWordUI();

        // Restart current level using PlayerProgress info
        GameManager.Instance.StartGame();
    }
}
