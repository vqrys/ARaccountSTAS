using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening; 

[RequireComponent(typeof(CanvasGroup))]
public class DragCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Data Kartu")]
    public string accountType; // Contoh: "Aset", "Kewajiban", atau "Ekuitas"

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas mainCanvas;
    
    // Variabel untuk mengembalikan kartu ke tempat asal
    private Transform startParent;
    private int startIndex; 

    public bool IsLocked { get; set; } = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        mainCanvas = GetComponentInParent<Canvas>();
        
        // Simpan rumah awal kartu beserta urutannya
        startParent = transform.parent;
        startIndex = transform.GetSiblingIndex();
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
    }
}