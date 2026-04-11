using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NamePopupController : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text warningText;
    [SerializeField] private Button saveButton;
    [SerializeField] private HomeUIController homeUIController;

    [Header("Avatar Selection")]
    [SerializeField] private Button boyButton;
    [SerializeField] private Button girlButton;
    [Tooltip("Gambar outline/highlight untuk menandakan Boy sedang dipilih")]
    [SerializeField] private Image boyHighlight; 
    [Tooltip("Gambar outline/highlight untuk menandakan Girl sedang dipilih")]
    [SerializeField] private Image girlHighlight;

    private string _selectedAvatar = "Boy"; // Default pilihan

    private void Awake()
    {
        if (warningText != null)
        {
            warningText.gameObject.SetActive(false);
        }

        if (saveButton != null)
        {
            saveButton.onClick.AddListener(OnClickSave);
        }

        // Daftarkan event tombol avatar
        if (boyButton != null) boyButton.onClick.AddListener(() => SelectAvatar("Boy"));
        if (girlButton != null) girlButton.onClick.AddListener(() => SelectAvatar("Girl"));

        SelectAvatar("Boy"); // Pilih Boy secara visual saat pertama kali muncul
    }

    private void SelectAvatar(string avatarId)
    {
        _selectedAvatar = avatarId;

        // Nyalakan/matikan highlight sesuai pilihan
        if (boyHighlight != null) boyHighlight.enabled = (avatarId == "Boy");
        if (girlHighlight != null) girlHighlight.enabled = (avatarId == "Girl");
    }

    private async void OnClickSave()
    {
        if (inputField == null)
        {
            ShowWarning("Input field belum di-assign.");
            return;
        }

        if (PlayerDataClient.Instance == null)
        {
            ShowWarning("Player data client tidak ditemukan.");
            return;
        }

        string newName = inputField.text.Trim();

        if (!PlayerDataClient.Instance.IsValidPlayerName(newName))
        {
            ShowWarning("Nama harus 4-16 karakter dan hanya huruf/angka.");
            return;
        }

        try
        {
            // Simpan nama DAN avatar yang dipilih
            await PlayerDataClient.Instance.SaveProfileDataAsync(newName, _selectedAvatar);

            if (homeUIController != null)
            {
                // Update UI Home secara instan
                homeUIController.SetProfile(newName, _selectedAvatar);
                homeUIController.ShowCreateNamePanel(false);
            }

            if (warningText != null)
            {
                warningText.gameObject.SetActive(false);
            }

            gameObject.SetActive(false);
        }
        catch
        {
            ShowWarning("Gagal menyimpan nama. Coba lagi.");
        }
    }

    private void ShowWarning(string message)
    {
        if (warningText != null)
        {
            warningText.gameObject.SetActive(true);
            warningText.text = message;
        }
    }
}