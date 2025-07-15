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
    private Dictionary<string, List<Image>> letterUIImages = new();
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
    //gets a list of letters that we need to spawn and makes new object that has the transform of the word container and the letterprefab
    //makes the letters gray and spawns them into the word container
    public void CreateWordUI(List<LetterDataSO> lettersToSpawn)
    {
        letterUIImages.Clear(); // Clear previous references
        Debug.Log("Creating word UI for: " + lettersToSpawn.Count + " letters");

        for (int i = 0; i < lettersToSpawn.Count; i++)
        {
            GameObject letterUI = Instantiate(letterUIPrefab, wordUIContainer);

            Image letterImage = letterUI.GetComponent<Image>();
            letterImage.sprite = lettersToSpawn[i].upperCase;
            letterImage.color = Color.gray; // Grayed out initially

            RectTransform rectTransform = letterUI.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(i * 100, 0);

            string letter = lettersToSpawn[i].letterName.ToUpper();

            if (!letterUIImages.ContainsKey(letter))
                letterUIImages[letter] = new List<Image>();

            letterUIImages[letter].Add(letterImage); // Store the image
        }
    }
    public void RevealLetter(string letter)
    {
        letter = letter.ToUpper();

        if (letterUIImages.ContainsKey(letter))
        {
            foreach (Image img in letterUIImages[letter])
            {
                if (img.color == Color.gray)
                {
                    img.color = Color.white; // Reveal the letter
                    break; // Only reveal one instance at a time
                }
            }
        }
    }


    public void UpdateHearts(int lives)
    {
       healthbar.UpdateHearts(lives);
    }
}
