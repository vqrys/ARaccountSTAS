using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSubtitleTrack", menuName = "Raccoon/Subtitle Track")]
public class RaccoonSubtitleTrack : ScriptableObject
{
    [Header("Audio")]
    public AudioClip voiceClip;

    [Header("Mission Integration (Opsional)")]
    [Tooltip("Isi dengan ID Objektif (misal: tut_pendahuluan). Jika diisi, objektif ini akan otomatis tamat saat Raccoon selesai bicara!")]
    public string objectiveIdToComplete;

    [Header("Subtitle Cues")]
    public List<RaccoonSubtitleCue> cues = new List<RaccoonSubtitleCue>(); 
}