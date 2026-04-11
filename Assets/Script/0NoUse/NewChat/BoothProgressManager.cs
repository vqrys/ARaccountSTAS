using UnityEngine;

public class BoothProgressManager : MonoBehaviour
{
    [Header("Target Booth Count")]
    public int requiredBoothCount = 3;

    [Header("Mission Link")]
    public MissionSystem missionSystem;
    public string boothObjectiveId = "booth_materi";

    private int completedBoothCount = 0;

    public void RegisterBoothCompleted()
    {
        if (completedBoothCount >= requiredBoothCount)
            return;

        completedBoothCount++;

        Debug.Log("Booth selesai: " + completedBoothCount + "/" + requiredBoothCount);

        if (missionSystem != null)
        {
            missionSystem.AddObjectiveProgressById(boothObjectiveId, 1);
        }
    }

    public bool AllBoothsCompleted()
    {
        return completedBoothCount >= requiredBoothCount;
    }
}