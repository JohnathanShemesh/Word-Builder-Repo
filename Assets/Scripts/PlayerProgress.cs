using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
    public static PlayerProgress Instance;

    private int currentLevel;
    private int currentWord;

    private const string LevelKey = "CurrentLevel";
    private const string WordKey = "CurrentWord";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadProgress(); // Load saved values
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public int GetCurrentLevel() => currentLevel;
    public int GetCurrentWord() => currentWord;

    public void SetProgress(int level, int word)
    {
        currentLevel = level;
        currentWord = word;

        PlayerPrefs.SetInt(LevelKey, currentLevel);
        PlayerPrefs.SetInt(WordKey, currentWord);
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        currentLevel = PlayerPrefs.GetInt(LevelKey, 0);
        currentWord = PlayerPrefs.GetInt(WordKey, 0);
        Debug.Log($"[PlayerProgress] Loaded progress: Level {currentLevel}, Word {currentWord}");
    }

    public void ResetProgress()
    {
        currentLevel = 0;
        currentWord = 0;
        PlayerPrefs.SetInt(LevelKey, currentLevel);
        PlayerPrefs.SetInt(WordKey, currentWord);
        PlayerPrefs.Save();
        Debug.Log("[PlayerProgress] Progress reset to 0");
    }

    [ContextMenu("Reset Progress")]
    private void ResetProgressFromInspector()
    {
        ResetProgress();
    }
}
