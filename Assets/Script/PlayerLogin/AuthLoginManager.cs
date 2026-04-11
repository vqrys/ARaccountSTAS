using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Authentication; 
using System.Threading.Tasks;
using TMPro;

public class AuthLoginManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button deleteAccountButton; 
    [Tooltip("Tarik Text (TMP) yang ada di dalam tombol Delete ke sini")]
    [SerializeField] private TMP_Text deleteButtonText; 

    private int _deleteClickCount = 0;

    private async void Start()
    {
        if (deleteAccountButton != null)
        {
            deleteAccountButton.gameObject.SetActive(false);
            deleteAccountButton.onClick.RemoveAllListeners();
            deleteAccountButton.onClick.AddListener(DeleteAccount);
        }

        await UGSBootstrap.InitializeServicesAsync();

        if (deleteAccountButton != null)
        {
            bool hasToken = AuthenticationService.Instance.SessionTokenExists;
            deleteAccountButton.gameObject.SetActive(hasToken);
        }
    }

    public void LoginAsGuest()
    {
        Debug.Log("[AuthLoginManager] Guest button clicked.");
        if (AuthManager.Instance != null) AuthManager.Instance.SignInAnonymously();
    }

    public void LoginWithUnity()
    {
        if (AuthManager.Instance != null) AuthManager.Instance.SignInWithUnity();
    }

    // FUNGSI KONFIRMASI 2 KALI
    public void DeleteAccount()
    {
        _deleteClickCount++;

        if (_deleteClickCount == 1)
        {
            Debug.Log("[AuthLoginManager] Konfirmasi pertama ditekan! Menunggu klik kedua...");
            if (deleteButtonText != null)
            {
                deleteButtonText.text = "Yakin Hapus? (Klik Lagi)";
                deleteButtonText.color = Color.red; // Ubah warna peringatan
            }
            
            // Batalkan konfirmasi jika tidak diklik lagi dalam 3 detik
            Invoke(nameof(ResetDeleteButton), 3f);
        }
        else if (_deleteClickCount >= 2)
        {
            Debug.Log("[AuthLoginManager] Mengeksekusi penghapusan akun!");
            CancelInvoke(nameof(ResetDeleteButton)); // Hentikan timer
            
            if (deleteButtonText != null) deleteButtonText.text = "Menghapus...";

            if (AuthManager.Instance != null)
            {
                AuthManager.Instance.DeleteAccount();
            }
        }
    }

    private void ResetDeleteButton()
    {
        _deleteClickCount = 0;
        if (deleteButtonText != null)
        {
            deleteButtonText.text = "Hapus Akun";
            deleteButtonText.color = Color.white; // Kembalikan warna awal
        }
    }
}