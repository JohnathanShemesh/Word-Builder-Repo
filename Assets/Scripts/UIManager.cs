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

    public void CreateWordUI(List<LetterDataSO> lettersToSpawn)
    {
        Debug.Log("Creating word UI for: " + lettersToSpawn.Count + " letters");

        for (int i = 0; i < lettersToSpawn.Count; i++)
        {
            GameObject letterUI = Instantiate(letterUIPrefab, wordUIContainer);

            // Get the Image component and set the sprite
            Image letterImage = letterUI.GetComponent<Image>();
            letterImage.sprite = lettersToSpawn[i].upperCase;

            // Set horizontal position to spread them out
            RectTransform rectTransform = letterUI.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(i * 100, 0); // 100 pixels apart

            Debug.Log($"Created UI for letter: {lettersToSpawn[i].letterName}");
        }
    }
}
