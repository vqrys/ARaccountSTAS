using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class DropSlot : MonoBehaviour, IDropHandler
{
    [Header("Identitas Slot")]
    public string acceptedType; // Isi: "Aset", "Kewajiban", atau "Ekuitas"
    
    [Header("Visual Feedback")]
    [Tooltip("Masukkan target gambar yang akan berubah warna (Hijau/Merah). Bisa child/objek lain.")]
    public Image feedbackImage;

    private Color _originalColor;
    public DragCard CurrentCard { get; private set; }

    private void Awake()
    {
        // Jika feedbackImage tidak diisi manual di inspector, coba cari di objek ini sendiri
        if (feedbackImage == null) feedbackImage = GetComponent<Image>();
        
        // Simpan warna aslinya agar bisa di-reset kembali normal saat kartu ditarik
        if (feedbackImage != null) _originalColor = feedbackImage.color;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            DragCard card = eventData.pointerDrag.GetComponent<DragCard>();

            // Hanya terima kartu jika slot sedang kosong
            if (card != null && CurrentCard == null)
            {
                CurrentCard = card;
                
                // Jadikan kartu ini sebagai Child dari Slot Drop
                card.transform.SetParent(this.transform, true);
                
                RectTransform cardRect = card.GetComponent<RectTransform>();
                
                // PENTING: Paksa Anchor Kartu menjadi Center (Tengah)
                // Ini mencegah kartu melenceng jauh saat snap
                cardRect.anchorMin = new Vector2(0.5f, 0.5f);
                cardRect.anchorMax = new Vector2(0.5f, 0.5f);
                cardRect.pivot = new Vector2(0.5f, 0.5f);

                // Animasi halus bergerak tepat ke tengah slot
                cardRect.DOAnchorPos(Vector2.zero, 0.25f).SetEase(Ease.OutBack);
                cardRect.localScale = Vector3.one; 
            }
        }
    }

    private void Update()
    {
        // Jika pemain berubah pikiran dan menarik kembali kartu keluar dari slot ini
        if (CurrentCard != null && CurrentCard.transform.parent != this.transform)
        {
            CurrentCard = null;
            ResetVisual(); // Kembalikan warna ke awal (bukan merah/hijau lagi)
        }
    }

    // Fungsi dipanggil oleh Quest2QuizManager saat menekan "Periksa"
    public void SetVisualStatus(bool isCorrect, bool isEmpty)
    {
        if (feedbackImage == null) return;

        if (isEmpty) 
        {
            feedbackImage.color = new Color(0.8f, 0.2f, 0.2f, 1f); // Merah jika belum diisi
        }
        else 
        {
            // Hijau terang jika benar, Merah jika salah
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