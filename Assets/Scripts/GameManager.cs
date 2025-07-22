using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Player Stats")]
    public int playerLives = 3;

    [Header("Level System")]
    public LevelDatabase levelDatabase;
    private LevelData currentLevel;
    public int currentLevelIndex = 0;
    public int currentWordIndex = 0;

    [Header("Letter Spawn Settings")]
    public GameObject letterPrefab;
    public Transform startingLocation;
    public GameObject Player;
    public LetterDataBaseSO lettersDataBaseRef;

    private List<string> currentWordLetters = new();  // New: List to track remaining letters

    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        PlayerProgress playerProgress = PlayerProgress.Instance;
         Player.transform.position = startingLocation.position;
        Debug.Log("Game started!");
        startingLocation.transform.position = Player.transform.position;
        currentLevelIndex = playerProgress.GetCurrentLevel();
        currentWordIndex = playerProgress.GetCurrentWord();
        SetLevel(playerProgress.GetCurrentLevel(), playerProgress.GetCurrentWord());
    }

    public void LoseLife()
    {
        playerLives--;
        UIManager.Instance.UpdateHearts(playerLives);
        if (playerLives <= 0)
        {
            Debug.Log("Game Over");
            UIManager.Instance.ShowGameOver();
        }
    }

    public void SetLevel(int levelIndex, int wordIndex)
    {
        PlayerProgress playerProgress = PlayerProgress.Instance;
        playerProgress.SetProgress(levelIndex, wordIndex);

        LevelData level = levelDatabase.GetLevel(levelIndex);

        if (wordIndex < 0 || wordIndex >= level.wordData.Count)
        {
            Debug.LogWarning("Invalid word index");
            return;
        }

        WordData currentWordData = level.wordData[wordIndex];
        string word = currentWordData.wordName.ToUpper();
        InitializeWordLetters(word);

        List<LetterDataSO> lettersToSpawn = new();
        foreach (char c in word)
        {
            LetterDataSO letterSO = lettersDataBaseRef.GetLetter(c.ToString());
            if (letterSO != null)
                lettersToSpawn.Add(letterSO);
            else
                Debug.LogWarning($"Letter '{c}' not found in letter database");
        }

        SpawnManager.Instance.SpawnLetters(currentWordData);
    }
    //makes a list of strings out of the current word
    public void InitializeWordLetters(string word)
    {
        currentWordLetters = new List<string>();
        foreach (char c in word)
        {
            currentWordLetters.Add(c.ToString().ToUpper());
        }

        Debug.Log("Initialized word letters:");
        foreach (var letter in currentWordLetters)
        {
            Debug.Log(letter);
        }
    }

    public bool CollectLetterSprite(LetterDataSO collectedLetter)
    {
        string letterName = collectedLetter.letterName.ToUpper().Trim();

        Debug.Log($"Trying to collect letter: '{letterName}'");

        if (!currentWordLetters.Contains(letterName))
        {
            Debug.LogWarning($"Letter '{letterName}' is NOT in the remaining word letters!");
            LoseLife();
            return false;
        }

        // Remove the first matching instance of the letter
        currentWordLetters.Remove(letterName);
        UIManager.Instance.RevealLetter(letterName);
        Debug.Log($"Collected letter '{letterName}'. Remaining: {currentWordLetters.Count}");

        if (currentWordLetters.Count == 0)
        {
            Debug.Log("Word completed!");
            OnWordCompleted();
        }

        return true;
    }

    private void OnWordCompleted()//add updates to player progress
    {
        StartCoroutine(OnWordCompletedRoutine());
    }

    private IEnumerator OnWordCompletedRoutine()
    {
        yield return new WaitForSeconds(0.7f); // Add a 1-second delay

        currentWordIndex++;
        LevelData currentLevel = levelDatabase.GetLevel(currentLevelIndex);

        if (currentWordIndex >= currentLevel.wordData.Count)
        {
            currentLevelIndex++;
            currentWordIndex = 0;

            if (currentLevelIndex >= levelDatabase.levels.Count)
            {
                Debug.Log("No more levels!");
                // Trigger end screen or game finish state
                yield break;
            }
        }

        Player.transform.position = startingLocation.position;
        UIManager.Instance.UpdateHearts(3);
        SetLevel(currentLevelIndex, currentWordIndex);
    }
}