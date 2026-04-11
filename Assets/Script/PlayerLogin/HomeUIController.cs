using System;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Authentication.PlayerAccounts; 

public class HomeUIController : MonoBehaviour
{
    [SerializeField] private List<TMP_Text> playerNameText = new List<TMP_Text>();
    [SerializeField] private GameObject createNamePanel;

    [Header("Profile Avatar GameObjects")]
    [Tooltip("Masukkan GameObject Avatar Boy ke sini")]
    [SerializeField] private List<GameObject> boyAvatars = new List<GameObject>();
    
    [Tooltip("Masukkan GameObject Avatar Girl ke sini")]
    [SerializeField] private List<GameObject> girlAvatars = new List<GameObject>();
    
    [Tooltip("Masukkan GameObject Avatar Guest ke sini")]
    [SerializeField] private List<GameObject> guestAvatars = new List<GameObject>();

    private async void Start()
    {
        await RefreshPlayerNameAsync();
    }

    public async System.Threading.Tasks.Task RefreshPlayerNameAsync()
    {
        if (playerNameText == null || playerNameText.Count == 0)
        {
            Debug.LogWarning("[HomeUIController] playerNameText list is empty.");
        }

        // 1. Cek apakah ini Guest
        bool isGuest = true;
        
        if (PlayerAccountService.Instance.IsSignedIn)
        {
            isGuest = false;
        }
        else if (AuthManager.Instance != null && !AuthManager.Instance.IsGuest())
        {
            isGuest = false;
        }

        // 2. JIKA GUEST
        if (isGuest)
        {
            SetProfile("Guest", "Guest");
            ShowCreateNamePanel(false);
            return;
        }

        // 3. JIKA BUKAN GUEST (Akun Unity) -> Cek data di Cloud Save
        if (PlayerDataClient.Instance == null)
        {
            Debug.LogWarning("[HomeUIController] PlayerDataClient instance not found.");
            SetProfile("Pemain Baru", "Boy"); // Ubah penyamaran agar tidak membingungkan
            ShowCreateNamePanel(true);
            return;
        }

        try
        {
            Debug.Log("[HomeUIController] Mencoba mengambil profil dari Cloud...");
            PlayerProfile profile = await PlayerDataClient.Instance.GetProfileDataAsync();

            Debug.Log($"[HomeUIController] Hasil Cloud -> Nama: '{profile.Name}', Avatar: '{profile.Avatar}'");

            if (string.IsNullOrWhiteSpace(profile.Name))
            {
                Debug.Log("[HomeUIController] Nama kosong! Menampilkan Panel Create Name...");
                SetProfile("Pemain Baru", "Boy"); 
                ShowCreateNamePanel(true); 
            }
            else
            {
                Debug.Log("[HomeUIController] Nama ditemukan! Menyembunyikan Panel Create Name...");
                SetProfile(profile.Name, profile.Avatar);
                ShowCreateNamePanel(false);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[HomeUIController] GAGAL MEMUAT CLOUD SAVE: {ex.Message}");
            SetProfile("Pemain Baru", "Boy");
            ShowCreateNamePanel(true); 
            Debug.Log("[HomeUIController] Memaksa Panel Create Name muncul setelah error Cloud!");
        }
    }

    public void SetProfile(string name, string avatarId)
    {
        if (playerNameText != null)
        {
            foreach (TMP_Text textElement in playerNameText)
            {
                if (textElement != null)
                {
                    textElement.text = name;
                }
            }
        }

        bool isBoy = (avatarId == "Boy");
        bool isGirl = (avatarId == "Girl");
        bool isGuest = (!isBoy && !isGirl); 

        if (boyAvatars != null)
        {
            foreach (GameObject obj in boyAvatars)
            {
                if (obj != null) obj.SetActive(isBoy);
            }
        }

        if (girlAvatars != null)
        {
            foreach (GameObject obj in girlAvatars)
            {
                if (obj != null) obj.SetActive(isGirl);
            }
        }

        if (guestAvatars != null)
        {
            foreach (GameObject obj in guestAvatars)
            {
                if (obj != null) obj.SetActive(isGuest);
            }
        }
    }

    public void ShowCreateNamePanel(bool show)
    {
        if (createNamePanel != null)
        {
            createNamePanel.SetActive(show);

            // JIKA PANEL DISURUH MUNCUL, PAKSA DIA BERADA DI TUMPUKAN PALING ATAS LAYAR!
            if (show)
            {
                createNamePanel.transform.SetAsLastSibling();
            }
        }
    }
}