using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))] // Memastikan script ini hanya bisa dipasang di objek UI
public class UIFloatingObject : MonoBehaviour
{
    [Header("UI Movement Settings")]
    [Tooltip("Jarak pergerakan naik/turun (dalam nilai pixel Canvas)")]
    public float moveDistance = 15f;     
    
    [Tooltip("Waktu yang dibutuhkan untuk naik/turun")]
    public float duration = 1f;         
    
    [Tooltip("Tambahan ketinggian awal (offset) dalam pixel Canvas")]
    public float heightOffset = 0f;     

    private RectTransform rectTransform;
    private Vector2 startAnchoredPos;

    void Start()
    {
        // 1. Ambil komponen RectTransform (Wajib untuk UI Canvas)
        rectTransform = GetComponent<RectTransform>();

        // 2. Simpan posisi awal (anchoredPosition) saat game dimulai
        startAnchoredPos = rectTransform.anchoredPosition;

        // 3. Hitung posisi dasar Y yang baru dengan menambahkan offset
        float baseY = startAnchoredPos.y + heightOffset;

        // 4. Pindahkan objek UI ke posisi Y yang sudah ditambah offset sebelum animasi
        rectTransform.anchoredPosition = new Vector2(startAnchoredPos.x, baseY);

        // 5. Mulai animasi DOTween khusus untuk UI (DOAnchorPosY)
        rectTransform.DOAnchorPosY(baseY + moveDistance, duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo); // Looping bolak-balik tanpa henti
    }

    // Pastikan animasi dibersihkan ketika objek UI dihancurkan/dimatikan
    private void OnDestroy()
    {
        if (rectTransform != null)
        {
            rectTransform.DOKill();
        }
    }
}