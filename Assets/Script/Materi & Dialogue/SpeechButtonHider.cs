using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Button))]
public class SpeechButtonHider : MonoBehaviour
{
    [Tooltip("ID Objektif yang akan dicek. Jika sudah tamat, tombol ini akan mati permanen.")]
    public string objectiveIdToCheck;
    
    private Button _button;
    private bool _isPermanentlyDisabled = false; // Penanda (flag) memori lokal

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnButtonClicked);
    }

    private void OnEnable()
    {
        // LAPISAN KEAMANAN 1: Jika sudah ditandai mati, langsung sembunyikan tanpa mengecek sistem lagi.
        if (_isPermanentlyDisabled)
        {
            gameObject.SetActive(false);
            return;
        }

        // LAPISAN KEAMANAN 2: Pengecekan instan tanpa jeda (Coroutine) jika MissionSystem sudah siap.
        if (MissionSystem.Instance != null && MissionSystem.Instance.IsInitialized)
        {
            CheckProgress();
        }
        else
        {
            // Hanya gunakan Coroutine jika sistem benar-benar sedang loading (awal scene)
            StartCoroutine(WaitAndCheck());
        }
    }

    private IEnumerator WaitAndCheck()
    {
        while (MissionSystem.Instance == null || !MissionSystem.Instance.IsInitialized)
        {
            yield return null; 
        }

        CheckProgress();
    }

    public void CheckProgress()
    {
        if (string.IsNullOrEmpty(objectiveIdToCheck)) return;

        if (MissionSystem.Instance.IsObjectiveCompleted(objectiveIdToCheck))
        {
            Debug.Log($"[SpeechButtonHider] Objektif '{objectiveIdToCheck}' sudah selesai. Mematikan tombol '{gameObject.name}' secara permanen.");
            DisableAndHide();
        }
    }

    private void OnButtonClicked()
    {
        // Saat diklik pertama kali, langsung kunci tombolnya agar tidak bisa dispam atau dipanggil ulang
        DisableAndHide();
    }

    // Fungsi utama untuk mengunci dan menyembunyikan tombol
    private void DisableAndHide()
    {
        _isPermanentlyDisabled = true;
        
        // LAPISAN KEAMANAN 3: Matikan fungsi interaksi tombol. 
        // Meskipun objek ini di-SetActive(true) oleh skrip lain, tombol tidak akan merespons klik.
        if (_button != null)
        {
            _button.interactable = false; 
        }
        
        gameObject.SetActive(false);
    }
}