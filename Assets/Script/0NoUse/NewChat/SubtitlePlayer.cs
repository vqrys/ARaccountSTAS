using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Unity.VisualScripting;

public class SubtitlePlayer : MonoBehaviour
{
    [Header("References")]
    public SubtitleTrack track;
    public AudioSource audioSource;

    [Header("UI")]
    public GameObject subtitlePanel;
    public GameObject Animated;
    public GameObject Raccoon;
    public TMP_Text subtitleText;
    public Button speechButton;
    public Button replayButton;
    public Button quizButton;

    [Header("Quiz")]
    public QuizManager quizManager;

    [Header("Animation Triggers")]
    [SerializeField] private List<AnimatorTriggerCaller> animatorTriggers;
    [SerializeField] private List<string> triggerNames;

    [Header("Replay Button Animation")]
    public RectTransform replayButtonRect;
    public float replayAnimDuration = 0.5f;
    public float replayStartX = 171f;
    public float replayEndX = 132f;

    Coroutine routine;
    int cueIndex;
    List<SubtitleCue> runtimeCues;

    void Awake()
    {
        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.Stop();
        }
    }

    void Start()
    {
        if (Animated != null) Animated.SetActive(false);
        if (subtitlePanel != null) subtitlePanel.SetActive(false);
        if (quizButton != null) quizButton.gameObject.SetActive(false);
        if (replayButton != null) replayButton.gameObject.SetActive(false);

        speechButton.onClick.RemoveAllListeners();
        speechButton.onClick.AddListener(PlaySpeech);

        replayButton.onClick.RemoveAllListeners();
        replayButton.onClick.AddListener(ReplaySpeech);

        quizButton.onClick.RemoveAllListeners();
        quizButton.onClick.AddListener(() =>
        {
            if (quizManager != null) quizManager.StartQuiz();
        });
    }

    public void PlaySpeech()
    {
        UnshowReplayButton();

        if (track == null || track.voiceClip == null)
        {
            Debug.LogError("SubtitleTrack atau voiceClip belum di-assign.");
            return;
        }
        if (audioSource == null)
        {
            Debug.LogError("AudioSource belum di-assign.");
            return;
        }
        if (track.cues == null || track.cues.Count == 0)
        {
            Debug.LogError("Cue subtitle kosong.");
            return;
        }

        runtimeCues = new List<SubtitleCue>(track.cues);
        runtimeCues.Sort((a, b) => a.time.CompareTo(b.time));

        cueIndex = 0;
        quizButton.gameObject.SetActive(false);
        replayButton.gameObject.SetActive(true);

        subtitlePanel.SetActive(true);
        subtitleText.text = "";

        if (Animated != null) Animated.SetActive(true);
        if (Raccoon != null) Raccoon.SetActive(true);

        if (animatorTriggers != null && animatorTriggers.Count > 0)
        {
            for (int i = 0; i < animatorTriggers.Count; i++)
            {
                animatorTriggers[i].SetTriggerByName(triggerNames[i]);
            }
        }

        audioSource.Stop();
        audioSource.clip = track.voiceClip;
        audioSource.time = 0f;
        audioSource.Play();

        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(RunSubtitles());
    }

    public void ReplaySpeech()
    {
        PlaySpeech();
    }

    public void OnQuizEnd()
    {
        Debug.Log("OnQuizEnd Triggered");
        speechButton.gameObject.SetActive(true);
        quizButton.gameObject.SetActive(false);
        replayButton.gameObject.SetActive(false);
        Animated.gameObject.SetActive(false);
    }

    IEnumerator RunSubtitles()
    {
        while (audioSource.isPlaying)
        {
            float t = audioSource.time;

            if (cueIndex < runtimeCues.Count && t >= runtimeCues[cueIndex].time)
            {
                subtitleText.text = runtimeCues[cueIndex].text;
                cueIndex++;
            }

            yield return null;
        }

        EndSpeech();
    }

    void EndSpeech()
    {
        subtitlePanel.SetActive(false);
        quizButton.gameObject.SetActive(true);

        if (animatorTriggers != null && animatorTriggers.Count > 0)
        {
            foreach (var trigger in animatorTriggers)
            {
                trigger.SetTriggerByName("TrRNormal");
                trigger.SetTriggerByName("TRQ12Stay");
            }
        }

        ShowReplayButton();
    }

    void ShowReplayButton()
    {
        if (replayButton == null || replayButtonRect == null) return;

        replayButton.gameObject.SetActive(true);

        Vector2 currentPos = replayButtonRect.anchoredPosition;
        replayButtonRect.anchoredPosition = new Vector2(replayStartX, currentPos.y);

        replayButtonRect
            .DOAnchorPosX(replayEndX, replayAnimDuration)
            .SetEase(Ease.OutCubic);
    }

    void UnshowReplayButton()
    {
        if (replayButton == null || replayButtonRect == null) return;

        // Kill animasi sebelumnya biar tidak tabrakan
        replayButtonRect.DOKill();

        Vector2 currentPos = replayButtonRect.anchoredPosition;

        replayButtonRect
            .DOAnchorPosX(replayStartX, replayAnimDuration)
            .SetEase(Ease.InCubic)
            .OnComplete(() =>
            {
                replayButton.gameObject.SetActive(false);
            });
    }
}