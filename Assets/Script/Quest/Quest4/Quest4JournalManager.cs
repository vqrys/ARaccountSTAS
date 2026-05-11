using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro; // Wajib ditambahkan untuk teks Unlock COA
using System.Collections.Generic;

// --- CLASS UNTUK SETIAP MISI JURNAL ---
[System.Serializable]
public class JournalMissionConfig
{
    [Header("Pengaturan Misi")]
    [Tooltip("ID Misi/Objektif yang akan ditamatkan jika jurnal ini benar (Contoh: q4_transaksi_1)")]
    public string objectiveIdToComplete;

    [Header("Referensi UI Tabel")]
    [Tooltip("Masukkan semua DropSlot yang ada di tabel jurnal SPESIFIK INI (No Akun, Nama Akun, Debit, Kredit)")]
    public List<DropSlot> journalSlots = new List<DropSlot>();
    
    [Header("Tombol Spesifik Misi Ini")]
    public Button checkButton;
    [Tooltip("Opsional: Jika menggunakan Global Score Panel, tombol ini tidak wajib diisi")]
    public Button continueButton; 

    [Header("Reward & Score Panel")]
    [Tooltip("Tulis teks akun yang di-unlock, misal: 'Kas (Aset) + Modal Pemilik (Ekuitas)'")]
    public string unlockedCoaText;

    [Header("Event Spesifik (Opsional)")]
    [Tooltip("Event ini dipanggil saat jurnal ini benar. (Bisa untuk animasi kertas terbang)")]
    public UnityEvent onJournalCorrect;
}

public class Quest4JournalManager : MonoBehaviour
{
    [Header("Daftar Misi Jurnal (Misi 1 - 10)")]
    [Tooltip("Tambahkan daftar misi jurnal di sini. Setiap misi punya pengaturan tombol & slot masing-masing.")]
    public List<JournalMissionConfig> journalMissions = new List<JournalMissionConfig>();

    [Header("Score Popup Global")]
    [Tooltip("Masukkan objek UI ScorePanel yang akan muncul saat jawaban benar")]
    public GameObject scorePanel;
    [Tooltip("Masukkan komponen NumberCounter dari angka 100")]
    public NumberCounter scoreCounter;
    [Tooltip("Masukkan objek teks (TMP) untuk menampilkan tulisan Unlock COA")]
    public TMP_Text coaUnlockTextDisplay;
    [Tooltip("Masukkan tombol Lanjutkan yang berada DI DALAM Score Panel")]
    public Button scoreContinueButton;

    [Header("UI Kontrol Layar Global")]
    [Tooltip("Masukkan tombol UI yang akan digunakan untuk memutar layar (Opsional)")]
    public Button rotateScreenButton;

    private void Start()
    {
        // Sembunyikan panel skor global di awal
        if (scorePanel != null) scorePanel.SetActive(false);

        // Setup setiap misi jurnal yang didaftarkan di dalam list
        foreach (var mission in journalMissions)
        {
            JournalMissionConfig currentMission = mission;

            if (currentMission.continueButton != null) 
                currentMission.continueButton.gameObject.SetActive(false);
            
            if (currentMission.checkButton != null) 
            {
                currentMission.checkButton.onClick.RemoveAllListeners();
                // Hubungkan tombol "Periksa" ke fungsi pengecekan khusus untuk misi ini
                currentMission.checkButton.onClick.AddListener(() => CheckJournalEntries(currentMission));
            }
        }

        // Setup tombol rotasi layar (Berlaku global untuk semua)
        if (rotateScreenButton != null)
        {
            rotateScreenButton.onClick.RemoveAllListeners();
            rotateScreenButton.onClick.AddListener(ToggleScreenOrientation);
        }
    }

    public void ToggleScreenOrientation()
    {
        if (Screen.orientation == ScreenOrientation.Portrait)
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            Debug.Log("[Quest 4] Layar diputar ke Landscape.");
        }
        else
        {
            Screen.orientation = ScreenOrientation.Portrait;
            Debug.Log("[Quest 4] Layar diputar ke Portrait.");
        }
    }

    public void CheckJournalEntries(JournalMissionConfig mission)
    {
        bool isAllCorrect = true;

        foreach (var slot in mission.journalSlots)
        {
            if (slot.CurrentCard == null)
            {
                slot.SetVisualStatus(false, true); // Merah (Kosong)
                isAllCorrect = false;
            }
            else
            {
                bool isMatch = slot.CurrentCard.accountType == slot.acceptedType;
                slot.SetVisualStatus(isMatch, false);

                if (!isMatch)
                {
                    isAllCorrect = false;
                }
            }
        }

        if (isAllCorrect)
        {
            Debug.Log($"[Quest 4] Penjurnalan BENAR untuk misi: {mission.objectiveIdToComplete}");
            OnJournalPassed(mission);
        }
        else
        {
            Debug.Log($"[Quest 4] Penjurnalan masih salah/kosong untuk misi: {mission.objectiveIdToComplete}");
        }
    }

    private void OnJournalPassed(JournalMissionConfig mission)
    {
        // 1. Matikan tombol periksa milik misi ini
        if (mission.checkButton != null) mission.checkButton.gameObject.SetActive(false);

        // 2. Kunci semua kartu pada tabel milik misi ini
        foreach (var slot in mission.journalSlots)
        {
            if (slot.CurrentCard != null) slot.CurrentCard.IsLocked = true;
        }

        // 3. Panggil Event animasi 3D (misal kertas terbang)
        mission.onJournalCorrect?.Invoke();

        // 4. MUNCULKAN SCORE PANEL
        ShowScorePanel(mission);
    }

    private void ShowScorePanel(JournalMissionConfig mission)
    {
        if (scorePanel != null)
        {
            scorePanel.SetActive(true);

            // Set animasi angka 0 ke 100
            if (scoreCounter != null)
            {
                scoreCounter.Value = 0;
                scoreCounter.NumberFormat = "0";
                scoreCounter.Duration = 0.5f;
                scoreCounter.Value = 100;
            }

            // Set teks Unlock COA sesuai dengan misi yang sedang dikerjakan
            if (coaUnlockTextDisplay != null)
            {
                coaUnlockTextDisplay.text = string.IsNullOrEmpty(mission.unlockedCoaText) ? "-" : mission.unlockedCoaText;
            }

            // Hubungkan tombol Lanjutkan di popup untuk menamatkan misi ini
            if (scoreContinueButton != null)
            {
                scoreContinueButton.onClick.RemoveAllListeners();
                scoreContinueButton.onClick.AddListener(() => ProceedToNextMission(mission));
            }
        }
        else
        {
            // Fallback jika tidak memakai Score Panel Global
            if (mission.continueButton != null) mission.continueButton.gameObject.SetActive(true);
            
            if (mission.continueButton != null)
            {
                mission.continueButton.onClick.RemoveAllListeners();
                mission.continueButton.onClick.AddListener(() => ProceedToNextMission(mission));
            }
        }
    }

    private void ProceedToNextMission(JournalMissionConfig mission)
    {
        // Tutup UI Popup
        if (scorePanel != null) scorePanel.SetActive(false);
        if (mission.continueButton != null) mission.continueButton.gameObject.SetActive(false);

        // Lapor ke MissionSystem agar melanjutkan ke misi berikutnya
        if (MissionSystem.Instance != null && !string.IsNullOrEmpty(mission.objectiveIdToComplete))
        {
            MissionSystem.Instance.CompleteObjectiveById(mission.objectiveIdToComplete);
            Debug.Log($"[Quest 4] Sinyal tamat dikirim untuk: {mission.objectiveIdToComplete}");
        }
    }
}