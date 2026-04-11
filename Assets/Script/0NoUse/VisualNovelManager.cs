using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VisualNovelManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject bgVN;
    public TMP_Text characterNameText;
    public TMP_Text dialogueText;
    public Button nextButton;
    public Button backButton;
    public Button cameraButton;

    [Header("Camera Button Visual")]
    public CanvasGroup cameraButtonGroup;

    private GameHandler gameHandler;
    private int currentIndex = 0;
    private List<string> dialogues;

    void Start()
    {
        characterNameText.text = "Mentor";

        nextButton.onClick.AddListener(NextDialogue);
        backButton.onClick.AddListener(PreviousDialogue);

        gameHandler = FindObjectOfType<GameHandler>();

        // 🔒 Lock camera button at start
        SetCameraButton(false);

        InitializeDialogues();
        ShowDialogue();
    }

    void InitializeDialogues()
    {
        string savedName = "Player";

    if (gameHandler != null)
    {
        string loadedName = gameHandler.LoadPlayerName();
        if (!string.IsNullOrEmpty(loadedName))
            savedName = loadedName;
    }


        dialogues = new List<string>
        {
            // "Halo " + savedName + ", selamat datang ke Akademi ARaccount.", USE IT WHEN STARTUI CHANGE SCENE
            "Halo, selamat datang ke Akademi ARaccount.",
            "Disini aku sebagai Mentor akan memandu mu cara menggunakan aplikasi ini.",
            "Aplikasi ARaccount dirancang untuk membantumu memahami dasar akuntansi.",
            "Kita akan belajar menggunakan teknologi Augmented Reality.",
            "Jika kamu siap, mari kita mulai.",

            "Selanjutnya untuk menyelesaikan misi utama mu.",
            "Yaitu mengerjakan Quest 1 Pengenalan Akuntansi.",
            "Kamu harus menekan Icon Camera terlebih dahulu untuk memulai Quest 1."
        };
    }

    void ShowDialogue()
    {
        dialogueText.text = dialogues[currentIndex];

        backButton.interactable = currentIndex > 0;
        nextButton.interactable = currentIndex < dialogues.Count - 1;

        HandleDialogueEvents();
    }

    void HandleDialogueEvents()
    {
        // 🔓 Unlock camera after index 4
        SetCameraButton(currentIndex >= 4);

        if (currentIndex == 4)
        {
            // FindObjectOfType<MissionManager>()?.CompleteObjective(0);
        }

        if (currentIndex == 7)
        {
            // FindObjectOfType<MissionManager>()?.CompleteObjective(1);
        }
    }

    void SetCameraButton(bool unlocked)
    {
        cameraButton.interactable = unlocked;

        if (cameraButtonGroup != null)
        {
            cameraButtonGroup.alpha = unlocked ? 1f : 0.4f;
            cameraButtonGroup.blocksRaycasts = unlocked;
        }
    }

    public void NextDialogue()
    {
        if (currentIndex < dialogues.Count - 1)
        {
            currentIndex++;
            ShowDialogue();
        }
        else
        {
            SaveTutorialProgress();
        }
    }

    public void PreviousDialogue()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            ShowDialogue();
        }
    }

    void SaveTutorialProgress()
    {
        if (gameHandler == null) return;

        gameHandler.SaveTutorialFinished();
        bgVN.SetActive(false);

        Debug.Log("Tutorial finished & saved!");
    }
}
