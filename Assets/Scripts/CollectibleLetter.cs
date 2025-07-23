using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleLetter : MonoBehaviour
{
    public LetterDataSO letterData;
    private bool collected = false;
    private Dictionary<string, Vector3> letterToWorldPositionMap = new();
    private PlayerLogic playerLogic;

    private void Start()
    {
        if (letterData != null)
            GetComponent<SpriteRenderer>().sprite = letterData.upperCase;
        else
            Debug.LogWarning("No letterData assigned to collectible letter!");
        letterToWorldPositionMap = UIManager.Instance.GetLetterWorldPositionDictionary();
        playerLogic = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLogic>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(letterData.letterName);
        if (!collected && other.CompareTag("Player"))
        {
            collected = true;

            bool isCorrect = playerLogic.CollectLetterSprite(letterData);

            if (isCorrect)
                StartCoroutine(MoveToWordImage());
            else
                StartCoroutine(WrongLetterFeedback());
        }
    }

    private IEnumerator MoveToWordImage()
    {
        if (!letterToWorldPositionMap.TryGetValue(letterData.letterName, out Vector3 targetScreenPos))
        {
            Debug.LogWarning($"No position found for letter: {letterData.letterName}");
            Destroy(gameObject);
            yield break;
        }

        // Convert screen position to world position
        Vector3 end = Camera.main.ScreenToWorldPoint(new Vector3(targetScreenPos.x, targetScreenPos.y, Camera.main.nearClipPlane));
        end.z = 0f;

        Vector3 start = transform.position;
        float time = 0, duration = 0.2f;

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
