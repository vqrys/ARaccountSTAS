using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonAnimation : MonoBehaviour
{
    public Button targetButton;
    public float scaleUp = 1.2f;
    public float duration = 0.15f;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = targetButton.transform.localScale;

        targetButton.onClick.AddListener(PlayAnimation);
    }

    void PlayAnimation()
    {
        targetButton.transform
            .DOScale(originalScale * scaleUp, duration)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                targetButton.transform
                    .DOScale(originalScale, duration)
                    .SetEase(Ease.InOutSine);
            });
    }
}