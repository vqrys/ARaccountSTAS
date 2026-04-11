using UnityEngine;

/// <summary>
/// Skrip jembatan ini bisa ditaruh di mana saja (Tombol, Raccoon, Objek AR).
/// Berfungsi untuk memanggil MissionSystem Global lewat Unity Events di Inspector.
/// </summary>
public class MissionTrigger : MonoBehaviour
{
    [Tooltip("Panggil fungsi ini dari Inspector (Button/Event) lalu ketik ID misinya.")]
    
    // Fungsi untuk menamatkan objektif biasa (misal: "tut_pendahuluan" atau "tut_quest")
    public void CompleteObjective(string objectiveId)
    {
        if (MissionSystem.Instance != null)
        {
            MissionSystem.Instance.CompleteObjectiveById(objectiveId);
            Debug.Log($"[MissionTrigger] Mengirim sinyal misi selesai: {objectiveId}");
        }
        else
        {
            Debug.LogWarning($"[MissionTrigger] Gagal menyelesaikan '{objectiveId}'. MissionSystem tidak ditemukan!");
        }
    }

    // Fungsi untuk menambah progres (misal: untuk kunjungan 3 Booth "q1_booth_materi")
    public void AddObjectiveProgress(string objectiveId)
    {
        if (MissionSystem.Instance != null)
        {
            MissionSystem.Instance.AddObjectiveProgressById(objectiveId, 1);
            Debug.Log($"[MissionTrigger] Menambah progres misi: {objectiveId}");
        }
        else
        {
            Debug.LogWarning($"[MissionTrigger] Gagal menambah progres '{objectiveId}'. MissionSystem tidak ditemukan!");
        }
    }
}