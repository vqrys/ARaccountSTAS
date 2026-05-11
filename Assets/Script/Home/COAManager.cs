using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

// Enum untuk kategori akun
public enum AccountCategory
{
    Semua,      // Index 0
    Aset,       // Index 1
    Kewajiban,  // Index 2
    Ekuitas,    // Index 3
    Pendapatan, // Index 4
    Beban       // Index 5
}

// Class ini adalah cetak biru untuk setiap kartu akun di COA
[System.Serializable]
public class COAItem
{
    [Header("Data Akun")]
    public string accountName;         // Nama akun, misal: "Kas"
    public string accountNumber;       // Nomor akun, misal: "101"
    public AccountCategory category;   // Kategori akun (Aset, Kewajiban, dll)
    public string accountDescription;  // Deskripsi singkat akun
    public string requiredMissionId;   // ID misi yang membuka akun ini
    public bool isUnlocked = false;    // Status apakah sudah terbuka atau belum

    [Header("Icon Akun")]
    [Tooltip("Sprite icon untuk akun ini (opsional)")]
    public Sprite accountIcon;

    [Header("Teks Saat Terkunci")]
    [Tooltip("Teks pengganti nama akun saat masih terkunci")]
    public string lockedAccountName = "AKUN TERKUNCI";

    [Tooltip("Pesan yang tampil jika akun belum terbuka")]
    [TextArea(2, 3)] 
    public string lockedMessage = "Selesaikan Quest 4 untuk membuka";

    [Header("Referensi UI Kartu")]
    [Tooltip("Masukkan GameObject kartu versi Terkunci (Ada gemboknya)")]
    public GameObject lockedCardObject; 
    
    [Tooltip("Masukkan komponen Text (TMP) untuk JUDUL/NAMA AKUN di dalam Locked Card")]
    public TMP_Text lockedAccountNameText;
    
    [Tooltip("Masukkan komponen Text (TMP) untuk PESAN di dalam Locked Card")]
    public TMP_Text lockedMessageText;
    
    [Tooltip("Masukkan GameObject kartu versi Terbuka (Ada teks dan detailnya)")]
    public GameObject unlockedCardObject;
}

public class COAManager : MonoBehaviour
{
    [Header("Database Akun COA")]
    public List<COAItem> coaDatabase;

    [Header("Referensi UI Panel Utama")]
    public GameObject coaPanel;      // Panel besar menu COA
    public Button openCoaButton;     // Tombol untuk membuka menu COA
    
    [Header("Referensi UI Progress")]
    [Tooltip("Masukkan teks yang menampilkan angka progress, misal '0 / 11'")]
    public TMP_Text progressText;

    [Header("Filter Sederhana (3 Tombol)")]
    [Tooltip("Tombol untuk menampilkan semua akun")]
    public Button filterSemuaButton;
    
    [Tooltip("Tombol panah kiri untuk pindah kategori sebelumnya")]
    public Button previousCategoryButton;
    
    [Tooltip("Tombol panah kanan untuk pindah kategori berikutnya")]
    public Button nextCategoryButton;

    [Tooltip("Text yang menampilkan nama kategori aktif (Semua, Aset, dll)")]
    public TMP_Text filterCategoryText;

    // Variabel internal untuk tracking filter aktif
    private AccountCategory currentCategory = AccountCategory.Semua;

    private void Start()
    {
        // 1. Pastikan panel COA tertutup saat game baru dimulai
        if (coaPanel != null) coaPanel.SetActive(false);

        // 2. Berikan perintah pada tombol untuk membuka panel saat diklik
        if (openCoaButton != null)
        {
            openCoaButton.onClick.RemoveAllListeners();
            openCoaButton.onClick.AddListener(OpenCOAPanel);
        }

        // 3. Daftarkan fungsi ke 3 tombol filter
        RegisterFilterButtons();

        // 4. Update tampilan awal
        UpdateAllCardsUI();
        UpdateProgressText();
        UpdateFilterCategoryText();
    }

    // Mendaftarkan listener ke 3 tombol filter
    private void RegisterFilterButtons()
    {
        if (filterSemuaButton != null)
        {
            filterSemuaButton.onClick.RemoveAllListeners();
            filterSemuaButton.onClick.AddListener(ShowAllCategories);
        }

        if (previousCategoryButton != null)
        {
            previousCategoryButton.onClick.RemoveAllListeners();
            previousCategoryButton.onClick.AddListener(NavigateToPreviousCategory);
        }

        if (nextCategoryButton != null)
        {
            nextCategoryButton.onClick.RemoveAllListeners();
            nextCategoryButton.onClick.AddListener(NavigateToNextCategory);
        }
    }

    // Fungsi ini dipanggil ketika pemain menekan tombol Buka COA
    public void OpenCOAPanel()
    {
        if (coaPanel == null) return;
        
        coaPanel.SetActive(true);
        
        // Cek apakah ada misi baru yang diselesaikan, lalu perbarui layar
        CheckUnlocksFromMissionSystem();
        UpdateAllCardsUI();
        UpdateProgressText();
        UpdateFilterCategoryText();
    }

    // Fungsi untuk menutup panel
    public void CloseCOAPanel()
    {
         if (coaPanel != null) coaPanel.SetActive(false);
    }

