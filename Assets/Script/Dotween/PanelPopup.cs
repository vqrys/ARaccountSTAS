using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PanelPopup : MonoBehaviour
{
    [Header("Popup")]
    public GameObject gameObjectPanel;
    public RectTransform rectPanel;
    public Vector2 hiddenPosition = new Vector2(0, -1000);
    public Vector2 shownPosition = new Vector2(0, 0);

    [Header("Overlay")]
    public CanvasGroup overlayCanvasGroup;

    [Header("Animation")]
    public float duration = 0.35f;
    public Ease openEase = Ease.OutBack;
    public Ease closeEase = Ease.InBack;

    private bool isOpen = false;

    void Start()
    {
        rectPanel.anchoredPosition = hiddenPosition;

        overlayCanvasGroup.alpha = 0f;
        overlayCanvasGroup.interactable = false;
        overlayCanvasGroup.blocksRaycasts = false;

        gameObjectPanel.SetActive(false);
    }

    public void OpenPopup()
    {
        if (isOpen) return;

        gameObjectPanel.SetActive(true);

        rectPanel.DOKill();
        overlayCanvasGroup.DOKill();

        overlayCanvasGroup.interactable = true;
        overlayCanvasGroup.blocksRaycasts = true;

        Sequence seq = DOTween.Sequence();
        seq.Join(overlayCanvasGroup.DOFade(1f, duration));
        seq.Join(rectPanel.DOAnchorPos(shownPosition, duration).SetEase(openEase));

        isOpen = true;
    }

    public void ClosePopup()
    {
        if (!isOpen) return;

        rectPanel.DOKill();
        overlayCanvasGroup.DOKill();

        Sequence seq = DOTween.Sequence();
        seq.Join(overlayCanvasGroup.DOFade(0f, duration));
        seq.Join(rectPanel.DOAnchorPos(hiddenPosition, duration).SetEase(closeEase));

        seq.OnComplete(() =>
        {
            overlayCanvasGroup.interactable = false;
            overlayCanvasGroup.blocksRaycasts = false;
            gameObjectPanel.SetActive(false);
        });

        isOpen = false;
    }
}