using UnityEngine;
using DG.Tweening;

public class FloatingObject : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveDistance = 2f;     // Seberapa tinggi objek bergerak
    public float duration = 1f;         // Waktu untuk naik/turun
    // public float heightOffset = 0f;     // Tambahan ketinggian awal (offset)

    private Vector3 startPos;

    void Start()
    {
        // 1. Simpan posisi awal objek saat game dimulai
        startPos = transform.position;

        // 2. Hitung posisi dasar Y yang baru dengan menambahkan offset
        // float baseY = startPos.y + heightOffset;

        // 3. Pindahkan objek ke posisi Y yang sudah ditambah offset sebelum animasi dimulai
        transform.position = new Vector3(startPos.x, startPos.z);

        // 4. Mulai animasi DOTween
        // Objek akan bergerak dari baseY menuju (baseY + moveDistance)
        transform.DOMoveY(moveDistance, duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
}