using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RaccoonSubtitlePlayer : MonoBehaviour
{
    [Header("UI References — assign from within this prefab")]
    public TMP_Text subtitleText;
    public GameObject subtitlePanel;

    [Header("Timing")]
    [Tooltip("Seconds the last subtitle stays visible after audio ends.")]
    public float lingerTime = 1.2f;

    public void Show()
    {
        if (subtitlePanel != null) subtitlePanel.SetActive(true);
        if (subtitleText  != null) subtitleText.text = "";
    }

    public void Hide()
    {
        if (subtitlePanel != null) subtitlePanel.SetActive(false);
        if (subtitleText  != null) subtitleText.text = "";
    }

    // Tipe datanya diperbaiki jadi RaccoonSubtitleTrack
    public IEnumerator RunSubtitles(RaccoonSubtitleTrack track, AudioSource audioSource)
    {
        var cues  = BuildSortedCues(track);
        int index = 0;

        while (audioSource.isPlaying)
        {
            float t = audioSource.time;

            while (index < cues.Count && t >= cues[index].time)
            {
                if (subtitleText != null)
                    subtitleText.text = cues[index].text;
                index++;
            }

            yield return null;
        }

        if (index < cues.Count && subtitleText != null)
            subtitleText.text = cues[cues.Count - 1].text;
    }

    // Tipe datanya diperbaiki jadi RaccoonSubtitleTrack dan RaccoonSubtitleCue
    private List<RaccoonSubtitleCue> BuildSortedCues(RaccoonSubtitleTrack track)
    {
        var copy = new List<RaccoonSubtitleCue>(track.cues);
        copy.Sort((a, b) => a.time.CompareTo(b.time));
        return copy;
    }
}