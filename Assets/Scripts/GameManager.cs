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
    public int currentLevelIndex = 0;
    private LevelData currentLevel;
    public Healthbar healthBar;
    public GameOverManager gameOverManager;
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
        LoadLevel(currentLevelIndex);
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
    //gets the index of the level from the leveldatabase and loads it.
    private void LoadLevel(int index)
    {
        Player.transform.position = startingLocation.transform.position;
        successImage.enabled = false;

        if (levelDatabase == null || levelDatabase.levels.Count == 0)
        {
            Debug.LogWarning("No level list defined!");
            return;
        }

        if (index < 0 || index >= levelDatabase.levels.Count)
        {
            Debug.LogWarning("Invalid level index!");
            return;
        }

        currentLevel = levelDatabase.levels[index];
        currentWord = currentLevel.wordData;

        if (currentDisplayedWord != null && currentWord.wordSprite != null)
        {
            currentDisplayedWord.sprite = currentWord.wordSprite;
        }
        else
        {
            Debug.LogWarning("Missing reference or word sprite");
        }

        StartNewLevel(currentWord);
    }
    //spawns a certain amount of letters from a list of the remaining letters at the remaining available spawn points
    public void SpawnWrongLetters(LevelData leveldata, List<Transform> availableSpawns)
    {
        int wrongLettersCount = leveldata.fakeLettersToSpawn;
        List<Sprite> wrongLetters = new List<Sprite>(alphabet.letterSprites);
        wrongLetters.RemoveAll(letter => currentWord.letterSprites.Contains(letter));

        List<Sprite> chosenWrongLetters = new List<Sprite>();

        for (int i = 0; i < wrongLettersCount; i++)
        {
            if (wrongLetters.Count == 0)
            {
                Debug.LogWarning("No more fake letters available to choose from");
                break;
            }

            int wordLocation = UnityEngine.Random.Range(0, wrongLetters.Count);
            chosenWrongLetters.Add(wrongLetters[wordLocation]);
            wrongLetters.RemoveAt(wordLocation);
        }

        foreach (Sprite wrongLetter in chosenWrongLetters)
        {
            if (availableSpawns.Count == 0)
            {
                Debug.LogWarning("No remaining spawn points for fake letters");
                break;
            }

            int spawnIndex = UnityEngine.Random.Range(0, availableSpawns.Count);
            Transform spawn = availableSpawns[spawnIndex];
            availableSpawns.RemoveAt(spawnIndex);

            GameObject letterObj = Instantiate(letterPrefab, spawn.position, Quaternion.identity);
            var letter = letterObj.GetComponent<CollectibleLetter>();
            letter.letterSprite = wrongLetter;
        }
    }

    private List<Sprite> collectedSprites = new();
    //gets the new level's word and removes the remaining letters fron the previous level. then calls a method to start the next level
    public void StartNewLevel(WordData word)
    {
        GameObject[] existingLetters = GameObject.FindGameObjectsWithTag("Letter");
        foreach (GameObject letter in existingLetters)
        {
            Destroy(letter);
        }

        Debug.Log("Removed previous letters: " + existingLetters.Length);

        Debug.Log("Starting new level with: " + word.wordName);
        currentWord = word;
        collectedSprites.Clear();
        SpawnAllLetters(currentLevel);
        Debug.Log("Selected new word: " + currentWord.wordName);
    }
    //gets the level's data and spawns the wrong and correct letters into the level
    public void SpawnAllLetters(LevelData levelData)
    {
        List<Transform> allPoints = GetAllLetterSpawnPoints();
        List<Transform> remainingPoints = SpawnCorrectLetters(allPoints);
        SpawnWrongLetters(levelData, remainingPoints);
    }
    //returns a list of all available spawn points in the level
    public List<Transform> GetAllLetterSpawnPoints()
    {
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
        List<Transform> spawnPoints = new();

        foreach (GameObject platform in platforms)
        {
            Transform spawnPoint = platform.transform.Find("LetterSpawnPoint");
            if (spawnPoint != null)
            {
                spawnPoints.Add(spawnPoint);
            }
        }

        return spawnPoints;
    }
    //spawns all the correct letters into the level and returns a list of the remaining available spawn points.
    public List<Transform> SpawnCorrectLetters(List<Transform> spawnPoints)
    {
        List<Transform> availableSpawns = new(spawnPoints);

        for (int i = 0; i < currentWord.letterSprites.Count; i++)
        {
            if (availableSpawns.Count == 0)
            {
                Debug.LogWarning("No remaining spawn points for correct letters");
                break;
            }

            int index = Random.Range(0, availableSpawns.Count);
            Transform spawn = availableSpawns[index];
            availableSpawns.RemoveAt(index);

            GameObject letterObj = Instantiate(letterPrefab, spawn.position, Quaternion.identity);
            var letter = letterObj.GetComponent<CollectibleLetter>();
            letter.letterSprite = currentWord.letterSprites[i];
        }

        return availableSpawns;
    }
    //gets the letter the player collected and decides if its the correct or wring letter and what to do in each case
    public bool CollectLetterSprite(Sprite letterSprite)
    {
        if (currentWord == null)
            return false;

        if (currentWord.letterSprites.Contains(letterSprite))
        {
            if (!collectedSprites.Contains(letterSprite))
            {
                collectedSprites.Add(letterSprite);
                Debug.Log("Collected: " + letterSprite.name);
                if (IsWordComplete())
                {
                    Debug.Log("Word completed: " + currentWord.wordName);
                    FinishedLevel(1.5f);
                }
            }
            return true;
        }
        else
        {
            LoseLife();           
            Debug.Log("Wrong letter collected: " + letterSprite.name);
            return false;
        }
    }
   

    // checks is the whole word was collected and returns a bool
    private bool IsWordComplete()
    {
        foreach (var sprite in currentWord.letterSprites)
        {
            if (!collectedSprites.Contains(sprite))
                return false;
        }
        return true;
    }
    //initiates the sequence of moving to the next level
    public void FinishedLevel(float delay)
    {
        successImage.enabled = true;
        if (collectedSprites != null)
        {
            collectedSprites.Clear();
        }

        currentLevelIndex++;
        if (currentLevelIndex >= levelDatabase.levels.Count)
        {
            Debug.Log("No more levels");
        }
        else
        {
            StartCoroutine(LoadNextLevelAfterDelay(delay));
        }
    }

    private IEnumerator LoadNextLevelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadLevel(currentLevelIndex);
    }
}
