using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class DropSlot : MonoBehaviour, IDropHandler
{
    [Header("Identitas Slot")]
    public string acceptedType; // Isi: "Aset", "Kewajiban", atau "Ekuitas"
    
    [Header("Visual Feedback")]
    [Tooltip("Masukkan target gambar yang akan berubah warna (Hijau/Merah). Bisa child/objek lain.")]
    public Image feedbackImage;

    [Tooltip("(OPSIONAL) Masukkan objek kosong/panel sebagai tempat kartu menempel. Jika dikosongkan, kartu akan menempel tepat di objek DropSlot ini.")]
    public Transform cardContainer;

    [Header("Pengaturan Skala Kartu Drop")]
    [Tooltip("Ubah ukuran skala kartu saat berhasil masuk ke slot ini. Default (1, 1, 1)")]
    public Vector3 droppedCardScale = Vector3.one;

    [Header("Event Drop")]
    [Tooltip("Daftar event yang akan dijalankan saat kartu BERHASIL MASUK ke slot ini.")]
    public UnityEvent onCardDropped;
    
    [Tooltip("Daftar event yang akan dijalankan saat kartu DILEPAS/DITARIK KELUAR dari slot ini.")]
    public UnityEvent onCardRemoved; // Variabel baru untuk event saat dilepas

    private Color _originalColor;
    public DragCard CurrentCard { get; private set; }

    private void Awake()
    {
        if (feedbackImage == null) feedbackImage = GetComponent<Image>();
        
        if (feedbackImage != null) _originalColor = feedbackImage.color;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            DragCard card = eventData.pointerDrag.GetComponent<DragCard>();

            if (card != null && CurrentCard == null)
            {
                CurrentCard = card;
                
                Transform targetParent = cardContainer != null ? cardContainer : this.transform;
                card.transform.SetParent(targetParent, true);
                
                RectTransform cardRect = card.GetComponent<RectTransform>();
                
                cardRect.anchorMin = new Vector2(0.5f, 0.5f);
                cardRect.anchorMax = new Vector2(0.5f, 0.5f);
                cardRect.pivot = new Vector2(0.5f, 0.5f);

                cardRect.DOAnchorPos(Vector2.zero, 0.25f).SetEase(Ease.OutBack);
                cardRect.DOScale(droppedCardScale, 0.25f).SetEase(Ease.OutBack); 

                card.SetDroppedState(true);

                // PANGGIL EVENT SAAT KARTU MASUK
                onCardDropped?.Invoke();
            }
        }
    }

    private void Update()
    {
        // Tentukan tempat menempel yang seharusnya (Container atau DropSlot itu sendiri)
        Transform targetParent = cardContainer != null ? cardContainer : this.transform;

        // Jika kartu sudah ditarik keluar (parent-nya berubah)
        if (CurrentCard != null && CurrentCard.transform.parent != targetParent)
        {
            CurrentCard = null;
            ResetVisual(); 
            
            // PANGGIL EVENT SAAT KARTU DILEPAS
            onCardRemoved?.Invoke();
        }
    }

    public void SetVisualStatus(bool isCorrect, bool isEmpty)
    {
        if (feedbackImage == null) return;

        if (isEmpty) 
        {
            feedbackImage.color = new Color(0.8f, 0.2f, 0.2f, 1f); 
        }
        else 
        {
            feedbackImage.color = isCorrect ? new Color(0.2f, 0.8f, 0.2f, 1f) : new Color(0.8f, 0.2f, 0.2f, 1f);
        }
    }

    public void ResetVisual()
    {
        if (feedbackImage != null)
        {
            feedbackImage.color = _originalColor;
        }
    }
}