using UnityEngine;
using UnityEngine.Events;

public class ARLookable : MonoBehaviour
{
    [Header("Mission Lock (Opsional)")]
    [Tooltip("Isi dengan ID Misi (contoh: Mission_02_Materi). Objek ini HANYA bisa di-scan jika misi tersebut sedang aktif.")]
    public string requiredMissionId;

    [Header("Event Per Object")]
    public UnityEvent onLookEnter;
    public UnityEvent onLookExit;

    private bool _isCurrentlyLooked = false;
    private MissionSystem _missionSystem; // Variabel untuk menyimpan MissionSystem

    private void Start()
    {
        // Mencari MissionSystem di dalam scene secara otomatis saat game dimulai
        _missionSystem = FindObjectOfType<MissionSystem>();
    }

    // Fungsi untuk mengecek apakah objek ini sedang dikunci oleh sistem Misi
     private bool IsLockedByMission()
    {
        // Jika dikosongkan, berarti objek ini bebas di-scan kapan saja
        if (string.IsNullOrEmpty(requiredMissionId)) return false; 

        if (_missionSystem != null)
        {
            // BUKA KUNCI JIKA: Misi saat ini adalah misi yang diminta (sedang dikerjakan)
            if (_missionSystem.CurrentMission != null && _missionSystem.CurrentMission.missionId == requiredMissionId)
            {
                return false;
            }

            // BUKA KUNCI JIKA: Misi tersebut sudah pernah diselesaikan (sudah lewat / progress lanjut)
            if (_missionSystem.IsMissionCompleted(requiredMissionId))
            {
                return false;
            }
        }
        
        // Jika misi belum tercapai atau MissionSystem belum siap, kunci objeknya
        return true; 
    }

    public void OnLookEnter()
    {
        if (IsLockedByMission()) return; // Abaikan tatapan (Raycast) jika misi belum sesuai
        
        _isCurrentlyLooked = true;
        onLookEnter.Invoke();
    }

    public void OnLookExit()
    {
        if (!_isCurrentlyLooked) return; // Hanya panggil Exit jika sebelumnya berhasil Enter

        _isCurrentlyLooked = false;
        onLookExit.Invoke();
    }
}