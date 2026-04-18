using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class MissionUIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform missionPanel;
    [SerializeField] private TMP_Text missionTitleText;
    [SerializeField] private TMP_Text missionDescriptionText;

    [Header("Objective UI")]
    [SerializeField] private Transform objectiveContainer;
    [SerializeField] private MissionUIObjective objectivePrefab;

    [Header("Animation")]
    [SerializeField] private float animDuration = 0.5f;
    [SerializeField] private float offscreenRightX = 800f;
    [SerializeField] private float offscreenLeftX = -800f;
    [SerializeField] private float centerX = 0f;

    private readonly List<MissionUIObjective> spawnedObjectives = new List<MissionUIObjective>();

    public void ShowMissionInstant(MissionData mission)
    {
        if (missionPanel == null || mission == null) return;

        missionPanel.anchoredPosition = new Vector2(centerX, missionPanel.anchoredPosition.y);
        RefreshMissionUI(mission);
    }

    public void TransitionToMission(MissionData mission)
    {
        if (missionPanel == null || mission == null) return;

        missionPanel.DOKill();

        missionPanel
            .DOAnchorPosX(offscreenLeftX, animDuration)
            .SetEase(Ease.InCubic)
            .OnComplete(() =>
            {
                RefreshMissionUI(mission);

                missionPanel.anchoredPosition = new Vector2(offscreenRightX, missionPanel.anchoredPosition.y);

                missionPanel
                    .DOAnchorPosX(centerX, animDuration)
                    .SetEase(Ease.OutCubic);
            });
    }

    public void RefreshMissionUI(MissionData mission)
    {
        if (missionTitleText != null)
            missionTitleText.text = mission.missionTitle;

        if (missionDescriptionText != null)
            missionDescriptionText.text = mission.missionDescription;

        ClearObjectives();

        if (mission.objectives == null) return;

        foreach (var obj in mission.objectives)
        {
            MissionUIObjective ui = Instantiate(objectivePrefab, objectiveContainer);

            if (obj.useCounter)
                ui.Setup(obj.objectiveId, obj.objectiveName, false, true, 0, obj.targetCount);
            else
                ui.Setup(obj.objectiveId, obj.objectiveName, false, false, 0, 1);

            spawnedObjectives.Add(ui);
        }
    }

    public void CompleteObjectiveUI(string objectiveId)
    {
        foreach (var ui in spawnedObjectives)
        {
            if (ui.ObjectiveId == objectiveId)
            {
                ui.SetCompleted(true);
                return;
            }
        }
    }

    public void UpdateObjectiveProgressUI(string objectiveId, int current, int target)
    {
        foreach (var ui in spawnedObjectives)
        {
            if (ui.ObjectiveId == objectiveId)
            {
                ui.SetProgress(current, target);
                return;
            }
        }
    }

    private void ClearObjectives()
    {
        // Memaksa hancurkan SEMUA objek yang ada di dalam container
        // (Termasuk prefab dummy yang mungkin kamu buat manual di Editor)
        foreach (Transform child in objectiveContainer)
        {
            Destroy(child.gameObject);
        }

        spawnedObjectives.Clear();
    }
}