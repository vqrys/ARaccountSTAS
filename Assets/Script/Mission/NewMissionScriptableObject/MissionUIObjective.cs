using UnityEngine;
using TMPro;

public class MissionUIObjective : MonoBehaviour
{
    [SerializeField] private TMP_Text objectiveText;
    [SerializeField] private GameObject completedIcon;

    private string objectiveId;
    private string objectiveName;
    private bool completed;

    private bool useCounter;
    private int currentCount;
    private int targetCount;

    public string ObjectiveId => objectiveId;
    public bool Completed => completed;

    public void Setup(string id, string name, bool isCompleted = false, bool counterMode = false, int current = 0, int target = 1)
    {
        objectiveId = id;
        objectiveName = name;
        completed = isCompleted;

        useCounter = counterMode;
        currentCount = current;
        targetCount = target;

        Refresh();
    }

    public void SetCompleted(bool value)
    {
        completed = value;
        Refresh();
    }

    public void SetProgress(int current, int target)
    {
        useCounter = true;
        currentCount = current;
        targetCount = target;
        completed = currentCount >= targetCount;
        Refresh();
    }

    public void Refresh()
    {
        if (objectiveText != null)
        {
            if (useCounter)
                objectiveText.text = $"{objectiveName} ({currentCount}/{targetCount})";
            else
                objectiveText.text = objectiveName;
        }

        if (completedIcon != null)
            completedIcon.SetActive(completed);
    }
}