using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SmoothScrollRect : MonoBehaviour
{
    public float scrollDuration = 0.25f;

    ScrollRect scrollRect;
    Coroutine scrollRoutine;

    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    public void ScrollToBottomSmooth()
    {
        if (scrollRoutine != null)
            StopCoroutine(scrollRoutine);

        scrollRoutine = StartCoroutine(ScrollRoutine());
    }

    IEnumerator ScrollRoutine()
    {
        float start = scrollRect.verticalNormalizedPosition;
        float target = 0f; // bottom

        float t = 0;
        while (t < scrollDuration)
        {
            t += Time.deltaTime;
            scrollRect.verticalNormalizedPosition =
                Mathf.Lerp(start, target, t / scrollDuration);
            yield return null;
        }

        scrollRect.verticalNormalizedPosition = target;
    }
}
