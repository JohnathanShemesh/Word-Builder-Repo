using System.Collections;
using UnityEngine;

public class CollectibleLetter : MonoBehaviour
{
    public LetterDataSO letterData;
    private bool collected = false;

    private void Start()
    {
        if (letterData != null)
            GetComponent<SpriteRenderer>().sprite = letterData.upperCase;
        else
            Debug.LogWarning("No letterData assigned to collectible letter!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(letterData.letterName);
        if (!collected && other.CompareTag("Player"))
        {
            collected = true;

            bool isCorrect = GameManager.Instance.CollectLetterSprite(letterData);

            if (isCorrect)
                StartCoroutine(MoveToWordImage());
            else
                StartCoroutine(WrongLetterFeedback());
        }
    }

    private IEnumerator MoveToWordImage()
    {
        Transform target = UIManager.Instance.wordUIContainer.transform;
        Vector3 start = transform.position;
        Vector3 end = Camera.main.ScreenToWorldPoint(target.position);
        end.z = 0;
        float time = 0, duration = 0.5f;

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

        sr.color = Color.red;
        float duration = 0.3f, shake = 0.05f, time = 0;
        Vector3 originalPos = transform.position;

        while (time < duration)
        {
            transform.position = originalPos + Random.insideUnitSphere * shake;
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;
        sr.color = originalColor;
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
