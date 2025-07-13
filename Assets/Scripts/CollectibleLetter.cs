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
                StartCoroutine(WrongLetterFeedback());
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
    private IEnumerator WrongLetterFeedback()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color originalColor = sr.color;

        // Change color to red
        sr.color = Color.red;

        // Shake effect (slight random movement)
        float duration = 0.3f;
        float elapsed = 0f;
        float shakeMagnitude = 0.05f;
        Vector3 originalPos = transform.position;

        while (elapsed < duration)
        {
            float x = Random.Range(-shakeMagnitude, shakeMagnitude);
            float y = Random.Range(-shakeMagnitude, shakeMagnitude);
            transform.position = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;
        sr.color = originalColor;

        yield return new WaitForSeconds(0.2f); // small delay before disappearing

        Destroy(gameObject);
    }

}
