using System.Collections.Generic;
using UnityEngine;

public class SubtitleTrack : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip voiceClip;

    [Header("Subtitle Text")]
    public List<SubtitleCue> cues = new List<SubtitleCue>();
}