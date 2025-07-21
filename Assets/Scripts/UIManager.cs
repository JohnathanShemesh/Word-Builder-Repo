using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Word UI Settings")]
    public Transform wordUIContainer;
    public GameObject letterUIPrefab;
    private Dictionary<string, Vector3> letterToWorldPositionMap = new();
    private List<string> wordLetters = new();         // Stores the expected letter strings
    private List<Image> letterImages = new();         // Corresponding UI images

    [Header("Health Bar Settings")]
    public Healthbar healthbar;

    void Awake()
    {
        Debug.Log("UIManager Awake called");
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

    public Dictionary<string, Vector3> GetLetterWorldPositionDictionary()
    {
        return new Dictionary<string, Vector3>(letterToWorldPositionMap);
    }

    public void AddLetterToWordUI(LetterDataSO letterData)
    {
        string letter = letterData.letterName.ToUpper();
        wordLetters.Add(letter);

        GameObject letterUI = Instantiate(letterUIPrefab, wordUIContainer);
        Image letterImage = letterUI.GetComponent<Image>();
        letterImage.sprite = letterData.upperCase;
        letterImage.color = Color.gray;

        RectTransform rectTransform = letterUI.GetComponent<RectTransform>();
        Vector2 position = new Vector2((wordLetters.Count - 1) * 100, 0);
        rectTransform.anchoredPosition = position;

        letterImages.Add(letterImage);

        // Get the world position of this UI element
        Vector3 worldPosition = rectTransform.position;
        letterToWorldPositionMap[letter] = worldPosition;
    }

    public void RevealLetter(string letter)
    {
        letter = letter.ToUpper();
        int index = wordLetters.IndexOf(letter);
        if (index >= 0 && index < letterImages.Count)
        {
            Image img = letterImages[index];
            if (img.color == Color.gray)
            {
                img.color = Color.white;
                wordLetters.RemoveAt(index);
                letterImages.RemoveAt(index);
                letterToWorldPositionMap.Remove(letter);
            }
        }
    }

    public void UpdateHearts(int lives)
    {
        healthbar.UpdateHearts(lives);
        GameManager.Instance.playerLives = lives;
    }

    public void ClearWordUI()
    {
        foreach (Transform child in wordUIContainer)
        {
            Destroy(child.gameObject);
        }
        wordLetters.Clear();
        letterImages.Clear();
        letterToWorldPositionMap.Clear();
    }
}
