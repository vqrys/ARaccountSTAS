using UnityEngine;
using DG.Tweening;

public class PanelPopupShow : MonoBehaviour
{
    [Header("Popup")]
    public GameObject gameObjectPanel;
    public RectTransform rectPanel;
    
    [Tooltip("Posisi saat panel disembunyikan (di luar layar)")]
    public Vector2 hiddenPosition = new Vector2(0, -1000);
    
    [Tooltip("Posisi saat panel ditampilkan (di tengah layar)")]
    public Vector2 shownPosition = new Vector2(0, 0);

    [Header("Animation")]
    public float duration = 0.35f;
    public Ease openEase = Ease.OutBack;
    public Ease closeEase = Ease.InBack;

    // Untuk mencegah animasi dipanggil berulang kali saat sedang berjalan
    private bool isOpen = false;

    void Start()
    {
        // 1. Pastikan objek aktif
        if (gameObjectPanel != null)
        {
            gameObjectPanel.SetActive(true);
        }

        // 2. Langsung pindahkan ke posisi tersembunyi tanpa animasi saat game dimulai
        rectPanel.anchoredPosition = hiddenPosition;
        
        // 3. Set status panel menjadi tertutup
        isOpen = false;
    }

    public void OpenPopup()
    {
        // Jika sudah terbuka, abaikan perintah ini
        if (isOpen) return;

        // Hentikan animasi DOTween sebelumnya (jika ada) agar tidak bentrok
        rectPanel.DOKill();

        // Animasikan panel bergerak dari posisi saat ini menuju ke shownPosition
        rectPanel.DOAnchorPos(shownPosition, duration).SetEase(openEase);

        // Tandai bahwa panel sekarang terbuka
        isOpen = true;
    }

    public void ClosePopup()
    {
        // Jika sudah tertutup, abaikan perintah ini
        if (!isOpen) return;

        // Hentikan animasi DOTween sebelumnya (jika ada) agar tidak bentrok
        rectPanel.DOKill();

        // Animasikan panel bergerak dari posisi saat ini menuju ke hiddenPosition
        // Perhatikan bahwa kita tidak menggunakan gameObjectPanel.SetActive(false) di sini
        rectPanel.DOAnchorPos(hiddenPosition, duration).SetEase(closeEase);

        // Tandai bahwa panel sekarang tertutup
        isOpen = false;
    }
}