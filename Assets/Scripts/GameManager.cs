using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{


    [Header("Level System")]
    public LevelDatabase levelDatabase;
    private LevelData currentLevel;
    public int currentLevelIndex = 0;
    public int currentWordIndex = 0;

    [Header("Letter Spawn Settings")]
   
    public LetterDataBaseSO lettersDataBaseRef;
 [Header("Player Info")]
    private PlayerLogic playerLogic;
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
        playerLogic = Player.GetComponent<PlayerLogic>();
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
        playerLogic.InitializeWordLetters(word);

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

    public void OnWordCompleted()//add updates to player progress
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
        playerLogic.ResetLives();
        SetLevel(currentLevelIndex, currentWordIndex);
    }
}