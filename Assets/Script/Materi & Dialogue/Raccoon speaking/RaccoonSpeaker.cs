using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; 

[RequireComponent(typeof(RaccoonAnimator))]
[RequireComponent(typeof(RaccoonSubtitlePlayer))]
[RequireComponent(typeof(AudioSource))]
public class RaccoonSpeaker : MonoBehaviour
{
    [Header("Speech Tracks")]
    public List<RaccoonSubtitleTrack> speechTracks = new List<RaccoonSubtitleTrack>(); 

    [Header("Events")]
    [Tooltip("Event ini akan terpanggil otomatis saat audio dan subtitle selesai dimainkan.")]
    public UnityEvent onSpeechEnd; 

    private RaccoonAnimator _animator;
    private RaccoonSubtitlePlayer _subtitlePlayer;
    private AudioSource _audioSource;
    private Coroutine _speechCoroutine;
    
    // Variabel baru untuk merekam dialog terakhir (Untuk fitur Replay)
    private RaccoonSubtitleTrack _lastPlayedTrack;

    private void Awake()
    {
        _animator       = GetComponent<RaccoonAnimator>();
        _subtitlePlayer = GetComponent<RaccoonSubtitlePlayer>();
        _audioSource    = GetComponent<AudioSource>();
    }

    public void StartSpeech(int index)
    {
        if (index < 0 || index >= speechTracks.Count)
        {
            Debug.LogWarning($"[RaccoonSpeaker] Index {index} out of range.");
            return;
        }
        StartSpeech(speechTracks[index]);
    }

    public void StartSpeech(RaccoonSubtitleTrack track)
    {
        if (track == null || track.voiceClip == null)
        {
            Debug.LogWarning("[RaccoonSpeaker] Track or voiceClip is null.");
            return;
        }

        _lastPlayedTrack = track; // Simpan ke memori untuk di-replay nanti
        StopSpeech();
        
        _speechCoroutine = StartCoroutine(SpeechRoutine(track));
    }

    // FUNGSI BARU: Untuk memutar ulang percakapan terakhir
    public void ReplayLastSpeech()
    {
        if (_lastPlayedTrack != null)
        {
            StartSpeech(_lastPlayedTrack);
        }
    }

    public void StopSpeech()
    {
        if (_speechCoroutine != null)
        {
            StopCoroutine(_speechCoroutine);
            _speechCoroutine = null;
        }

        if (_audioSource.isPlaying) _audioSource.Stop();

        _animator.StopAll();
        _subtitlePlayer.Hide();
    }

    // FUNGSI BARU: Untuk dihubungkan ke Tombol "Skip" di UI
    public void SkipSpeech()
    {
        // Cegah skip jika raccoon sedang tidak berbicara
        if (_speechCoroutine == null) return; 

        // Hentikan paksa audio, animasi, dan subtitle
        StopSpeech();

        Debug.Log("[RaccoonSpeaker] Dialog di-skip!");
        
        // PAKSA panggil event OnSpeechEnd agar Misi tetap selesai/berlanjut!
        onSpeechEnd?.Invoke();
    }

    private IEnumerator SpeechRoutine(RaccoonSubtitleTrack track)
    {
        yield return _animator.PlayBounceIn();

        _audioSource.clip = track.voiceClip;
        _audioSource.Play();

        _animator.StartMouthFlap(_audioSource);
        _subtitlePlayer.Show();

        yield return _subtitlePlayer.RunSubtitles(track, _audioSource);

        yield return new WaitForSeconds(_subtitlePlayer.lingerTime);
        StopSpeech();

        // PANGGIL EVENT SAAT SEMUANYA SELESAI SECARA NORMAL
        onSpeechEnd?.Invoke();
    }
}