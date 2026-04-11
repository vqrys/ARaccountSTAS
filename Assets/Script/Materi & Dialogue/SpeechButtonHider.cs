using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpeechButtonHider : MonoBehaviour
{
    public string objectiveIdToCheck;
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(HideButton);
    }

    private void OnEnable()
    {
        StartCoroutine(WaitAndCheck());
    }

    private IEnumerator WaitAndCheck()
    {
        // TUNGGU: Selama MissionSystem belum ada atau belum selesai download data...
        while (MissionSystem.Instance == null || !MissionSystem.Instance.IsInitialized)
        {
            yield return null; // Tunggu ke frame berikutnya
        }

        // Jika sudah sampai sini, berarti data Cloud sudah pasti masuk ke memori
        CheckProgress();
    }

    public void CheckProgress()
    {
        if (string.IsNullOrEmpty(objectiveIdToCheck)) return;

        if (MissionSystem.Instance.IsObjectiveCompleted(objectiveIdToCheck))
        {
            Debug.Log($"[SpeechButtonHider] {gameObject.name} disembunyikan karena {objectiveIdToCheck} sudah tamat.");
            gameObject.SetActive(false);
        }
    }

    private void HideButton()
    {
        gameObject.SetActive(false);
    }
}
// ```

// ### Mengapa ini terjadi? (Visualisasi Alur)
// Pahami urutan kejadian di bawah ini untuk melihat di mana letak kesalahannya sebelumnya:



// ```json?chameleon
// {
//   "component": "LlmGeneratedComponent",
//   "props": {
//     "height": "400px",
//     "prompt": "Create a sequence flow visualizer for Unity Cloud Save initialization. \n1. Process A (MissionSystem): Starts Loading -> Awaiting Cloud Response (1-2s) -> Finish Loading.\n2. Process B (UI Button): Start -> Wait 0.2s -> Check Progress -> Data Not Ready (Fail).\n3. Goal: Show that Process B must wait for Process A to reach 'Finish' state. \nUse labels like 'Cloud Service', 'MissionSystem', and 'UI Button'. Distinguish between the 'Old Way' (Timer) and 'New Way' (Status Check)."
//   }
// }