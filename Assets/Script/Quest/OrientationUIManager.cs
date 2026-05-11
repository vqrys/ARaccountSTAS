using UnityEngine;
using System.Collections.Generic;

// Class untuk menyimpan data setiap panel yang bisa dipindah-pindah
[System.Serializable]
public class MovablePanelConfig
{
    [Tooltip("Masukkan RectTransform dari panel yang posisinya ingin diubah (Misal: Subtitle Panel)")]
    public RectTransform panel;

    [Tooltip("Kordinat Posisi (X, Y) panel saat layar berdiri (Portrait)")]
    public Vector2 portraitPosition;

    [Tooltip("Kordinat Posisi (X, Y) panel saat layar miring (Landscape)")]
    public Vector2 landscapePosition;
}

public class OrientationUIManager : MonoBehaviour
{
    [Header("Pengaturan Aktifasi")]
    [Tooltip("Jika MATI, peringatan dan tabel tidak akan muncul sampai fungsi ActivateOrientationUI() dipanggil.")]
    public bool startActive = false;

    [Header("UI Panels")]
    [Tooltip("Panel peringatan (Info) yang muncul saat HP posisi berdiri (Portrait)")]
    public GameObject portraitInfoPanel;

    [Tooltip("Panel utama (Tabel Jurnal) yang muncul saat HP posisi miring (Landscape)")]
    public GameObject landscapeMainPanel;

    [Header("Movable Panels (Opsional)")]
    [Tooltip("Daftar panel yang posisinya ingin diubah menyesuaikan rotasi layar")]
    public List<MovablePanelConfig> movablePanels = new List<MovablePanelConfig>();

    private bool _wasLandscape;
    private bool _isActive;

    private void Start()
    {
        // 1. Matikan semuanya dulu agar layar bersih di awal
        if (portraitInfoPanel != null) portraitInfoPanel.SetActive(false);
        if (landscapeMainPanel != null) landscapeMainPanel.SetActive(false);

        _isActive = startActive;
        _wasLandscape = (Screen.width > Screen.height);
        
        // 2. Terapkan tampilan awal HANYA JIKA dicentang aktif sejak awal
        if (_isActive)
        {
            ApplyOrientationUI(_wasLandscape);
        }
    }

    private void Update()
    {
        // Jika sistem sedang dimatikan (belum dipanggil), abaikan cek rotasi
        if (!_isActive) return;

        bool isCurrentlyLandscape = Screen.width > Screen.height;

        // Jika orientasi HP berubah (Pemain baru saja memutar HP-nya)
        if (isCurrentlyLandscape != _wasLandscape)
        {
            ApplyOrientationUI(isCurrentlyLandscape);
            _wasLandscape = isCurrentlyLandscape; // Simpan status terbaru
        }
    }

    // =========================================================
    // FUNGSI INI YANG HARUS KAMU PANGGIL DARI RACCOON SPEAKER
    // =========================================================
    public void ActivateOrientationUI()
    {
        _isActive = true;
        _wasLandscape = (Screen.width > Screen.height);
        
        // Langsung terapkan tampilan (munculkan panel) sesuai rotasi HP saat ini
        ApplyOrientationUI(_wasLandscape);
        Debug.Log("[OrientationUI] Sistem Rotasi Diaktifkan! Memeriksa layar...");
    }

    // Opsional: Untuk mematikan kembali UI jika Raccoon sedang bicara hal lain
    public void DeactivateOrientationUI()
    {
        _isActive = false;
        if (portraitInfoPanel != null) portraitInfoPanel.SetActive(false);
        if (landscapeMainPanel != null) landscapeMainPanel.SetActive(false);
    }

    // Fungsi internal untuk mengubah UI berdasarkan orientasi
    private void ApplyOrientationUI(bool isLandscape)
    {
        if (isLandscape)
        {
            // L A N D S C A P E
            Debug.Log("[OrientationUI] Mode Landscape Terdeteksi. Menampilkan Tabel Jurnal.");
            
            if (portraitInfoPanel != null) portraitInfoPanel.SetActive(false);
            if (landscapeMainPanel != null) landscapeMainPanel.SetActive(true);

            foreach (var config in movablePanels)
            {
                if (config.panel != null) config.panel.anchoredPosition = config.landscapePosition;
            }
        }
        else
        {
            // P O R T R A I T
            Debug.Log("[OrientationUI] Mode Portrait Terdeteksi. Menampilkan Peringatan Putar Layar.");
            
            if (portraitInfoPanel != null) portraitInfoPanel.SetActive(true);
            if (landscapeMainPanel != null) landscapeMainPanel.SetActive(false); 

            foreach (var config in movablePanels)
            {
                if (config.panel != null) config.panel.anchoredPosition = config.portraitPosition;
            }
        }
    }
}