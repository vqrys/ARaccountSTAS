using UnityEngine;

[System.Serializable]
public class SubtitleCue
{
    [Min(0f)]
    public float time;   // detik sejak audio mulai

    [TextArea(2, 4)]
    public string text;  // teks popup
}