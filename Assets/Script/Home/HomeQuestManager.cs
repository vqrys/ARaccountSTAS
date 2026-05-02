using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

[System.Serializable]
public class QuestUIConfig
{
    public string questName; 
    
    [Header("UI Elements")]
    public Button questButton;
    public Image statusIcon;

    [Header("Quest Settings")]
    [Tooltip("ID Misi terakhir dari quest ini (MissionData). Digunakan untuk mengecek apakah quest INI sudah tamat.")]
    public string finalMissionId;
 
    [Tooltip("Nama scene yang akan di-load saat tombol ditekan (misal: Quest_1_Scene).")]
    public string sceneName;
}

public class HomeQuestManager : MonoBehaviour
{
    [Header("Status Sprites")]
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Sprite unlockedSprite;

    [Header("Tutorial Settings")]
    [Tooltip("ID OBJEKTIF (bukan misi) yang harus selesai sebelum Quest 1 terbuka.")]
    [SerializeField] private string tutorialObjectiveId = "tut_pendahuluan";

    [Header("Daftar Quest (Urutkan dari Quest 1 s/d Quest 7)")]
    [SerializeField] private QuestUIConfig[] questConfigs;

    private async void Start()
    {
        foreach (var quest in questConfigs)
        {
            if (quest.questButton != null) quest.questButton.interactable = false;
        }
        await RefreshQuestUIAsync();
    }

    public void ForceRefreshUI()
    {
        _ = RefreshQuestUIAsync();
    }

    private async Task RefreshQuestUIAsync()
    {
        while (MissionSystem.Instance != null && !MissionSystem.Instance.IsInitialized)
        {
            await Task.Delay(100);
        }

        PlayerProgressData progress = null;
        if (PlayerDataClient.Instance != null)
        {
            progress = await PlayerDataClient.Instance.LoadProgressAsync();
        }

        for (int i = 0; i < questConfigs.Length; i++)
        {
            var config = questConfigs[i];
            if (config == null) continue;

            bool isUnlocked = false;

            if (i == 0)
            {
                if (MissionSystem.Instance != null)
                {
                    isUnlocked = MissionSystem.Instance.IsObjectiveCompleted(tutorialObjectiveId);
                }
                else if (progress != null && progress.completedObjectives != null)
                {
                    isUnlocked = progress.completedObjectives.Contains(tutorialObjectiveId);
                }
            }
            else
            {
                string previousQuestFinalId = questConfigs[i - 1].finalMissionId;
                
                if (!string.IsNullOrEmpty(previousQuestFinalId))
                {
                    // PERBAIKAN: Selalu cek memori lokal dari MissionSystem terlebih dahulu agar Bypass berefek instan!
                    if (MissionSystem.Instance != null)
                    {
                        isUnlocked = MissionSystem.Instance.IsMissionCompleted(previousQuestFinalId);
                    }
                    
                    // Jika di memori lokal belum tamat, baru jadikan hasil unduhan Cloud sebagai backup
                    if (!isUnlocked && progress != null && progress.completedMissions != null)
                    {
                        isUnlocked = progress.completedMissions.Contains(previousQuestFinalId);
                    }
                }
            }

            if (config.questButton != null)
            {
                config.questButton.interactable = isUnlocked;
                config.questButton.onClick.RemoveAllListeners();
                
                string sceneToLoad = config.sceneName;
                config.questButton.onClick.AddListener(() => OnQuestClicked(sceneToLoad));
            }

            if (config.statusIcon != null)
            {
                config.statusIcon.sprite = isUnlocked ? unlockedSprite : lockedSprite;
                config.statusIcon.gameObject.SetActive(true);
            }
        }
    }

    private void OnQuestClicked(string targetSceneName)
    {
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogWarning("[HomeQuestManager] Nama Scene belum diisi di Inspector!");
            return;
        }

        Debug.Log($"[HomeQuestManager] Memuat Scene: {targetSceneName}");
        
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(targetSceneName);
        }
    }
}