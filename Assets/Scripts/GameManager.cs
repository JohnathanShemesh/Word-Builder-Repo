
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Level System")]
    public LevelDatabase levelDatabase;
    public int currentLevelIndex = 0;
    private LevelData currentLevel;
    [Header("Word Management")]
    public List<WordData> availableWords; // ����� ������ ����
    public WordData currentWord;
    public Alphabet alphabet;
    [Header("Letter Spawn Settings")]
    public GameObject letterPrefab;          // ������ �� ���
    public Transform[] letterSpawnPoints;    // ������ ����� ������
    public Transform[] usedLetterSpawnPoints;
    public Image currentDisplayedWord; // ����� ��� �-Inspector
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

   
    public void StartGame()
    {
        Debug.Log("����� �����!");
        startingLocation.transform.position = Player.transform.position;
        successImage.enabled = false;
        LoadLevel(currentLevelIndex);
    }

    private void LoadLevel(int index)
    {
        Player.transform.position = startingLocation.transform.position;
        successImage.enabled = false;
        if (levelDatabase == null || levelDatabase.levels.Count == 0)
        {
            Debug.LogWarning("�� ������ ����� �����!");
            return;
        }

        if (index < 0 || index >= levelDatabase.levels.Count)
        {
            Debug.LogWarning("������ ��� �� ����!");
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
            Debug.LogWarning("��� ����� ������ �� ���� ����� �����");
        }

        StartNewLevel(currentWord);
    }
    /*
    public void LoadRandomWord()
    {
        Debug.Log(" ������ ���� ��������");
        if (availableWords.Count == 0)
        {
            Debug.LogWarning(" ��� ����� ������!");
            return;
        }
        int index = Random.Range(0, availableWords.Count);
        StartNewLevel(availableWords[index]);
        if (currentDisplayedWord != null && currentWord.wordSprite != null)
        {
            currentDisplayedWord.sprite = currentWord.wordSprite;
        }
        else
        {
            Debug.LogWarning("��� ����� ������ �� ���� ����� �����");
        }
    }
    */
    private List<Sprite> collectedSprites = new();

    public void StartNewLevel(WordData word)
    {
        Debug.Log("������� ��� ��� ��: " + word.wordName);
        currentWord = word;
        collectedSprites.Clear();

        Debug.Log(" ���� ���� �����: " + currentWord.wordName);
        SpawnLetters();
        //SpawnFakeLetters();
    }

    public void SpawnLetters()
    {
        Debug.Log(" ������ ������ �����...");
        if (currentWord == null || letterPrefab == null)
        {
            Debug.LogWarning(" �� ���� ����� ������ � ����� ������");
            return;
        }

        // ���� �� �� ���������� ��� ���� "Platform"
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
        List<Transform> spawnPoints = new List<Transform>();

        // ���� �� ��������, ������ �� ���� ����� "LetterSpawnPoint"
        foreach (GameObject platform in platforms)
        {
            Transform spawnPoint = platform.transform.Find("LetterSpawnPoint");
            if (spawnPoint != null)
            {
                spawnPoints.Add(spawnPoint);
            }
        }

        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning(" �� ����� ������ Spawn");
            return;
        }

        // ����� ������� ��������� ���� ������
        List<Transform> availableSpawns = new(spawnPoints);

        for (int i = 0; i < currentWord.letterSprites.Count; i++)
        {
            if (availableSpawns.Count == 0)
            {
                Debug.LogWarning(" �� ����� ������ ������ �������");
                break;
            }

            int index = Random.Range(0, availableSpawns.Count);
            Transform spawn = availableSpawns[index];
            availableSpawns.RemoveAt(index);

            GameObject letterObj = Instantiate(letterPrefab, spawn.position, Quaternion.identity);
            var letter = letterObj.GetComponent<CollectibleLetter>();
            letter.letterSprite = currentWord.letterSprites[i];
        }
    }
    /*
    public void SpawnFakeLetters()
    {
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
        List<Transform> spawnPoints = new List<Transform>();

        // ���� �� ��������, ������ �� ���� ����� "LetterSpawnPoint"
        foreach (GameObject platform in platforms)
        {
            Transform spawnPoint = platform.transform.Find("LetterSpawnPoint");
            if (spawnPoint != null)
            {
                spawnPoints.Add(spawnPoint);
            }
        }

        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning(" �� ����� ������ Spawn");
            return;
        }

        // ����� ������� ��������� ���� ������
        List<Transform> availableSpawns = new(spawnPoints);

       List<Sprite> wrongLetters = new List<Sprite>(alphabet.letterSprites);
        wrongLetters.RemoveAll(letter => currentWord.letterSprites.Contains(letter));
        
        int openSpots = availableSpawns.Count;
        int index = Random.Range(0, availableSpawns.Count);
        while (openSpots > 0)
        {
            Transform spawn = availableSpawns[openSpots];
            GameObject letterObj = Instantiate(letterPrefab, spawn.position, Quaternion.identity);
            var letter = letterObj.GetComponent<CollectibleLetter>();
            
            openSpots--;
        }

    }
    */
    public void CollectLetterSprite(Sprite letterSprite)
    {
        if (currentWord == null)
            return;

        if (currentWord.letterSprites.Contains(letterSprite))
        {
            if (!collectedSprites.Contains(letterSprite))
            {
                collectedSprites.Add(letterSprite);
                Debug.Log(" ����: " + letterSprite.name);

                if (IsWordComplete())
                {
                    Debug.Log(" ����� �� �����: " + currentWord.wordName);
                    FinishedLevel(1.5f);
                    // ����� ���� ����� ��� ���� ��� �� ����� UI
                }
            }
        }
        else
        {
            Debug.Log(" ��� �� ��� �����: " + letterSprite.name);
        }
    }
   
    private bool IsWordComplete()
    {
        foreach (var sprite in currentWord.letterSprites)
        {
            if (!collectedSprites.Contains(sprite))
                return false;
        }
        return true;
    }
    public void FinishedLevel(float delay)
    {
        successImage.enabled = true;
        if(collectedSprites != null)
        {
            collectedSprites.Clear();
        }
        currentLevelIndex++;
        if(currentLevelIndex >= levelDatabase.levels.Count)
        {
            Debug.Log("no more levels");
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
