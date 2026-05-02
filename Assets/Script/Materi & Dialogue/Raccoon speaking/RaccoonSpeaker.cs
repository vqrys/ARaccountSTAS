using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; 

// --- KELAS UNTUK EVENT SPESIFIK ---
[System.Serializable]
public class SpecificTrackEvent
{
    [Tooltip("Pilih Track spesifik yang ingin dipantau")]
    public RaccoonSubtitleTrack track;
    
    [Tooltip("Event ini HANYA dipanggil saat track MULAI dimainkan (Cocok untuk Trigger Animasi Buka)")]
    public UnityEvent onThisTrackStart;

    [Tooltip("Event ini HANYA dipanggil jika track SELESAI atau DI-SKIP (Cocok untuk Trigger Animasi Tutup)")]
    public UnityEvent onThisTrackEnd;
}

[RequireComponent(typeof(RaccoonAnimator))]
[RequireComponent(typeof(RaccoonSubtitlePlayer))]
[RequireComponent(typeof(AudioSource))]
public class RaccoonSpeaker : MonoBehaviour
{
    [Header("Speech Tracks")]
    public List<RaccoonSubtitleTrack> speechTracks = new List<RaccoonSubtitleTrack>(); 

    [Header("Specific Events (Per-Track)")]
    [Tooltip("Tambahkan event di sini jika kamu ingin sesuatu terjadi HANYA saat track tertentu dimulai atau selesai.")]
    public List<SpecificTrackEvent> specificTrackEvents = new List<SpecificTrackEvent>();

    [Header("Global Events")]
    [Tooltip("Event umum yang terpanggil SETIAP KALI audio apapun MULAI (Cocok untuk menyembunyikan tombol UI).")]
    public UnityEvent onSpeechStart; 

    [Tooltip("Event umum yang terpanggil SETIAP KALI audio apapun SELESAI (Cocok untuk memunculkan Tombol Replay).")]
    public UnityEvent onSpeechEnd; 

    private RaccoonAnimator _animator;
    private RaccoonSubtitlePlayer _subtitlePlayer;
    private AudioSource _audioSource;
    private Coroutine _speechCoroutine;
    
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

        _lastPlayedTrack = track;
        StopSpeech(); 
        
        // PANGGIL EVENT START SPESIFIK (Untuk Animasi Masuk dll)
        TriggerSpecificStartEvents(track);

        // PANGGIL EVENT START GLOBAL
        onSpeechStart?.Invoke();

        _speechCoroutine = StartCoroutine(SpeechRoutine(track));
    }

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

    public void SkipSpeech()
    {
        if (_speechCoroutine == null) return; 

        StopSpeech();
        CompleteTrackMission(_lastPlayedTrack);
        
        // Panggil event END spesifik (Untuk menutup animasi seketika saat di-skip)
        TriggerSpecificEndEvents(_lastPlayedTrack); 
        
        // Panggil event END global
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

        // 1. OTOMATIS TAMATKAN MISI
        CompleteTrackMission(track);

        // 2. PANGGIL EVENT END SPESIFIK (Hanya untuk track ini)
        TriggerSpecificEndEvents(track);

        // 3. PANGGIL EVENT UMUM (Untuk memunculkan Replay dll)
        onSpeechEnd?.Invoke();
    }

    // Fungsi internal untuk lapor ke Mission System
    private void CompleteTrackMission(RaccoonSubtitleTrack track)
    {
        if (track != null && !string.IsNullOrEmpty(track.objectiveIdToComplete))
        {
            if (MissionSystem.Instance != null)
            {
                MissionSystem.Instance.CompleteObjectiveById(track.objectiveIdToComplete);
                Debug.Log($"[RaccoonSpeaker] Otomatis menyelesaikan objektif: {track.objectiveIdToComplete}");
            }
        }
    }

    // Fungsi internal untuk memanggil event START
    private void TriggerSpecificStartEvents(RaccoonSubtitleTrack track)
    {
        if (track == null) return;

        foreach (var specificEvent in specificTrackEvents)
        {
            if (specificEvent.track == track)
            {
                specificEvent.onThisTrackStart?.Invoke();
                Debug.Log($"[RaccoonSpeaker] Memanggil Event Start Spesifik untuk track: {track.name}");
            }
        }
    }

    // Fungsi internal untuk memanggil event END
    private void TriggerSpecificEndEvents(RaccoonSubtitleTrack track)
    {
        if (track == null) return;

        foreach (var specificEvent in specificTrackEvents)
        {
            if (specificEvent.track == track)
            {
                specificEvent.onThisTrackEnd?.Invoke();
                Debug.Log($"[RaccoonSpeaker] Memanggil Event End Spesifik untuk track: {track.name}");
            }
        }
    }
}