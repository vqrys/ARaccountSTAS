using UnityEngine;
using System.Collections;

public class UIObjectiveListener : MonoBehaviour
{
    [Tooltip("ID Objektif yang harus tamat agar UI ini muncul (Misal: materi_mentor_selesai)")]
    public string requiredObjectiveId;

    [Tooltip("Apakah UI ini harus disembunyikan saat game dimulai?")]
    public bool hideOnStart = true;

    private void Start()
    {
        if (hideOnStart)
        {
            gameObject.SetActive(false);
        }

        // Mulai memantau MissionSystem di latar belakang
        StartCoroutine(MonitorObjectiveRoutine());
    }

    private IEnumerator MonitorObjectiveRoutine()
    {
        // Tunggu sampai Mission System siap
        while (MissionSystem.Instance == null || !MissionSystem.Instance.IsInitialized)
        {
            yield return new WaitForSeconds(0.5f);
        }

        // Cek terus menerus setiap 0.5 detik apakah objektifnya sudah tamat
        while (true)
        {
            if (MissionSystem.Instance.IsObjectiveCompleted(requiredObjectiveId))
            {
                // Jika tamat, munculkan UI ini dan hentikan pemantauan!
                gameObject.SetActive(true);
                yield break; 
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}