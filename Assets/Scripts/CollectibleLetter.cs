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

    //checks that the player touched the letter and destroys the object of the letter
    private bool collected = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !collected)
        {
            collected = true;

            bool isCorrect = GameManager.Instance.CollectLetterSprite(letterSprite);

            if (isCorrect)
            {
                StartCoroutine(MoveToWordImage());
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }


    private IEnumerator MoveToWordImage()
    {
        Transform targetTransform = GameManager.Instance.currentDisplayedWord.transform;
        Vector3 start = transform.position;
        Vector3 screenPoint = targetTransform.position;
        Vector3 end = Camera.main.ScreenToWorldPoint(screenPoint);
        end.z = 0;
        float duration = 0.5f;
        float time = 0f;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(start, end, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = end;

        Destroy(gameObject);
    }

}
