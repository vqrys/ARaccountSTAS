using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Tambahkan ini untuk akses perpindahan scene darurat

public class AuthHomeManager : MonoBehaviour
{
    [SerializeField] private Button linkUnityButton;
    [SerializeField] private Button signOutButton; // Pastikan ini ada di script-mu!

    private void Start()
    {
        RefreshUI();

        // Menyambungkan tombol secara otomatis lewat kode (lebih aman daripada manual di inspector)
        if (signOutButton != null)
        {
            signOutButton.onClick.RemoveAllListeners();
            signOutButton.onClick.AddListener(SignOut);
        }
    }

    public void SignOut()
    {
        if (AuthManager.Instance != null)
        {
            // Jika ada AuthManager (Game dimainkan dari LoginScene), hapus sesi Guest/Player
            AuthManager.Instance.SignOut();
        }
        else
        {
            Debug.LogWarning("[AuthHomeManager] AuthManager tidak ditemukan! Memaksa kembali ke LoginScene...");
            
            // Fallback Darurat: Paksa pindah ke LoginScene meskipun AuthManager hilang
            if (SceneTransitionManager.Instance != null)
            {
                SceneTransitionManager.Instance.LoadScene("LoginScene");
            }
            else
            {
                SceneManager.LoadScene("LoginScene");
            }
        }
    }

    public void LinkWithUnity()
    {
        if (AuthManager.Instance == null)
        {
            Debug.LogError("[AuthHomeManager] AuthManager not found.");
            return;
        }

        // langsung panggil flow Unity login
        AuthManager.Instance.SignInWithUnity();
    }

    public void RefreshUI()
    {
        if (linkUnityButton == null) return;

        if (AuthManager.Instance == null)
        {
            linkUnityButton.interactable = false;
            return;
        }

        // jika sudah Unity account → disable tombol
        if (!AuthManager.Instance.IsGuest())
        {
            linkUnityButton.interactable = false;
        }
        else
        {
            linkUnityButton.interactable = true;
        }
    }
}