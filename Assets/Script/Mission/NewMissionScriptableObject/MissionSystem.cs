using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement; 
using Unity.Services.Authentication; 

public class MissionSystem : MonoBehaviour
{
    public static MissionSystem Instance { get; private set; }

    [Header("Mission Flow")]
    [SerializeField] private MissionData startingMission;
    
    private MissionUIController missionUI;
    private RaccoonSpeaker mentorSpeaker; 

    [Header("Mission Database (Untuk Resume)")]
    [SerializeField] private MissionData[] missionDatabase;

    [Header("Events")]
    public UnityEvent onMissionCompleted;
    public UnityEvent onAllMissionsCompleted;

    private MissionData currentMission;
    private PlayerProgressData playerProgress = new PlayerProgressData();
    private Dictionary<string, int> objectiveProgress = new Dictionary<string, int>();

    private bool _isInitialized = false;
    public bool IsInitialized => _isInitialized; 

    public MissionData CurrentMission => currentMission;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); 

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        missionUI = FindObjectOfType<MissionUIController>();
        mentorSpeaker = FindObjectOfType<RaccoonSpeaker>();

        if (_isInitialized && currentMission != null && missionUI != null)
        {
            missionUI.ShowMissionInstant(currentMission);

            if (currentMission.objectives != null)
            {
                foreach (var obj in currentMission.objectives)
                {
                    if (playerProgress.completedObjectives.Contains(obj.objectiveId))
                    {
                        missionUI.CompleteObjectiveUI(obj.objectiveId);
                    }
                    else if (obj.useCounter && objectiveProgress.ContainsKey(obj.objectiveId))
                    {
                        missionUI.UpdateObjectiveProgressUI(obj.objectiveId, objectiveProgress[obj.objectiveId], obj.targetCount);
                    }
                }
            }
        }
    }

    public void ResetSystem()
    {
        _isInitialized = false;
        playerProgress = new PlayerProgressData();
        objectiveProgress.Clear();
        currentMission = null;
        Debug.Log("[MissionSystem] Memori lokal berhasil di-reset (Kosong).");
    }

    public async void InitializeSystemAsync()
    {
        if (_isInitialized) return; 

        while (!UGSBootstrap.IsInitialized || !AuthenticationService.Instance.IsSignedIn)
        {
            await System.Threading.Tasks.Task.Delay(100); 
        }

        if (PlayerDataClient.Instance != null)
        {
            playerProgress = await PlayerDataClient.Instance.LoadProgressAsync();
            Debug.Log("[MissionSystem] Progres berhasil dimuat dari Cloud setelah Login Siap.");
        }

        // Pastikan memori tidak Null agar aman setelah reset akun
        if (playerProgress == null) playerProgress = new PlayerProgressData();
        if (playerProgress.completedMissions == null) playerProgress.completedMissions = new List<string>();
        if (playerProgress.completedObjectives == null) playerProgress.completedObjectives = new List<string>();

        // ==========================================================
        // 🛠️ FITUR BARU: AUTO-BYPASS SAAT LOAD GAME
        // ==========================================================
        if (!string.IsNullOrEmpty(adminMissionIdToBypass))
        {
            bool needsSave = false;
            
            // 1. Tamatkan Misi Bypass secara otomatis
            if (!playerProgress.completedMissions.Contains(adminMissionIdToBypass))
            {
                playerProgress.completedMissions.Add(adminMissionIdToBypass);
                needsSave = true;
            }
            
            // 2. Tamatkan tutorial awal agar UI Home Quest 1 tidak terkunci
            if (!playerProgress.completedObjectives.Contains("tut_pendahuluan"))
            {
                playerProgress.completedObjectives.Add("tut_pendahuluan");
                needsSave = true;
            }

            // 3. Lompat langsung menjadikan ini misi target saat ini
            playerProgress.currentMissionId = adminMissionIdToBypass;
            needsSave = true;

            // Simpan perubahan ke Cloud
            if (needsSave && PlayerDataClient.Instance != null)
            {
                await PlayerDataClient.Instance.SaveProgressAsync(playerProgress);
            }

            // Refresh UI Home agar gembok Quest Terbuka
            HomeQuestManager homeManager = FindObjectOfType<HomeQuestManager>();
            if (homeManager != null) homeManager.ForceRefreshUI();

            Debug.Log($"[Admin Tool] Auto-Bypass diaktifkan! Melompat ke: {adminMissionIdToBypass}");
        }
        // ==========================================================

        MissionData missionToLoad = startingMission;

        if (playerProgress != null && !string.IsNullOrEmpty(playerProgress.currentMissionId))
        {
            foreach (var mission in missionDatabase)
            {
                if (mission.missionId == playerProgress.currentMissionId)
                {
                    missionToLoad = mission;
                    break;
                }
            }
        }

        _isInitialized = true; 
        StartMission(missionToLoad, true);
    }

    void Start()
    {
        InitializeSystemAsync();
    }

    public void StartMission(MissionData mission, bool instantUI = false)
    {
        if (mission == null) return;

        currentMission = mission;
        
        if (playerProgress == null) playerProgress = new PlayerProgressData();
        
        playerProgress.currentMissionId = currentMission.missionId;
        objectiveProgress.Clear();

        // TAHAP 1: Konstruksi Antarmuka Pengguna (UI)
        // Fungsi ini harus dijalankan terlebih dahulu untuk menciptakan prefab objektif di layar.
        if (missionUI != null)
        {
            if (instantUI) missionUI.ShowMissionInstant(mission);
            else missionUI.TransitionToMission(mission);
        }

        // TAHAP 2: Pembaruan Status Visual (Ceklis)
        // Setelah elemen UI tersedia, sistem baru dapat menyisir data yang sudah dimuat dari Cloud.
        if (currentMission.objectives != null)
        {
            foreach (var obj in currentMission.objectives)
            {
                if (obj.useCounter) objectiveProgress[obj.objectiveId] = 0;
                
                // Verifikasi apakah ID objektif ada dalam daftar progres yang dimuat dari Cloud Save.
                if (playerProgress.completedObjectives.Contains(obj.objectiveId) && missionUI != null)
                {
                    // Eksekusi pembaruan visual ceklis pada UI.
                    missionUI.CompleteObjectiveUI(obj.objectiveId);
                }
            }
        }

        Debug.Log("Mission started: " + mission.missionTitle);
    }

    public async void CompleteObjectiveById(string objectiveId)
    {
        if (currentMission == null || string.IsNullOrEmpty(objectiveId)) return;
        if (playerProgress.completedObjectives.Contains(objectiveId)) return;

        MissionObjectiveData objectiveData = GetObjectiveData(objectiveId);
        if (objectiveData == null) return;

        playerProgress.completedObjectives.Add(objectiveId);
        if (missionUI != null) missionUI.CompleteObjectiveUI(objectiveId);

        if (PlayerDataClient.Instance != null)
            await PlayerDataClient.Instance.SaveProgressAsync(playerProgress);

        if (IsCurrentMissionCompleted()) CompleteCurrentMission();
    }

    public async void AddObjectiveProgressById(string objectiveId, int amount = 1)
    {
        if (currentMission == null) return;

        MissionObjectiveData objectiveData = GetObjectiveData(objectiveId);
        if (objectiveData == null || !objectiveData.useCounter) return;

        if (playerProgress.completedObjectives.Contains(objectiveId)) return;

        if (!objectiveProgress.ContainsKey(objectiveId))
            objectiveProgress[objectiveId] = 0;

        objectiveProgress[objectiveId] += amount;
        
        if (objectiveProgress[objectiveId] > objectiveData.targetCount)
            objectiveProgress[objectiveId] = objectiveData.targetCount;

        if (missionUI != null)
            missionUI.UpdateObjectiveProgressUI(objectiveId, objectiveProgress[objectiveId], objectiveData.targetCount);

        if (objectiveProgress[objectiveId] >= objectiveData.targetCount)
        {
            playerProgress.completedObjectives.Add(objectiveId);

            if (missionUI != null)
                missionUI.CompleteObjectiveUI(objectiveId);

            if (PlayerDataClient.Instance != null)
            {
                await PlayerDataClient.Instance.SaveProgressAsync(playerProgress);
            }

            if (IsCurrentMissionCompleted())
            {
                CompleteCurrentMission();
            }
        }
    }

    public bool IsObjectiveCompleted(string objectiveId)
    {
        return playerProgress.completedObjectives.Contains(objectiveId);
    }

    public bool IsCurrentMissionCompleted()
    {
        if (currentMission == null || currentMission.objectives == null) return false;
        foreach (var obj in currentMission.objectives)
        {
            if (!playerProgress.completedObjectives.Contains(obj.objectiveId)) return false;
        }
        return true;
    }

    // FUNGSI BARU: Mengecek apakah sebuah misi sudah pernah diselesaikan sebelumnya
    public bool IsMissionCompleted(string missionId)
    {
        return playerProgress.completedMissions.Contains(missionId);
    }

    private MissionObjectiveData GetObjectiveData(string objectiveId)
    {
        if (currentMission == null || currentMission.objectives == null) return null;
        foreach (var obj in currentMission.objectives)
        {
            if (obj.objectiveId == objectiveId) return obj;
        }
        return null;
    }

    private async void CompleteCurrentMission()
    {
        if (!playerProgress.completedMissions.Contains(currentMission.missionId))
        {
            playerProgress.completedMissions.Add(currentMission.missionId);
        }

        onMissionCompleted?.Invoke();

        if (currentMission.nextMission != null)
        {
            StartMission(currentMission.nextMission, false);
        }
        else
        {
            playerProgress.currentMissionId = ""; 
            onAllMissionsCompleted?.Invoke();
        }

        if (PlayerDataClient.Instance != null)
        {
            await PlayerDataClient.Instance.SaveProgressAsync(playerProgress);
        }
    }

    // =================================================================================
    // 🛠️ ADMIN / DEVELOPER TOOLS (Untuk Tes Cepat)
    // =================================================================================

    [Header("🛠️ Admin & Testing Tools")]
    [Tooltip("Ketik ID misi yang ingin ditamatkan instan (Misal: Mission_05_Quest1JenisPerusahaan)")]
    public string adminMissionIdToBypass = "Mission_05_Quest1JenisPerusahaan";

    [ContextMenu("🛠️ ADMIN: Tamatkan Misi Ini (Bypass)")]
    public async void AdminBypassMission()
    {
        if (string.IsNullOrEmpty(adminMissionIdToBypass)) return;

        // 1. CEGAH ERROR NULL SETELAH HAPUS AKUN DENGAN MEMBUAT LIST BARU
        if (playerProgress == null) playerProgress = new PlayerProgressData();
        if (playerProgress.completedMissions == null) playerProgress.completedMissions = new List<string>();
        if (playerProgress.completedObjectives == null) playerProgress.completedObjectives = new List<string>();

        bool needsSave = false;

        // 2. Tamatkan misi yang diminta
        if (!playerProgress.completedMissions.Contains(adminMissionIdToBypass))
        {
            playerProgress.completedMissions.Add(adminMissionIdToBypass);
            needsSave = true;
        }

        // 3. Wajib tamatkan tutorial awal agar tombol Quest 1 juga ikut terbuka!
        if (!playerProgress.completedObjectives.Contains("tut_pendahuluan"))
        {
            playerProgress.completedObjectives.Add("tut_pendahuluan");
            needsSave = true;
        }

        if (needsSave)
        {
            Debug.Log($"[Admin Tool] Memproses Bypass untuk: {adminMissionIdToBypass}...");

            if (PlayerDataClient.Instance != null)
            {
                await PlayerDataClient.Instance.SaveProgressAsync(playerProgress);
                await System.Threading.Tasks.Task.Delay(500); // Beri waktu sebentar agar cloud sinkron
            }

            // Paksa UI Home refresh gemboknya
            HomeQuestManager homeManager = FindObjectOfType<HomeQuestManager>();
            if (homeManager != null) homeManager.ForceRefreshUI();
            
            Debug.Log($"[Admin Tool] Bypass SUKSES!");
        }
        else
        {
            Debug.Log($"[Admin Tool] Misi '{adminMissionIdToBypass}' SUDAH TAMAT sebelumnya.");
        }
    }

    [ContextMenu("🛠️ ADMIN: Hapus Seluruh Progres (Reset)")]
    public async void AdminResetAllProgress()
    {
        // Pastikan List dibuat baru agar tidak error
        playerProgress = new PlayerProgressData();
        playerProgress.completedMissions = new List<string>();
        playerProgress.completedObjectives = new List<string>();
        
        if (PlayerDataClient.Instance != null)
        {
            await PlayerDataClient.Instance.SaveProgressAsync(playerProgress);
        }

        HomeQuestManager homeManager = FindObjectOfType<HomeQuestManager>();
        if (homeManager != null) homeManager.ForceRefreshUI();

        Debug.Log("[Admin Tool] SELURUH PROGRES TELAH DIHAPUS!");
    }
}