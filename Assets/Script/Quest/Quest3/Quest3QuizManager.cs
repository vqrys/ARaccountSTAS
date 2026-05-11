using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Quest3QuizManager : MonoBehaviour
{
    [Header("Pengaturan Kuis")]
    [Tooltip("ID Objektif yang akan ditamatkan saat kuis ini benar semua (Misal: q3_misi1)")]
    public string objectiveIdToComplete;

    [Header("Daftar Slot Drop")]
    [Tooltip("Masukkan SEMUA slot drop (keranjang/kotak) yang ada di kuis ini")]
    public List<DropSlot> dropSlots = new List<DropSlot>();

    [Header("UI Tombol Utama")]
    public Button checkButton;
    [Tooltip("Tombol lanjutkan biasa (di luar popup) - Opsional")]
    public Button continueButton;

    [Header("Score Popup (Opsional)")]
    [Tooltip("Masukkan objek UI ScorePanel")]
    public GameObject scorePanel;
    [Tooltip("Masukkan Text yang memiliki komponen NumberCounter")]
    public NumberCounter scoreCounter;
    [Tooltip("Masukkan tombol Lanjutkan yang berada DI DALAM Score Panel")]
    public Button scoreContinueButton;

    private void Start()
    {
        // Sembunyikan UI Lanjutan & Popup di awal
        if (continueButton != null) continueButton.gameObject.SetActive(false);
        if (scorePanel != null) scorePanel.SetActive(false);
        
        // Daftarkan fungsi ke tombol periksa
        if (checkButton != null) 
        {
            checkButton.onClick.RemoveAllListeners();
            checkButton.onClick.AddListener(CheckAnswers);
        }

        // Daftarkan fungsi ke tombol lanjutkan (di dalam popup)
        if (scoreContinueButton != null)
        {
            scoreContinueButton.onClick.RemoveAllListeners();
            scoreContinueButton.onClick.AddListener(OnContinueClicked);
        }
        else if (continueButton != null) // Fallback jika tidak pakai popup
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(OnContinueClicked);
        }
    }

    public void CheckAnswers()
    {
        bool isAllCorrect = true;
        
        Dictionary<Image, int> groupTotalSlots = new Dictionary<Image, int>();
        Dictionary<Image, int> groupCorrectSlots = new Dictionary<Image, int>();

        // 1. Periksa setiap slot
        foreach (var slot in dropSlots)
        {
            bool isSlotCorrect = false;

            if (slot.CurrentCard != null)
            {
                isSlotCorrect = (slot.CurrentCard.accountType == slot.acceptedType);
            }

            if (!isSlotCorrect)
            {
                isAllCorrect = false;
            }

            // 2. Kumpulkan statistik grup gambar
            if (slot.feedbackImage != null)
            {
                if (!groupTotalSlots.ContainsKey(slot.feedbackImage))
                {
                    groupTotalSlots[slot.feedbackImage] = 0;
                    groupCorrectSlots[slot.feedbackImage] = 0;
                }

                groupTotalSlots[slot.feedbackImage]++;
                if (isSlotCorrect) groupCorrectSlots[slot.feedbackImage]++;
            }
        }

        // 3. Terapkan warna
        foreach (var kvp in groupTotalSlots)
        {
            Image sharedImage = kvp.Key;
            int total = kvp.Value;
            int correct = groupCorrectSlots[sharedImage];

            if (correct == total)
            {
                sharedImage.color = new Color(0.2f, 0.8f, 0.2f, 1f); // Hijau
            }
            else if (correct == 0)
            {
                sharedImage.color = new Color(0.8f, 0.2f, 0.2f, 1f); // Merah
            }
            else
            {
                sharedImage.color = new Color(0.9f, 0.7f, 0.1f, 1f); // Kuning/Oranye
            }
        }

        // 4. Jika seluruh kuis benar
        if (isAllCorrect)
        {
            OnQuizPassed();
        }
        else
        {
            Debug.Log("[Quest3Quiz] Jawaban masih ada yang salah atau kosong.");
        }
    }

    private void OnQuizPassed()
    {
        Debug.Log($"[Quest3Quiz] Kuis Lulus! Memunculkan Popup Nilai.");

        // Matikan tombol Periksa
        if (checkButton != null) checkButton.gameObject.SetActive(false);

        // Kunci semua kartu
        foreach (var slot in dropSlots)
        {
            if (slot.CurrentCard != null) slot.CurrentCard.IsLocked = true;
        }

        // MUNCULKAN SCORE POPUP
        if (scorePanel != null)
        {
            scorePanel.SetActive(true);

            // Jalankan animasi angka dari 0 ke 100
            if (scoreCounter != null)
            {
                scoreCounter.Value = 0;
                scoreCounter.NumberFormat = "0";
                scoreCounter.Duration = 0.5f; 
                scoreCounter.Value = 100; // Karena Drag&Drop harus benar semua, nilainya mutlak 100
            }
        }
        else
        {
            // Jika ScorePanel belum dipasang di Inspector, munculkan tombol Lanjut biasa (Fallback)
            if (continueButton != null) continueButton.gameObject.SetActive(true);
        }
    }

    // Fungsi ini dipanggil saat tombol Lanjutkan di klik
    private void OnContinueClicked()
    {
        // Tutup UI Kuis
        if (scorePanel != null) scorePanel.SetActive(false);
        if (continueButton != null) continueButton.gameObject.SetActive(false);

        // Lapor ke MissionSystem agar melanjutkan cerita
        if (MissionSystem.Instance != null && !string.IsNullOrEmpty(objectiveIdToComplete))
        {
            MissionSystem.Instance.CompleteObjectiveById(objectiveIdToComplete);
            Debug.Log($"[Quest3Quiz] Mengirim sinyal tamat untuk: {objectiveIdToComplete}");
        }
    }
}