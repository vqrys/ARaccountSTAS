using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RaccoonAnimator : MonoBehaviour
{
    [Header("UI Reference")]
    [Tooltip("Komponen Image UI untuk Raccoon (Pastikan menggunakan Image, bukan RawImage)")]
    public Image raccoonImage;

    [Header("Sprites")]
    [Tooltip("Masukkan 'Raccoon default.jpg' (Mulut Tertutup)")]
    public Sprite defaultSprite;
    
    [Tooltip("Masukkan 'Raccoon speech.jpg' (Mulut Terbuka)")]
    public Sprite speechSprite;

    private Coroutine _speechCoroutine;

    // Fungsi ini dipanggil oleh RaccoonSpeaker sebelum bicara dimulai
    public IEnumerator PlayBounceIn()
    {
        // Dibiarkan kosong (yield break) agar tidak ada animasi melompat yang kaku.
        // Tapi fungsi ini tetap dipertahankan supaya script RaccoonSpeaker kamu tidak error.
        yield break;
    }

    // Dipanggil oleh RaccoonSpeaker saat audio mulai
    public void StartMouthFlap(AudioSource audioSource)
    {
        StopMouthFlap(); // Hentikan proses sebelumnya jika Raccoon ditekan ulang secara cepat
        _speechCoroutine = StartCoroutine(SpeechRoutine(audioSource));
    }

    private IEnumerator SpeechRoutine(AudioSource audioSource)
    {
        // 1. Saat mulai bicara: Ganti ke gambar mulut terbuka
        if (raccoonImage != null && speechSprite != null)
        {
            raccoonImage.sprite = speechSprite;
        }

        // 2. Tunggu secara diam-diam sampai audio selesai diputar
        yield return new WaitWhile(() => audioSource.isPlaying);

        // 3. Setelah audio selesai: Kembalikan ke gambar mulut tertutup
        SetDefaultSprite();
    }

    // Dipanggil saat bicara di-skip atau dihentikan paksa
    public void StopMouthFlap()
    {
        if (_speechCoroutine != null)
        {
            StopCoroutine(_speechCoroutine);
            _speechCoroutine = null;
        }
        SetDefaultSprite();
    }

    // Fungsi bawaan yang diakses oleh RaccoonSpeaker untuk memberhentikan semuanya
    public void StopAll()
    {
        StopMouthFlap();
    }

    // Fungsi kecil untuk mengembalikan sprite ke default
    private void SetDefaultSprite()
    {
        if (raccoonImage != null && defaultSprite != null)
        {
            raccoonImage.sprite = defaultSprite;
        }
    }
}