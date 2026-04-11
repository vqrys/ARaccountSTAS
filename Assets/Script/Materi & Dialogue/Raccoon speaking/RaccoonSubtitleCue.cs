using UnityEngine;

/// <summary>
/// A single timed subtitle entry.
/// 'time' is seconds since the audio clip started.
/// </summary>
[System.Serializable]
public class RaccoonSubtitleCue
{
    [Min(0f)]
    public float time;

    [TextArea(2, 4)]
    public string text;
}