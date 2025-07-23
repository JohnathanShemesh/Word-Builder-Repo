using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
    public static PlayerProgress Instance;

    private int currentLevel;
    private int currentWord;

    private const string SaveKey = "PlayerSavedData";

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
        LoadProgress();
    }
    public int GetCurrentLevel() => currentLevel;
    public int GetCurrentWord() => currentWord;
    public int SetCurrentLevel(int level) => currentLevel = level;
    public int SetCurrentWord(int word) => currentWord = word;
    public void SaveProgress()
    {
        SaveData data = new SaveData
        {
            level = GetCurrentLevel(),
            word = GetCurrentWord()
        };
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
        Debug.Log("the progress is saved");
    }

    private void LoadProgress()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            string json = PlayerPrefs.GetString(SaveKey);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);
            if(saveData != null)
            {
                currentLevel = saveData.level;
                currentWord = saveData.word;
                Debug.Log($"[PlayerProgress] Loaded progress: Level {currentLevel}, Word {currentWord}");
            }
            else
            {
                Debug.Log("failed to load playerprefs save data");
            }
        }
        else
        {
            Debug.Log("no save found in player progress");
        }
        
    }

    public void ResetProgress()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            PlayerPrefs.DeleteKey(SaveKey);
            PlayerPrefs.Save();
            Debug.Log("save data is deleted from playerprefs");
        }
        else
        {
            Debug.Log("no save data found to delete");

        }
        
    }

    [ContextMenu("Reset Progress")]
    private void ResetProgressFromInspector()
    {
        ResetProgress();
    }
}
