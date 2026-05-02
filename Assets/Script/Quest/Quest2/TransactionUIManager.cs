using UnityEngine;
using TMPro;
using DG.Tweening;

public class TransactionUIManager : MonoBehaviour
{
    [Header("Referensi UI Angka")]
    public TMP_Text textKas;
    public TMP_Text textUtang;
    public TMP_Text textModal;
    public TMP_Text textStatus;

    // Contoh fungsi untuk menggulirkan angka secara halus
    public void AnimateNumber(TMP_Text textElement, int startValue, int targetValue, float duration)
    {
        // DOTween akan mengubah nilai x dari startValue ke targetValue dalam waktu 'duration'
        DOTween.To(() => startValue, x => 
        {
            startValue = x;
            // Format angka dengan titik ribuan (contoh: 10.000.000)
            textElement.text = "Rp " + startValue.ToString("N0"); 
        }, targetValue, duration).SetEase(Ease.OutCubic);
    }

    // Fungsi ini dipanggil dari event RaccoonSpeaker saat Contoh 1 dimulai
    public void PlayAnimasiContoh1()
    {
        AnimateNumber(textKas, 0, 10000000, 2f);
        AnimateNumber(textModal, 0, 10000000, 2f);
        
        textStatus.text = "TETAP SEIMBANG";
        textStatus.color = Color.green;
    }

    // Fungsi ini dipanggil dari event RaccoonSpeaker saat Contoh 2 dimulai
    public void PlayAnimasiContoh2()
    {
        AnimateNumber(textKas, 10000000, 15000000, 2f);
        AnimateNumber(textUtang, 0, 5000000, 2f);
        
        textStatus.text = "TETAP SEIMBANG";
        textStatus.color = Color.green;
    }
}