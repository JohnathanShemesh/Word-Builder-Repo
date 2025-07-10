
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    
    [Header("Word Management")]
    public List<WordData> availableWords; // מילים שיצרנו מראש
    public WordData currentWord;
    public Alphabet alphabet;
    [Header("Letter Spawn Settings")]
    public GameObject letterPrefab;          // פריפאב של אות
    public Transform[] letterSpawnPoints;    // נקודות להצבת אותיות
    public Transform[] usedLetterSpawnPoints;
    public Image currentDisplayedWord; // משויך דרך ה-Inspector
    public Image successImage;
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
        successImage.enabled = false;
    }

   
    public void StartGame()
    {
        Debug.Log("המשחק התחיל!");
        LoadRandomWord();
    }

    public void LoadRandomWord()
    {
        Debug.Log(" בוחרים מילה רנדומלית");
        if (availableWords.Count == 0)
        {
            Debug.LogWarning(" אין מילים זמינות!");
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
            Debug.LogWarning("חסר חיבור לתמונה או שאין תמונה למילה");
        }
    }
    private List<Sprite> collectedSprites = new();

    public void StartNewLevel(WordData word)
    {
        Debug.Log("מתחילים שלב חדש עם: " + word.wordName);
        currentWord = word;
        collectedSprites.Clear();

        Debug.Log(" מילה חדשה נבחרה: " + currentWord.wordName);
        SpawnLetters();
        //SpawnFakeLetters();
    }

    public void SpawnLetters()
    {
        Debug.Log(" מציבים אותיות בסצנה...");
        if (currentWord == null || letterPrefab == null)
        {
            Debug.LogWarning(" לא ניתן להציב אותיות – חסרים נתונים");
            return;
        }

        // מוצא את כל האובייקטים שעם תגית "Platform"
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
        List<Transform> spawnPoints = new List<Transform>();

        // מתוך כל פלטפורמה, מוצאים את הילד שנקרא "LetterSpawnPoint"
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
            Debug.LogWarning(" לא נמצאו נקודות Spawn");
            return;
        }

        // נשתמש במקומות רנדומליים מתוך הרשימה
        List<Transform> availableSpawns = new(spawnPoints);

        for (int i = 0; i < currentWord.letterSprites.Count; i++)
        {
            if (availableSpawns.Count == 0)
            {
                Debug.LogWarning(" לא נשארו נקודות פנויות לאותיות");
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

        // מתוך כל פלטפורמה, מוצאים את הילד שנקרא "LetterSpawnPoint"
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
            Debug.LogWarning(" לא נמצאו נקודות Spawn");
            return;
        }

        // נשתמש במקומות רנדומליים מתוך הרשימה
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
                Debug.Log(" אספת: " + letterSprite.name);

                if (IsWordComplete())
                {
                    Debug.Log(" השלמת את המילה: " + currentWord.wordName);
                    successImage.enabled = true;
                    // בהמשך נוכל לעשות כאן מעבר שלב או להציג UI
                }
            }
        }
        else
        {
            Debug.Log(" זאת לא אות נכונה: " + letterSprite.name);
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

}
