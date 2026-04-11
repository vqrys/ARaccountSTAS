using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChatItemUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text chatText;
    public CanvasGroup canvasGroup;
    public RectTransform rectTransform;
    public Image backgroundImage;

    [Header("Quiz Button")]
    public Button quizButton;

    [Header("Typewriter")]
    public float typingSpeed = 0.03f;

    [Header("Animation")]
    public float animDuration = 0.25f;

    void Awake()
    {
        chatText.text = "";
        canvasGroup.alpha = 0f;
        rectTransform.localScale = new Vector3(0.85f, 0.85f, 1f);

        if (quizButton != null)
            quizButton.gameObject.SetActive(false);
    }

    public void PlayChat(string message, System.Action onFinish)
    {
        StartCoroutine(ChatRoutine(message, onFinish));
    }

    public void ShowQuizButton(System.Action onClick)
    {
        if (quizButton == null) return;

        quizButton.gameObject.SetActive(true);
        quizButton.onClick.RemoveAllListeners();
        quizButton.onClick.AddListener(() => onClick?.Invoke());
    }

    public void SetFlatBackground(Sprite flatSprite)
    {
        if (backgroundImage != null && flatSprite != null)
            backgroundImage.sprite = flatSprite;
    }

    IEnumerator ChatRoutine(string message, System.Action onFinish)
    {
        // Animasi muncul
        float t = 0;
        while (t < animDuration)
        {
            t += Time.deltaTime;
            float lerp = t / animDuration;

            canvasGroup.alpha = Mathf.Lerp(0, 1, lerp);
            rectTransform.localScale = Vector3.Lerp(
                new Vector3(0.85f, 0.85f, 1f),
                Vector3.one,
                lerp
            );
            yield return null;
        }

        // Typewriter (aman untuk <b>)
        chatText.text = message;
        chatText.ForceMeshUpdate(); // 🔥 INI YANG KURANG

        chatText.maxVisibleCharacters = 0;

        int totalChars = chatText.textInfo.characterCount;
        while (chatText.maxVisibleCharacters < totalChars)
        {
            chatText.maxVisibleCharacters++;
            yield return new WaitForSeconds(typingSpeed);
        }

        onFinish?.Invoke();
    }
}
