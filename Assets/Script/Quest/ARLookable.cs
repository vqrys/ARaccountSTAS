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

        // Cek menggunakan referensi _missionSystem yang sudah ditemukan di Start()
        if (_missionSystem != null && _missionSystem.CurrentMission != null)
        {
            return _missionSystem.CurrentMission.missionId != requiredMissionId;
        }
        
        return true; // Jika MissionSystem belum siap, kunci dulu untuk amannya
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