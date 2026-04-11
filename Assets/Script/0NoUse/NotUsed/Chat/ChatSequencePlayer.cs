using System.Collections;
using UnityEngine;

public class ChatSequencePlayer : MonoBehaviour
{
    public ChatUIManager chatUI;
    public ChatPushAnimator pushAnimator;
    public SmoothScrollRect smoothScroll;
    public QuizManager quizManager;
    public GameObject startQuizButton;


    [TextArea(2, 4)]
    public string[] chatLines;

    [Header("Timing")]
    public float typingSpeed = 0.03f;
    public float animDuration = 0.25f;
    public float extraDelay = 0.5f;

    bool isPlaying = false;

    // Dipanggil oleh Button
    public void PlayChatSequence()
    {
        if (isPlaying) return;
        StartCoroutine(PlaySequence());
    }

    public void OnStartQuizClicked()
    {
        Debug.Log("Tombol Quiz Ditekan");
        // Sembunyikan tombol
        if (startQuizButton != null)
            startQuizButton.SetActive(false);

        // Ambil bubble terakhir
        ChatItemUI lastBubble = chatUI.GetLastChat();

        // Ubah background jadi flat
        if (lastBubble != null && chatUI.flatBubbleSprite != null)
        {
            lastBubble.SetFlatBackground(chatUI.flatBubbleSprite);
        }

        // Dorong chat ke atas
        pushAnimator?.AnimatePush();
        smoothScroll?.ScrollToBottomSmooth();

        // Tunda sedikit biar animasi terlihat natural
        StartCoroutine(StartQuizAfterDelay(0.3f));
    }

    IEnumerator PlaySequence()
    {
        isPlaying = true;

        foreach (string line in chatLines)
        {
            chatUI.AddChat(line);

            pushAnimator?.AnimatePush();
            smoothScroll?.ScrollToBottomSmooth();

            float typingTime = line.Length * typingSpeed;
            float totalWait = animDuration + typingTime + extraDelay;

            yield return new WaitForSeconds(totalWait);
        }

        isPlaying = false;

        // 🔥 TAMPILKAN TOMBOL QUIZ (BUKAN LANGSUNG START)
        if (startQuizButton != null)
            startQuizButton.SetActive(true);
    }

    IEnumerator StartQuizAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (quizManager != null)
            quizManager.StartQuiz();
    }


}
