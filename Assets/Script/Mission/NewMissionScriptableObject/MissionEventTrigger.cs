using UnityEngine;

public class MissionEventTrigger : MonoBehaviour
{
    [SerializeField] private MissionSystem missionSystem;
    [SerializeField] private string objectiveId;

    public void TriggerObjective()
    {
        if (missionSystem == null)
        {
            Debug.LogWarning("MissionSystem belum di-assign.");
            return;
        }

        missionSystem.CompleteObjectiveById(objectiveId);
    }
}