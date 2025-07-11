using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectibleLetter : MonoBehaviour
{
    public Sprite letterSprite;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = letterSprite;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.CollectLetterSprite(letterSprite);
            Destroy(gameObject);
        }
    }

}
