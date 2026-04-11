using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

public class QuizManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject questionPanel;
    public TMP_Text questionText;

    public Transform answerContainer;
    public AnswerOptionButton answerPrefab;

    public Button actionButton;          
    public TMP_Text actionButtonText;    

    public GameObject UiIcons;

    [Header("Quiz Data")]
    public List<QuizQuestion> questions = new List<QuizQuestion>();

    int currentIndex = 0;
    int score = 0;

    int selectedIndex = -1;
    bool isChecked = false;

    [Header("Action Button Visual")]
    public Image actionButtonImage;
    public Color enabledColor = Color.white;
    public Color disabledColor = Color.gray;

    [Header("Score Popup")]
    public GameObject scorePopup;
    public NumberCounter scoreCounter;   // text yang ada component NumberCounter
    public TMP_Text scoreDescText;
    public Button scoreCloseButton;
    public BoothSubtitlePlayer onQuizEnd;

    List<AnswerOptionButton> spawned = new List<AnswerOptionButton>();

    void Awake()
    {
        if (UiIcons != null) UiIcons.SetActive(true);
        if (questionPanel != null) questionPanel.SetActive(false);
        if (actionButton != null) actionButton.gameObject.SetActive(false);

        if (scorePopup != null) scorePopup.SetActive(false);

        if (scoreCloseButton != null)
        {
            scoreCloseButton.onClick.RemoveAllListeners();
            scoreCloseButton.onClick.AddListener(() =>
            {
                if (scorePopup != null) scorePopup.SetActive(false);
            });
        }
    }

    public void StartQuiz()
    {   
        if (questions == null || questions.Count == 0)
        {
            Debug.LogError("QuizManager: questions kosong.");
            return;
        }
        if (questionPanel == null || questionText == null || answerContainer == null || answerPrefab == null || actionButton == null || actionButtonText == null)
        {
            Debug.LogError("QuizManager: UI reference belum lengkap.");
            return;
        }

        currentIndex = 0;
        score = 0;

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(OnActionButton);

        ShowCurrentQuestion();
    }

    void ShowCurrentQuestion()
    {
        UiIcons.SetActive(false);
        questionPanel.SetActive(true);
        actionButton.gameObject.SetActive(true);

        isChecked = false;
        selectedIndex = -1;

        SetActionButton("Periksa", false); // wajib pilih dulu

        ClearAnswers();

        QuizQuestion q = questions[currentIndex];
        questionText.text = q.question;

        for (int i = 0; i < q.answers.Length; i++)
        {
            int optionIndex = i;

            AnswerOptionButton opt = Instantiate(answerPrefab, answerContainer);
            opt.Init(optionIndex, q.answers[i], OnOptionSelected);
            opt.SetInteractable(true);

            spawned.Add(opt);
        }
    }

    void OnOptionSelected(int optionIndex)
    {
        if (isChecked) return;

        selectedIndex = optionIndex;

        // tampilkan selected animation sesuai animator flow kamu
        for (int i = 0; i < spawned.Count; i++)
        {
            if (i == selectedIndex) spawned[i].SetSelected();
            else spawned[i].SetNormal();
        }

        SetActionButton("Periksa", true); // tombol Periksa aktif setelah memilih jawaban
        Debug.Log("SelectedIndex: " + selectedIndex);
    }
    
    void OnActionButton()
    {
        if (!isChecked) CheckAnswer();
        else NextQuestion();
    }

    void CheckAnswer()
    {
        if (selectedIndex < 0)
        {
            Debug.LogWarning("Belum pilih jawaban.");
            return;
        }

        isChecked = true;

        QuizQuestion q = questions[currentIndex];
        bool isCorrect = selectedIndex == q.correctIndex;

        // lock pilihan setelah diperiksa
        foreach (var opt in spawned) opt.SetInteractable(false);

        if (!isCorrect)
        {
            spawned[selectedIndex].SetWrong();
            spawned[q.correctIndex].SetCorrect();
        }
        else
        {
            score++;
            spawned[selectedIndex].SetCorrect(); // ✅ ini yang kamu kurang
        }

        SetActionButton("Lanjut", true);
    }

    void NextQuestion()
    {
        currentIndex++;

        if (currentIndex < questions.Count)
            ShowCurrentQuestion();
        else
            EndQuiz();
    }

    void EndQuiz()
    {   
        UiIcons.SetActive(true);
        questionPanel.SetActive(false);
        actionButton.gameObject.SetActive(false);
        

         if (onQuizEnd != null) 
        {
            onQuizEnd.OnQuizEnd();
        }

        int total = questions.Count;
        int percent = Mathf.RoundToInt((score * 100f) / total);

        // kalau cuma 2 soal dan benar 1, jangan terlalu rendah
        if (total == 2 && percent == 50)
        {
            percent = 75;
        }

        Debug.Log($"Quiz selesai! Benar: {score}/{total} ({percent}%)");

        ShowScorePopup(percent, score, total);
    
        
    }

    void ShowScorePopup(int percent, int correct, int total)
    {
        if (scorePopup == null || scoreCounter == null || scoreDescText == null)
        {
            Debug.LogError("Score popup reference belum lengkap (scorePopup/scoreCounter/scoreDescText).");
            return;
        }

        scorePopup.SetActive(true);

        // reset dulu biar animasi dari 0
        scoreCounter.Value = 0;
        scoreCounter.NumberFormat = "0"; // biar tampil 0-100 tanpa koma
        scoreCounter.Duration = 0.5f;    // bisa kamu adjust

        // set deskripsi berdasarkan performa
        if (percent >= 100)
            scoreDescText.text = $"Sempurna! {correct}/{total} benar.";
        else if (percent >= 80)
            scoreDescText.text = $"Bagus. {correct}/{total} benar.";
        else if (percent >= 60)
            scoreDescText.text = $"Lumayan. {correct}/{total} benar. Coba ulangi ya.";
        else
            scoreDescText.text = $"Masih rendah. {correct}/{total} benar. Coba ulangi lagi.";

        // jalankan counter ke target
        scoreCounter.Value = percent;
    }

    void SetActionButton(string label, bool interactable)
    {
        actionButtonText.text = label;
        actionButton.interactable = interactable;

        if (actionButtonImage != null) 
            actionButtonImage.color = interactable ? enabledColor : disabledColor;
    }

    void ClearAnswers()
    {
        for (int i = 0; i < spawned.Count; i++)
        {
            if (spawned[i] != null) Destroy(spawned[i].gameObject);
        }
        spawned.Clear();
    }
    
}