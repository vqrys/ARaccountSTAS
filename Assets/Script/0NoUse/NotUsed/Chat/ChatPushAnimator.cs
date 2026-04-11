using System.Collections;
using UnityEngine;

public class ChatPushAnimator : MonoBehaviour
{
    public float pushDuration = 0.25f;

    RectTransform rect;
    float lastHeight;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        lastHeight = rect.rect.height;
    }

    public void AnimatePush()
    {
        StopAllCoroutines();
        StartCoroutine(PushRoutine());
    }

    IEnumerator PushRoutine()
    {
        // ⛔ WAJIB tunggu 1 frame supaya layout selesai
        yield return null;
        yield return new WaitForEndOfFrame();

        float newHeight = rect.rect.height;
        float delta = newHeight - lastHeight;

        // Kalau chat pertama → tidak perlu push
        if (delta <= 1f)
        {
            lastHeight = newHeight;
            yield break;
        }

        Vector2 originalPos = rect.anchoredPosition;
        rect.anchoredPosition = originalPos - new Vector2(0, delta);

        float t = 0;
        while (t < pushDuration)
        {
            t += Time.deltaTime;
            rect.anchoredPosition = Vector2.Lerp(
                originalPos - new Vector2(0, delta),
                originalPos,
                t / pushDuration
            );
            yield return null;
        }

        rect.anchoredPosition = originalPos;
        lastHeight = newHeight;
    }
}