    // Mengecek ke MissionSystem apakah syarat misi sudah terpenuhi
    private void CheckUnlocksFromMissionSystem()
    {
        // Cegah error jika MissionSystem belum ada di scene
        if (MissionSystem.Instance == null) return;

        foreach (var item in coaDatabase)
        {
            // Jika akun ini masih terkunci dan memiliki syarat misi...
            if (!item.isUnlocked && !string.IsNullOrEmpty(item.requiredMissionId))
            {
                // Tanya MissionSystem: "Apakah misi ini sudah tamat?"
                if (MissionSystem.Instance.IsObjectiveCompleted(item.requiredMissionId))
                {
                    item.isUnlocked = true; // Buka kunci akunnya!
                    Debug.Log($"[COA] Akun Terbuka: {item.accountName}");
                }
            }
        }
    }

    // ========== SISTEM FILTER SEDERHANA (3 TOMBOL) ==========

    // Tombol "Semua" - Kembali ke tampilan semua kategori
    private void ShowAllCategories()
    {
        currentCategory = AccountCategory.Semua;
        
        Debug.Log("[COA] Filter: Menampilkan Semua Akun");
        
        UpdateAllCardsUI();
        UpdateFilterCategoryText();
    }

    // Navigasi ke kategori sebelumnya (panah kiri ◀)
    private void NavigateToPreviousCategory()
    {
        int currentIndex = (int)currentCategory;
        currentIndex--;
        
        // Wrap around: jika sudah di Semua (0), kembali ke Beban (5)
        if (currentIndex < 0) 
            currentIndex = System.Enum.GetValues(typeof(AccountCategory)).Length - 1;
        
        currentCategory = (AccountCategory)currentIndex;
        
        Debug.Log($"[COA] Filter: {currentCategory}");
        
        UpdateAllCardsUI();
        UpdateFilterCategoryText();
    }

    // Navigasi ke kategori berikutnya (panah kanan ▶)
    private void NavigateToNextCategory()
    {
        int currentIndex = (int)currentCategory;
        currentIndex++;
        
        // Wrap around: jika sudah di Beban (5), kembali ke Semua (0)
        if (currentIndex >= System.Enum.GetValues(typeof(AccountCategory)).Length) 
            currentIndex = 0;
        
        currentCategory = (AccountCategory)currentIndex;
        
        Debug.Log($"[COA] Filter: {currentCategory}");
        
        UpdateAllCardsUI();
        UpdateFilterCategoryText();
    }

    // Update teks kategori filter yang sedang aktif
    private void UpdateFilterCategoryText()
    {
        if (filterCategoryText == null) return;

        // Konversi enum ke teks Indonesia
        string categoryName = currentCategory switch
        {
            AccountCategory.Semua => "Semua",
            AccountCategory.Aset => "Aset",
            AccountCategory.Kewajiban => "Kewajiban",
            AccountCategory.Ekuitas => "Ekuitas",
            AccountCategory.Pendapatan => "Pendapatan",
            AccountCategory.Beban => "Beban",
            _ => "Semua"
        };

        filterCategoryText.text = categoryName;
    }

    // Memperbarui visual semua kartu berdasarkan filter aktif
    private void UpdateAllCardsUI()
    {
        foreach (var item in coaDatabase)
        {
            // CEK FILTER: Apakah akun ini sesuai dengan kategori yang dipilih?
            bool matchesFilter = (currentCategory == AccountCategory.Semua) || 
                                 (item.category == currentCategory);

            // Jika tidak sesuai filter, sembunyikan kartu ini
            if (!matchesFilter)
            {
                if (item.lockedCardObject != null) item.lockedCardObject.SetActive(false);
                if (item.unlockedCardObject != null) item.unlockedCardObject.SetActive(false);
                continue; // Lewati akun ini
            }

            // Jika sampai di sini, berarti akun sesuai filter
            if (item.isUnlocked)
            {
                // Jika terbuka: Matikan kartu gembok, nyalakan kartu detail
                if (item.lockedCardObject != null) item.lockedCardObject.SetActive(false);
                if (item.unlockedCardObject != null) item.unlockedCardObject.SetActive(true);
            }
            else
            {
                // Jika terkunci: Nyalakan kartu gembok, matikan kartu detail
                if (item.lockedCardObject != null) item.lockedCardObject.SetActive(true);
                if (item.unlockedCardObject != null) item.unlockedCardObject.SetActive(false);

                // --- MENERAPKAN TEKS SAAT TERKUNCI ---
                if (item.lockedAccountNameText != null)
                {
                    item.lockedAccountNameText.text = item.lockedAccountName;
                }

                if (item.lockedMessageText != null)
                {
                    item.lockedMessageText.text = item.lockedMessage;
                }
            }
        }
    }

    // Memperbarui teks progress (misal dari "0 / 11" menjadi "5 / 11")
    private void UpdateProgressText()
    {
        if (progressText == null) return;

        int totalAccounts = coaDatabase.Count;
        int unlockedCount = 0;

        // Hitung berapa banyak yang isUnlocked = true
        foreach (var item in coaDatabase)
        {
            if (item.isUnlocked) unlockedCount++;
        }

        // Tuliskan ke dalam UI Text
        progressText.text = $"{unlockedCount} / {totalAccounts}";
    }

    // ========== FUNGSI UTILITAS (OPSIONAL) ==========

    // Mendapatkan jumlah akun per kategori
    public int GetCategoryCount(AccountCategory category)
    {
        if (category == AccountCategory.Semua)
            return coaDatabase.Count;
        
        return coaDatabase.Count(item => item.category == category);
    }

    // Mendapatkan jumlah akun terbuka per kategori
    public int GetUnlockedCategoryCount(AccountCategory category)
    {
        if (category == AccountCategory.Semua)
            return coaDatabase.Count(item => item.isUnlocked);
        
        return coaDatabase.Count(item => item.category == category && item.isUnlocked);
    }
}