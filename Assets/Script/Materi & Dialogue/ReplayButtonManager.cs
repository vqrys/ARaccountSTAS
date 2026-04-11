using UnityEngine;
using UnityEngine.UI;

public class ReplayButtonManager : MonoBehaviour
{
    [Header("Referensi UI")]
    [Tooltip("Masukkan tombol Replay yang memiliki komponen CanvasGroup")]
    public CanvasGroup replayButtonCanvasGroup;
    public Button replayButton;

    [Header("Referensi NPC")]
    [Tooltip("Masukkan objek Raccoon yang memiliki script RaccoonSpeaker")]
    public RaccoonSpeaker raccoonSpeaker;

    private void Start()
    {
        // Kondisi awal: Tombol disembunyikan secara instan saat game dimulai
        HideReplayButton();

        // Mendaftarkan event saat tombol diklik
        if (replayButton != null)
        {
            replayButton.onClick.RemoveAllListeners();
            replayButton.onClick.AddListener(OnReplayClicked);
        }
    }

    // Dipanggil dari Event OnSpeechEnd milik Raccoon
    public void ShowReplayButton()
    {
        if (replayButtonCanvasGroup != null)
        {
            replayButtonCanvasGroup.alpha = 1f;
            replayButtonCanvasGroup.interactable = true;
            replayButtonCanvasGroup.blocksRaycasts = true;
        }
    }

    public void HideReplayButton()
    {
        if (replayButtonCanvasGroup != null)
        {
            replayButtonCanvasGroup.alpha = 0f;
            replayButtonCanvasGroup.interactable = false;
            replayButtonCanvasGroup.blocksRaycasts = false;
        }
    }

    private void OnReplayClicked()
    {
        // 1. Sembunyikan tombol
        HideReplayButton();

        // 2. Panggil fungsi Replay dari RaccoonSpeaker
        if (raccoonSpeaker != null)
        {
            raccoonSpeaker.ReplayLastSpeech();
        }
        else
        {
            Debug.LogWarning("[ReplayButton] RaccoonSpeaker belum dimasukkan ke inspector!");
        }
    }
} 