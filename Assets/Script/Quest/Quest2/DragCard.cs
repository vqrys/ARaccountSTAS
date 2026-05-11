using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening; 

[RequireComponent(typeof(CanvasGroup))]
public class DragCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Data Kartu")]
    public string accountType; // Contoh: "Aset", "Kewajiban", atau "Ekuitas"

    [Header("Pengaturan Visual & Skala")]
    [Tooltip("Ukuran skala kartu saat berada di dalam daftar (Panel Bawah)")]
    public Vector3 panelScale = Vector3.one;
    [Tooltip("Ukuran skala kartu saat sedang diseret (Drag)")]
    public Vector3 dragScale = Vector3.one;
    
    [Tooltip("Centang jika ingin menyembunyikan gambar background kartu saat masuk ke slot")]
    public bool hideImageOnDrop = false;
    [Tooltip("Masukkan komponen Image (Background) dari kartu ini")]
    public Image backgroundImage;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas mainCanvas;
    
    // Variabel untuk mengembalikan kartu ke tempat asal
    private Transform startParent;
    private int startIndex; 
    private Color _originalImageColor;

    public bool IsLocked { get; set; } = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        mainCanvas = GetComponentInParent<Canvas>();
        
        // Simpan rumah awal kartu beserta urutannya
        startParent = transform.parent;
        startIndex = transform.GetSiblingIndex();

        // Simpan warna asli background jika ada
        if (backgroundImage != null)
        {
            _originalImageColor = backgroundImage.color;
        }

        // Set skala awal agar sesuai dengan pengaturan di inspector
        rectTransform.localScale = panelScale;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (IsLocked) return;

        // PENTING: Pindahkan kartu langsung ke Canvas Utama agar bebas bergerak 
        // dan tidak dibatasi oleh Horizontal Layout Group dari CardPanel
        transform.SetParent(mainCanvas.transform, true);
        transform.SetAsLastSibling(); // Pastikan kartu selalu di paling atas layar

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        // Ubah skala ke ukuran drag agar terlihat normal saat ditarik
        rectTransform.DOScale(dragScale, 0.15f);

        // Jika sebelumnya sempat di-drop lalu ditarik lagi, kembalikan warna gambar
        SetDroppedState(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (IsLocked) return;
        
        // Mengikuti posisi jari/mouse secara presisi di kanvas
        rectTransform.anchoredPosition += eventData.delta / mainCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsLocked) return;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // JIKA SETELAH DILEPAS PARENTNYA MASIH CANVAS (Tidak dijatuhkan ke DropSlot)
        // Maka kartu akan kembali ke posisinya semula
        if (transform.parent == mainCanvas.transform)
        {
            ReturnToStart();
        }
    }

    public void ReturnToStart()
    {
        // Kembalikan ke CardPanel (Horizontal Layout Group akan otomatis merapikannya)
        transform.SetParent(startParent, false);
        transform.SetSiblingIndex(startIndex); // Pastikan urutannya kembali seperti semula
        
        // Kembalikan skala ke ukuran panel (Offset scale)
        rectTransform.DOScale(panelScale, 0.25f);
        
        // Kembalikan warna ke semula (jika tadinya disembunyikan)
        SetDroppedState(false);
    }

    // Fungsi dipanggil oleh DropSlot saat kartu masuk
    public void SetDroppedState(bool isDropped)
    {
        if (hideImageOnDrop && backgroundImage != null)
        {
            Color c = _originalImageColor;
            c.a = isDropped ? 0f : _originalImageColor.a;
            backgroundImage.color = c;
        }
    }
}