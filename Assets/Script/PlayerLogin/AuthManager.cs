using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance { get; private set; }

    public event Action OnSignedIn;

    [SerializeField] private string homeSceneName = "HomeScene";
    [SerializeField] private string loginSceneName = "LoginScene";

    private bool _isSigningIn;

    private async void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        await UGSBootstrap.InitializeServicesAsync();

        PlayerAccountService.Instance.SignedIn += HandleUnityPlayerAccountSignedIn;
    }

    private async void Start()
    {
        await TryAutoLoginAsync();
    }

    public async void SignInAnonymously()
    {
        if (_isSigningIn) return;

        try
        {
            _isSigningIn = true;

            if (AuthenticationService.Instance.IsSignedIn) return;

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("[AuthManager] Anonymous sign-in success.");
            HandleLoginSuccess();
        }
        catch (Exception ex)
        {
            Debug.LogError($"[AuthManager] Anonymous sign-in failed: {ex.Message}");
        }
        finally
        {
            _isSigningIn = false;
        }
    }

    public async void SignInWithUnity()
    {
        if (_isSigningIn) return;

        try
        {
            _isSigningIn = true;

            if (!PlayerAccountService.Instance.IsSignedIn)
            {
                await PlayerAccountService.Instance.StartSignInAsync();
            }
            else
            {
                await SignInOrLinkWithUnityAsync(PlayerAccountService.Instance.AccessToken);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[AuthManager] Unity sign-in failed: {ex.Message}");
            _isSigningIn = false;
        }
    }

    private async void HandleUnityPlayerAccountSignedIn()
    {
        try
        {
            await SignInOrLinkWithUnityAsync(PlayerAccountService.Instance.AccessToken);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[AuthManager] Handle Unity account sign-in failed: {ex.Message}");
        }
        finally
        {
            _isSigningIn = false;
        }
    }

    private async Task SignInOrLinkWithUnityAsync(string accessToken)
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);
            Debug.Log("[AuthManager] Signed in with Unity account.");
            HandleLoginSuccess();
            return;
        }

        bool hasUnityId = AuthenticationService.Instance.PlayerInfo != null &&
                          AuthenticationService.Instance.PlayerInfo.GetUnityId() != null;

        if (!hasUnityId)
        {
            await AuthenticationService.Instance.LinkWithUnityAsync(accessToken);
            Debug.Log("[AuthManager] Guest account linked to Unity account.");
        }

        HandleLoginSuccess();
    }

    private async Task TryAutoLoginAsync()
    {
        try
        {
            if (!AuthenticationService.Instance.SessionTokenExists)
            {
                Debug.Log("[AuthManager] No session token found. Player baru.");
                if (SceneTransitionManager.Instance != null)
                {
                    SceneTransitionManager.Instance.HideLoadingScreen();
                }
            }

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("[AuthManager] Auto login success.");
            HandleLoginSuccess();
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[AuthManager] Auto login failed: {ex.Message}");
            if (SceneTransitionManager.Instance != null)
            {
                SceneTransitionManager.Instance.HideLoadingScreen();
            }
        }
    }

    private void HandleLoginSuccess()
    {
        OnSignedIn?.Invoke();

        // SURUH MISSION SYSTEM MEMUAT ULANG DATA DARI AWAL
        if (MissionSystem.Instance != null)
        {
            MissionSystem.Instance.InitializeSystemAsync();
        }

        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(homeSceneName);
        }
    }

    public void SignOut()
    {
        try
        {
            if (AuthenticationService.Instance.IsSignedIn)
            {
                bool isUnityLinked = AuthenticationService.Instance.PlayerInfo != null &&
                                     AuthenticationService.Instance.PlayerInfo.GetUnityId() != null;

                AuthenticationService.Instance.SignOut(clearCredentials: !isUnityLinked);
            }
            
            if (PlayerAccountService.Instance.IsSignedIn)
            {
                PlayerAccountService.Instance.SignOut();
            }

            Debug.Log("[AuthManager] Signed out.");

            // BERSIHKAN RAM MISSION SYSTEM
            if (MissionSystem.Instance != null)
            {
                MissionSystem.Instance.ResetSystem();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[AuthManager] Sign out error (akan tetap dipaksa ke Login): {ex.Message}");
        }
        finally
        {
            if (SceneTransitionManager.Instance != null)
            {
                SceneTransitionManager.Instance.LoadScene(loginSceneName);
            }
            else
            {
                SceneManager.LoadScene(loginSceneName);
            }
        }
    }

    public async void DeleteAccount()
    {
        try
        {
            Debug.Log("[AuthManager] Memulai proses hapus akun...");

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                Debug.Log("[AuthManager] Sesi belum aktif. Melakukan sign-in darurat sebelum menghapus...");
                if (AuthenticationService.Instance.SessionTokenExists)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }
                else
                {
                    Debug.LogWarning("[AuthManager] Tidak ada token untuk dihapus.");
                    return;
                }
            }

            // 1. HAPUS DATA CLOUD SAVE TERLEBIH DAHULU SEBELUM AKUN DIHAPUS
            try
            {
                Debug.Log("[AuthManager] Menyapu bersih data Cloud Save...");
                var keys = await Unity.Services.CloudSave.CloudSaveService.Instance.Data.Player.ListAllKeysAsync();
                
                // Menyiapkan opsi penghapusan versi terbaru untuk menghilangkan Warning CS0618
                var deleteOptions = new Unity.Services.CloudSave.Models.Data.Player.DeleteOptions();

                foreach (var key in keys)
                {
                    // Masukkan deleteOptions ke dalam parameter kedua
                    await Unity.Services.CloudSave.CloudSaveService.Instance.Data.Player.DeleteAsync(key.Key, deleteOptions);
                }
                Debug.Log("[AuthManager] Data Cloud Save sukses disapu bersih.");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[AuthManager] Info saat menghapus Cloud Save: {e.Message}");
            }
            
            // 2. HAPUS AKUN DARI UGS
            await AuthenticationService.Instance.DeleteAccountAsync();

            if (PlayerAccountService.Instance.IsSignedIn)
            {
                PlayerAccountService.Instance.SignOut();
            }

            Debug.Log("[AuthManager] Akun BERHASIL dihapus secara permanen!");

            // 3. BERSIHKAN CACHE LOKAL DI DEVICE
            AuthenticationService.Instance.ClearSessionToken();
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();

            // BERSIHKAN RAM MISSION SYSTEM SAAT HAPUS AKUN
            if (MissionSystem.Instance != null)
            {
                MissionSystem.Instance.ResetSystem();
            }

            if (SceneTransitionManager.Instance != null)
            {
                SceneTransitionManager.Instance.LoadScene(loginSceneName);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[AuthManager] GAGAL menghapus akun. Alasan: {ex.Message}");
        }
    }

    public bool IsGuest()
    {
        if (!AuthenticationService.Instance.IsSignedIn)
            return true;

        return AuthenticationService.Instance.PlayerInfo == null ||
               AuthenticationService.Instance.PlayerInfo.GetUnityId() == null;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            PlayerAccountService.Instance.SignedIn -= HandleUnityPlayerAccountSignedIn;
        }
    }
}