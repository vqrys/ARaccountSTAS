using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement; 
using Unity.Services.Authentication; // TAMBAHAN PENTING UNTUK CEK LOGIN

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
    public bool IsInitialized => _isInitialized; // Properti untuk dicek oleh skrip lain

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

    async void Start()
    {
        if (_isInitialized) return; 

        // --- PERBAIKAN KRUSIAL: TUNGGU SAMPAI PLAYER SELESAI LOGIN ---
        while (!UGSBootstrap.IsInitialized || !AuthenticationService.Instance.IsSignedIn)
        {
            // Menunggu 0.1 detik berulang kali sampai sistem Login selesai (saat Loading Screen)
            await System.Threading.Tasks.Task.Delay(100); 
        }

        if (PlayerDataClient.Instance != null)
        {
            playerProgress = await PlayerDataClient.Instance.LoadProgressAsync();
            Debug.Log("[MissionSystem] Progres berhasil dimuat dari Cloud setelah Login Siap.");
        }

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

        _isInitialized = true; // Tandai bahwa MissionSystem sudah 100% siap dipakai skrip lain!
        StartMission(missionToLoad, true);
    }

    public void StartMission(MissionData mission, bool instantUI = false)
    {
        if (mission == null) return;

        currentMission = mission;
        
        if (playerProgress == null) playerProgress = new PlayerProgressData();
        
        playerProgress.currentMissionId = currentMission.missionId;
        objectiveProgress.Clear();

        if (currentMission.objectives != null)
        {
            foreach (var obj in currentMission.objectives)
            {
                if (obj.useCounter) objectiveProgress[obj.objectiveId] = 0;
                
                if (playerProgress.completedObjectives.Contains(obj.objectiveId) && instantUI && missionUI != null)
                {
                    missionUI.CompleteObjectiveUI(obj.objectiveId);
                }
            }
        }

        if (missionUI != null)
        {
            if (instantUI) missionUI.ShowMissionInstant(mission);
            else missionUI.TransitionToMission(mission);
        }

        if (currentMission.mentorDialogue != null && mentorSpeaker != null)
        {
            mentorSpeaker.StartSpeech(currentMission.mentorDialogue);
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
}