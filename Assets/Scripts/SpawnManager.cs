using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Unity.VisualScripting.Antlr3.Runtime;
public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;
    public GameObject letterPrefab; 
    public Transform[] spawnPoints; 
    public LetterDataBaseSO allLetters;
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

    public void SpawnLetters(WordData word)
    {
        ClearPreviousLetters();
        List<Transform> availableSpots = GetAllLetterSpawnPoints();
        List<LetterDataSO> lettersToSpawn = new(); // Collect actual letters to spawn

        // Step 1: Get the correct LetterDataSO objects from the word string
        foreach (char c in word.wordName)
        {
            LetterDataSO matchingLetter = allLetters.letterSOs
                .FirstOrDefault(l => l.letterName.Equals(c.ToString(), StringComparison.OrdinalIgnoreCase));

            if (matchingLetter != null)
            {
                if (availableSpots.Count == 0)
                {
                    Debug.LogWarning("No available spawn points left!");
                    return;
                }

                int index = UnityEngine.Random.Range(0, availableSpots.Count);
                Transform spawnPoint = availableSpots[index];
                availableSpots.RemoveAt(index);

                GameObject letterGO = Instantiate(letterPrefab, spawnPoint.position, Quaternion.identity);
                CollectibleLetter collectible = letterGO.GetComponent<CollectibleLetter>();
                collectible.letterData = matchingLetter;
                lettersToSpawn.Add(matchingLetter);

                UIManager.Instance.AddLetterToWordUI(matchingLetter);
            }
            else
            {
                Debug.LogWarning($"Letter '{c}' not found in LetterDatabase!");
            }
        }        
        
        // Step 2: Spawn fake letters
        SpawnFakeLetters(word.fakeLettersToSpawn, lettersToSpawn, availableSpots);
    }

    private void SpawnFakeLetters(int wrongLetterAmountToSpawn, List<LetterDataSO> correctLetters, List<Transform> availableSpots)
    {
        var wrongLetters = allLetters.letterSOs
            .Where(l => !correctLetters.Contains(l))
            .ToList();

        if (wrongLetters.Count < wrongLetterAmountToSpawn)
        {
            Debug.LogWarning("Not enough fake letters available in the database.");
            wrongLetterAmountToSpawn = wrongLetters.Count;
        }

        for (int i = 0; i < wrongLetterAmountToSpawn; i++)
        {
            if (availableSpots.Count == 0)
            {
                Debug.LogWarning("No available spawn points left for fake letters!");
                return;
            }

            int spotIndex = UnityEngine.Random.Range(0, availableSpots.Count);
            Transform spawnPoint = availableSpots[spotIndex];
            availableSpots.RemoveAt(spotIndex);

            int letterIndex = UnityEngine.Random.Range(0, wrongLetters.Count);
            LetterDataSO fakeLetter = wrongLetters[letterIndex];
            wrongLetters.RemoveAt(letterIndex);

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
        if (existingLetters.Length == 0)
        {
            Debug.Log("array empty");
            return;
        }
        foreach (GameObject obj in existingLetters)
        {
            Destroy(obj);
        }
    }
}
