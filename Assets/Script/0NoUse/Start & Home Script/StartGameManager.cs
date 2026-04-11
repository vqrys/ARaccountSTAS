using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class StartGameManager : MonoBehaviour
{
    public GameObject BG;
    public GameObject PlayUI;
    public GameObject Paper;
    public GameObject confirmYesNo;
    public GameObject confirmRestartYesNo;
    public GameObject Profile;
    public TMP_InputField namaInputField;
    public TMP_Text statusText;
    public TMP_Text namaPlayer;
    private GameHandler gameHandler;
    private Animator mAnimator;

    private string namaPemain;
    private bool isAnimating = false; // Flag to track if animation is running
    public Animator homeButtonAnimator;

    [Header("Fade")]
    public CanvasGroup mainCanvasGroup;

    private void Start()
    {
        mAnimator = GetComponent<Animator>();
        gameHandler = FindObjectOfType<GameHandler>();

        if (homeButtonAnimator != null)
        {
            homeButtonAnimator.SetTrigger("Selected");
        }

        string savedName = gameHandler.LoadPlayerName();

        if (!string.IsNullOrEmpty(savedName))
        {
            namaPlayer.text = savedName;          // Display saved player name
            namaInputField.gameObject.SetActive(false);
            TriggerTrDown();
            statusText.text = "Welcome back!";
            UIManager.instance.HomePage();
            Paper.SetActive(false);

            // **Load mission and tutorial progress**
            LoadMissionAndTutorialProgress();  // Load and hide completed objectives and tutorial
        }
        else
        {
            namaInputField.gameObject.SetActive(true);
            Profile.SetActive(false);
            UIManager.instance.Icons.SetActive(false);
            Paper.SetActive(true);
            mAnimator.SetTrigger("TrDown");
            statusText.text = "Masukkan nama pemain";
        }

        confirmYesNo.SetActive(false);
    }

    private void LoadMissionAndTutorialProgress()
    {
        // Check if tutorial is already completed
        bool tutorialFinished = gameHandler.IsTutorialFinished();  // Use IsTutorialFinished instead
        if (tutorialFinished)
        {
            // Hide elements related to tutorial completion
            UIManager.instance.Mission.SetActive(false); // Hide the mission UI if already completed
            UIManager.instance.DialogVN.SetActive(false); // Hide VN dialogue if completed
            Debug.Log("Tutorial completed. Hiding tutorial UI.");
        }
        else
        {
            // Show mission UI if tutorial is not completed
            UIManager.instance.Mission.SetActive(true);
            UIManager.instance.DialogVN.SetActive(true);
            Debug.Log("Tutorial not completed. Showing mission UI.");
        }

        // You can add more checks here for specific objectives if you need to hide certain UI elements based on mission progress
    }


    public void TriggerTrDown()
    {
        if (isAnimating) return; // Prevent double-triggering animation

        isAnimating = true;
        mAnimator.SetTrigger("TrDown"); // Trigger the "TrDown" animation
        StartCoroutine(TriggerTrUpAfterDelay(3f)); // Wait for 3 seconds before triggering "TrUp"
    }

    public void TriggerTrUp()
    {
        mAnimator.SetTrigger("TrUp");  // Trigger the "TrUp" animation
        isAnimating = false;  // Animation finished, reset the flag
    }

    private IEnumerator TriggerTrUpAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);  // Wait for the specified delay
        TriggerTrUp();  // Call TriggerTrUp() after the delay
    }

    // ===================== BUTTON LOGIC =====================

    public void OnSetujuClicked()
    {
        namaPemain = namaInputField.text;

        if (!string.IsNullOrEmpty(namaPemain))
        {
            mAnimator.SetTrigger("TrDown");
            statusText.text = "Nama pemain: " + namaPemain + ", yakin?";
            confirmYesNo.SetActive(true);
        }
        else
        {
            TriggerTrDown();
            statusText.text = "Nama tidak boleh kosong!";
            confirmYesNo.SetActive(false);
        }
    }

    public void OnKonfirmasiClicked()
    {
        if (!string.IsNullOrEmpty(namaPemain))
        {
            gameHandler.SavePlayerName(namaPemain);

            namaPlayer.text = namaPemain;
            namaInputField.gameObject.SetActive(false);

            TriggerTrDown();
            statusText.text = "Nama disimpan!";
            UIManager.instance.Icons.SetActive(true);
            UIManager.instance.Mission.SetActive(true);
            UIManager.instance.DialogVN.SetActive(true);
            UIManager.instance.raccoon.SetActive(true);
            confirmYesNo.SetActive(false);
        }
    }

    public void OnKonfirmasiNO()
    {
        mAnimator.SetTrigger("TrUp");
        confirmYesNo.SetActive(false);
    }

    public void OnLanjutkanClicked()
    {
        string savedName = gameHandler.LoadPlayerName();

        if (!string.IsNullOrEmpty(savedName))
        {
            namaPlayer.text = savedName;
            TriggerTrDown();
            statusText.text = "Selamat datang kembali, " + savedName;
            confirmYesNo.SetActive(false);
            confirmRestartYesNo.SetActive(false);
            Paper.SetActive(false);
            UIManager.instance.HomePage();
        }
        else
        {
            TriggerTrDown();
            statusText.text = "Nama belum tersimpan!";
            confirmYesNo.SetActive(false);
            confirmRestartYesNo.SetActive(false);
        }   
    }

    public void OnRestartButtonClicked()
    {
        Debug.Log("❓ Restart button clicked");

        mAnimator.SetTrigger("TrDown");
        statusText.text = "Apakah kamu ingin memulai yang baru?";
        confirmYesNo.SetActive(false);
        confirmRestartYesNo.SetActive(true);
    }

    public void OnRestartConfirmYes()
    {
        Debug.Log("🔄 Restart confirmed by user");

        if (gameHandler != null)
        {
            gameHandler.ResetGame();
            Debug.Log("🗑️ Save data deleted successfully");
            UIManager.instance.homePage.SetActive(true);
            UIManager.instance.StartingUI.SetActive(false);
        }
        else
        {
            Debug.LogWarning("⚠️ GameHandler not found!");
        }

        StartCoroutine(RestartWithFade());
    }

    private IEnumerator RestartWithFade()
    {
        yield return StartCoroutine(FadeCanvas(1f, 0f, 0.3f));

        UIManager.instance.Icons.SetActive(false);
        UIManager.instance.Mission.SetActive(false);
        UIManager.instance.DialogVN.SetActive(false);
        Profile.SetActive(false);
        BG.SetActive(true);
        Paper.SetActive(true);

        namaInputField.gameObject.SetActive(true);
        TriggerTrDown();
        statusText.text = "Masukkan nama pemain";

        confirmYesNo.SetActive(false);
        confirmRestartYesNo.SetActive(false);

        Debug.Log("✅ Restart UI reset completed");

        yield return StartCoroutine(FadeCanvas(0f, 1f, 0.5f));
    }

    private IEnumerator FadeCanvas(float from, float to, float duration)
    {
        float elapsed = 0f;
        mainCanvasGroup.alpha = from;
        mainCanvasGroup.blocksRaycasts = false;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            mainCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        mainCanvasGroup.alpha = to;
        mainCanvasGroup.blocksRaycasts = true;
    }
}
