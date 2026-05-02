using UnityEngine;
using DG.Tweening; // Menggunakan DOTween untuk rotasi yang halus

public class NPCFaceCamera : MonoBehaviour
{
    [Header("Pengaturan Rotasi")]
    [Tooltip("Waktu yang dibutuhkan NPC untuk berputar (detik)")]
    public float rotationDuration = 0.5f;

    [Tooltip("Pilih tipe kemulusan animasi putaran")]
    public Ease rotationEase = Ease.OutQuad;

    [Tooltip("Ubah angka ini (misal: 90, -90, atau 180) jika NPC malah menghadap ke samping/belakang")]
    public float yOffsetAngle = 0f;

    private Quaternion _startRotation;

    private void Start()
    {
        // Simpan arah rotasi asli NPC saat game baru dimulai
        _startRotation = transform.rotation;
    }

    /// <summary>
    /// Panggil fungsi ini dari OnLookEnter di ARLookable
    /// </summary>
    public void TurnToCamera()
    {
        if (Camera.main == null)
        {
            Debug.LogWarning("[NPCFaceCamera] Main Camera tidak ditemukan!");
            return;
        }

        // 1. Cari arah dari NPC ke Kamera
        Vector3 directionToCamera = Camera.main.transform.position - transform.position;

        // 2. KUNCI SUMBU Y: Hilangkan perbedaan ketinggian (Y) agar tidak mendongak/menunduk
        directionToCamera.y = 0;

        if (directionToCamera != Vector3.zero)
        {
            // 3. Hitung target rotasi dasar agar menghadap kamera
            Quaternion lookRotation = Quaternion.LookRotation(directionToCamera);

            // 4. Tambahkan offset sudut untuk mengoreksi model 3D yang "Offside"
            lookRotation *= Quaternion.Euler(0, yOffsetAngle, 0);

            // 5. Putar dengan animasi halus
            transform.DORotateQuaternion(lookRotation, rotationDuration).SetEase(rotationEase);
        }
    }

    /// <summary>
    /// Panggil fungsi ini dari OnLookExit di ARLookable
    /// </summary>
    public void ResetRotation()
    {
        // Mengembalikan NPC ke rotasi awal seperti sebelum ditatap
        transform.DORotateQuaternion(_startRotation, rotationDuration).SetEase(rotationEase);
    }
}