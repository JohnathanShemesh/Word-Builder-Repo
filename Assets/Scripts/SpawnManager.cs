using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    public GameObject letterPrefab; // Prefab של האות עם סקריפט CollectibleLetter
    public Transform[] spawnPoints; // תצטרך להכניס את הנקודות האלה באינספקטור

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SpawnLetters(List<LetterDataSO> lettersToSpawn)
    {
        ClearPreviousLetters();
        List<Transform> availableSpots = GetAllLetterSpawnPoints();

        foreach (LetterDataSO letter in lettersToSpawn)
        {
            if (availableSpots.Count == 0)
            {
                Debug.LogWarning("No available spawn points left!");
                return;
            }

            // בוחר מקום רנדומלי
            int index = Random.Range(0, availableSpots.Count);
            Transform spawnPoint = availableSpots[index];
            availableSpots.RemoveAt(index);

            // יוצר את האות
            GameObject letterGO = Instantiate(letterPrefab, spawnPoint.position, Quaternion.identity);
            CollectibleLetter collectible = letterGO.GetComponent<CollectibleLetter>();
            collectible.letterData = letter; // מזין את הנתונים לתוך האובייקט
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
