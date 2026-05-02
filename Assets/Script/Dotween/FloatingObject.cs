using UnityEngine;
using DG.Tweening;

public class FloatingObjectAR : MonoBehaviour
{
    [Header("Pengaturan Gerakan")]
    [Tooltip("Gunakan nilai kecil untuk AR, karena 1 unit = 1 meter di dunia nyata")]
    public float moveDistance = 0.2f;     // Seberapa tinggi objek bergerak
    public float duration = 1f;           // Waktu untuk naik/turun

    private Vector3 startLocalPos;

    void Start()
    {
        // 1. Simpan posisi LOKAL awal objek saat game dimulai
        // PENTING: Gunakan localPosition agar posisi dihitung relatif terhadap Image Target (Parent)
        startLocalPos = transform.localPosition;

        // 2. Mulai animasi DOTween
        // Gunakan DOLocalMoveY agar pergerakan tetap mengikuti Image Target saat kamera bergerak
        // Target ketinggiannya adalah: Posisi Y lokal awal ditambah jarak gerakan
        transform.DOLocalMoveY(startLocalPos.y + moveDistance, duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
}