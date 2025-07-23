using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{


    [Header("Level System")]
    public LevelDatabase levelDatabase;
    private LevelData currentLevel;


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
        SetLevel();
    }



    public void SetLevel()
    {
        int levelIndex = PlayerProgress.Instance.GetCurrentLevel();
        int wordIndex = PlayerProgress.Instance.GetCurrentWord();
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
        int level = PlayerProgress.Instance.GetCurrentLevel();
        int word = PlayerProgress.Instance.GetCurrentWord();
        word++;
        yield return new WaitForSeconds(0.7f); // Add a 1-second delay

        LevelData currentLevel = levelDatabase.GetLevel(level);
        PlayerProgress.Instance.SetCurrentWord(word);
        if (word >= currentLevel.wordData.Count)
        {
            level++;
            PlayerProgress.Instance.SetCurrentLevel(level);
            PlayerProgress.Instance.SetCurrentWord(0);
            if (level >= levelDatabase.levels.Count)
            {
                Debug.Log("No more levels!");
                // Trigger end screen or game finish state
                yield break;
            }
        }

        Player.transform.position = startingLocation.position;
        playerLogic.ResetLives();
        PlayerProgress.Instance.SaveProgress();
        SetLevel();
    }
}