using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Player Stats")]
    public int playerLives = 3;
    public Text livesText;

    [Header("Level System")]
    public LevelDatabase levelDatabase;
    private LevelData currentLevel;
    public Healthbar healthBar;
    public GameOverManager gameOverManager;
    public int currentLevelIndex = 0;
    public int currentWordIndex = 0;
    [Header("Word Management")]
    public List<WordData> availableWords; // Predefined word list
    public WordData currentWord;
    public Alphabet alphabet;
    public GameObject currentWordUi;

    [Header("Letter Spawn Settings")]
    public GameObject letterPrefab;           // Letter prefab
    public Transform[] letterSpawnPoints;     // Letter spawn points
    public Transform[] usedLetterSpawnPoints;
    public Image currentDisplayedWord;        // Linked through Inspector
    public Image successImage;
    public Transform startingLocation;
    public GameObject Player;

    [SerializeField] LetterDataBaseSO lettersDataBase;//new
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
        FindObjectOfType<Healthbar>().UpdateHearts(playerLives);
        if (playerLives <= 0)
        {
            Debug.Log("Game Over");
            gameOverManager.ShowGameOver();
            currentWordUi.SetActive(false);
        }        
    }
    public bool CollectLetter(LetterDataSO letterdata)
    {
        return true;
    }
    public void GetNewLevel(int levelIndex, int wordIndex)
    {
        LevelData level = levelDatabase.GetLevel(levelIndex);

        if (wordIndex < 0 || wordIndex >= level.wordData.Count)
        {
            Debug.LogWarning("Invalid word index");
            return;
        }

        WordData currentWordData = level.wordData[wordIndex];
        string word = currentWordData.wordName;

        List<LetterDataSO> lettersToSpawn = new List<LetterDataSO>();

        foreach (char c in word)
        {
            LetterDataSO letterSO = lettersDataBase.GetLetter(c.ToString());
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
        SpawnManager.Instance.SpawnLetters(lettersToSpawn);
    }
    void OnWordCompleted()
    {
        currentWordIndex++;
        LevelData currentLevel = levelDatabase.GetLevel(currentLevelIndex);

        if (currentWordIndex >= currentLevel.wordData.Count)
        {
            // עברנו את כל המילים בשלב, עוברים לשלב הבא
            currentLevelIndex++;
            currentWordIndex = 0;

            if (currentLevelIndex >= levelDatabase.levels.Count)
            {
                Debug.Log("No more levels!");
                // אפשר להציג מסך סיום או משהו
                return;
            }
        }

        GetNewLevel(currentLevelIndex, currentWordIndex);
    }

   
    //spawns all the correct letters into the level and returns a list of the remaining available spawn points.
    
    public bool CollectLetterSprite(Sprite letterSprite)
    {
        return true;
        //create method that is responsible for the player collection of a letter. if its the wrong letter decreace life
    }
   

      
}
