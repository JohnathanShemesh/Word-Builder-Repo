using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel; // assign in inspector (Panel with black bg + button)
    public GameObject restartButton;  // הכפתור שמתחיל מחדש
    public GameObject gameOverImage;
    public Movement player;
    void Start()
    {
        gameOverPanel.SetActive(false);
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        restartButton.SetActive(true);
        gameOverImage.SetActive(true);
        player.enabled = false;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
