using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    public GameObject letterPrefab; // Prefab של האות עם סקריפט CollectibleLetter
    public Transform[] spawnPoints; // תצטרך להכניס את הנקודות האלה באינספקטור

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

    public void SpawnLetters(List<LetterDataSO> lettersToSpawn, LetterDataBaseSO allLetters, LevelData currentLevel)
    {
        ClearPreviousLetters();
        List<Transform> availableSpots = GetAllLetterSpawnPoints();

        // Spawn correct letters
        foreach (LetterDataSO letter in lettersToSpawn)
        {
            if (availableSpots.Count == 0)
            {
                Debug.LogWarning("No available spawn points left!");
                return;
            }

            int index = Random.Range(0, availableSpots.Count);
            Transform spawnPoint = availableSpots[index];
            availableSpots.RemoveAt(index);

            GameObject letterGO = Instantiate(letterPrefab, spawnPoint.position, Quaternion.identity);
            CollectibleLetter collectible = letterGO.GetComponent<CollectibleLetter>();
            collectible.letterData = letter;
        }

        //  Now spawn fake (wrong) letters
        int wrongLetterAmountToSpawn = currentLevel.fakeLettersToSpawn;

        // 1. Filter all letters that are NOT part of the current word
        var wrongLetters = allLetters.letterSOs.Where(l => !lettersToSpawn.Contains(l))
            .ToList();

        // Safety check
        if (wrongLetters.Count < wrongLetterAmountToSpawn)
        {
            Debug.LogWarning("Not enough fake letters available in the database.");
            wrongLetterAmountToSpawn = wrongLetters.Count;
        }

        // 2. Shuffle the list and take only the number we need
        for (int i = 0; i < wrongLetterAmountToSpawn; i++)
        {
            if (availableSpots.Count == 0)
            {
                Debug.LogWarning("No available spawn points left for fake letters!");
                return;
            }

            int spotIndex = Random.Range(0, availableSpots.Count);
            Transform spawnPoint = availableSpots[spotIndex];
            availableSpots.RemoveAt(spotIndex);

            // Pick a random wrong letter from the list
            int letterIndex = Random.Range(0, wrongLetters.Count);
            LetterDataSO fakeLetter = wrongLetters[letterIndex];
            wrongLetters.RemoveAt(letterIndex); // prevent duplicates

            GameObject letterGO = Instantiate(letterPrefab, spawnPoint.position, Quaternion.identity);
            CollectibleLetter collectible = letterGO.GetComponent<CollectibleLetter>();
            collectible.letterData = fakeLetter;
        }
    }

    //gets the index of the level from the leveldatabase and loads it.    
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

    private void ClearPreviousLetters()
    {
        GameObject[] existingLetters = GameObject.FindGameObjectsWithTag("Letter");
        foreach (GameObject obj in existingLetters)
        {
            Destroy(obj);
        }
    }
}
