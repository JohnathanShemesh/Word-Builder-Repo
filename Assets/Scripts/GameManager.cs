using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Player Stats")]
    public int playerLives = 3;

    [Header("Level System")]
    public LevelDatabase levelDatabase;
    private LevelData currentLevel;
    public Healthbar healthBar;
    public GameOverManager gameOverManager;
    public int currentLevelIndex = 0;
    public int currentWordIndex = 0;
    [Header("Letter Spawn Settings")]
    public GameObject letterPrefab;           // Letter prefab
    public Transform[] letterSpawnPoints;     // Letter spawn points
    public Transform[] usedLetterSpawnPoints;
    public Image currentDisplayedWord;        // Linked through Inspector
    public Image successImage;
    public Transform startingLocation;
    public GameObject Player;
    public LetterDataBaseSO lettersDataBaseRef;//new
    //counters for collection method
    private Dictionary<string, int> requiredLetterCounts = new();
    private Dictionary<string, int> collectedLetterCounts = new();
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
    //makes a starting location for the player. 
    //disables the level complete image.
    //calls "load level" method that gets the level we want to start
    public void StartGame()
    {
        Debug.Log("Game started!");
        startingLocation.transform.position = Player.transform.position;
        successImage.enabled = false;
        GetNewLevel(currentLevelIndex,currentWordIndex);
    }
    //decreasing healthpoints when a player takes the wrong letter. also checks to see if the player lost all healthpoints in which case it called the game over method
    public void LoseLife()
    {
        playerLives--;
        UIManager.Instance.UpdateHearts(playerLives);
        if (playerLives <= 0)
        {
            Debug.Log("Game Over");
            gameOverManager.ShowGameOver();
        }        
    }

    public void GetNewLevel(int levelIndex, int wordIndex)
    {
        LevelData level = levelDatabase.GetLevel(levelIndex);
        LetterDataBaseSO letterDataBaseSO = lettersDataBaseRef;
        if (wordIndex < 0 || wordIndex >= level.wordData.Count)
        {
            Debug.LogWarning("Invalid word index");
            return;
        }

        WordData currentWordData = level.wordData[wordIndex];
        string word = currentWordData.wordName;
        GameManager.Instance.requiredLetterCounts = new Dictionary<string, int>();
        InitializeLetterCounts(word); // <-- Add this line!

        List<LetterDataSO> lettersToSpawn = new List<LetterDataSO>();

        foreach (char c in word)
        {
            LetterDataSO letterSO = lettersDataBaseRef.GetLetter(c.ToString());
            if (letterSO != null)
            {
                lettersToSpawn.Add(letterSO);
            }
            else
            {
                Debug.LogWarning($"Letter '{c}' not found in letter database");
            }
        }

        UIManager.Instance.CreateWordUI(lettersToSpawn);
        SpawnManager.Instance.SpawnLetters(currentWordData);
    }
    void OnWordCompleted()
    {
        currentWordIndex++;
        LevelData currentLevel = levelDatabase.GetLevel(currentLevelIndex);

        if (currentWordIndex >= currentLevel.wordData.Count)
        {
            currentLevelIndex++;
            currentWordIndex = 0;

            if (currentLevelIndex >= levelDatabase.levels.Count)
            {
                Debug.Log("No more levels!");
                //add finished game screen
                return;
            }
        }
        Player.transform.position = startingLocation.position;
        UIManager.Instance.UpdateHearts(3);
        GetNewLevel(currentLevelIndex, currentWordIndex);
    }


    public bool CollectLetterSprite(LetterDataSO collectedLetter)
    {
        string letterName = collectedLetter.letterName.ToUpper().Trim();

        Debug.Log($"Trying to collect letter: '{letterName}'");
        Debug.Log("Required letters:");
        foreach (var key in requiredLetterCounts.Keys)
        {
            Debug.Log(key);
        }
        if (!requiredLetterCounts.ContainsKey(letterName.ToUpper()))
        {
            Debug.LogWarning($"Letter '{letterName}' is NOT in the required letters!");
            LoseLife();
            return false;
        }

        if (!collectedLetterCounts.ContainsKey(letterName))
        {
            collectedLetterCounts[letterName] = 1;
        }
        else
        {
            collectedLetterCounts[letterName]++;
        }
        UIManager.Instance.RevealLetter(letterName.ToUpper());
        Debug.Log($"Collected '{letterName}' count: {collectedLetterCounts[letterName]} / {requiredLetterCounts[letterName]}");

        bool wordCompleted = true;

        foreach (var kvp in requiredLetterCounts)
        {
            string requiredLetter = kvp.Key;
            int requiredAmount = kvp.Value;

            int collectedAmount = collectedLetterCounts.ContainsKey(requiredLetter) ? collectedLetterCounts[requiredLetter] : 0;

            if (collectedAmount < requiredAmount)
            {
                wordCompleted = false;
                break;
            }
        }

        if (wordCompleted)
        {
            Debug.Log("Word completed!");
            OnWordCompleted();
        }

        return true;
    }

    public void InitializeLetterCounts(string word)
    {
        requiredLetterCounts.Clear();
        collectedLetterCounts.Clear();
        foreach (char c in word)
        {
            string letter = c.ToString().ToUpper();
            if (requiredLetterCounts.ContainsKey(letter))
                requiredLetterCounts[letter]++;
            else
                requiredLetterCounts[letter] = 1;
        }
        PrintRequiredLetterCounts();
    }
    public void PrintRequiredLetterCounts()
    {
        Debug.Log("Required letters and counts:");
        foreach (var kvp in requiredLetterCounts)
        {
            Debug.Log($"Letter: {kvp.Key}, Count: {kvp.Value}");
        }
    }


}
