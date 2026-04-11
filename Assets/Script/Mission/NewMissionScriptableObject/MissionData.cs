using UnityEngine;

[CreateAssetMenu(fileName = "NewMission", menuName = "Mission System/Mission Data")]
public class MissionData : ScriptableObject
{
    [Header("Identitas Misi")]
    public string missionId;
    public string missionTitle;

    [TextArea(2, 4)]
    public string missionDescription;

    [Header("Dialog Mentor (Opsional)")]
    [Tooltip("Masukkan SubtitleTrack mentor untuk misi ini. Biarkan kosong jika tidak ada dialog.")]
    public RaccoonSubtitleTrack mentorDialogue; // Referensi ke sistem dialog milikmu!

    [Header("Objektif & Lanjutan")]
    public MissionObjectiveData[] objectives;
    public MissionData nextMission;
}