using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public Image[] hearts; // UI images for each heart
    public Sprite fullHeart;
    public Sprite emptyHeart;

    public void UpdateHearts(int lives)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < lives)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;
        }
    }
}
