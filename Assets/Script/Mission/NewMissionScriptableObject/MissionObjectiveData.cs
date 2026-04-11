using UnityEngine;

[System.Serializable]
public class MissionObjectiveData
{
    public string objectiveId;
    public string objectiveName;

    [Header("Counter")]
    public bool useCounter;
    public int targetCount = 1;
}