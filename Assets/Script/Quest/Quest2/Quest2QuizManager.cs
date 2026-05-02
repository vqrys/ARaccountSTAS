using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Quest2QuizManager : MonoBehaviour
{
    [Header("Daftar Slot")]
    public List<DropSlot> slots = new List<DropSlot>();

    [Header("Tombol Periksa")]
    public Button checkButton;
    public Button continueButton; // Tombol untuk lanjut misi (awalnya mati)

    [Header("Mission Link")]
    public string objectiveIdToComplete = "q2_drag_drop";

    private void Start()
    {
        if (continueButton != null) continueButton.gameObject.SetActive(false);
        checkButton.onClick.AddListener(CheckAnswers);
    }

    public void CheckAnswers()
    {
        bool allCorrect = true;

        foreach (var slot in slots)
        {
            if (slot.CurrentCard == null)
            {
                slot.SetVisualStatus(false, true); // Merah karena kosong
                allCorrect = false;
            }
            else
            {
                bool isMatch = slot.CurrentCard.accountType == slot.acceptedType;
                slot.SetVisualStatus(isMatch, false);
                
                if (!isMatch)
                {
                    allCorrect = false;
                    // Opsional: Tendang kartu yang salah balik ke posisi awal
                    // slot.CurrentCard.ReturnToStart(); 
                }
            }
        }

        if (allCorrect)
        {
            Debug.Log("[Quiz2] Jawaban Sempurna!");
            OnSuccess();
        }
        else
        {
            Debug.Log("[Quiz2] Masih ada yang salah atau kosong.");
        }
    }

    private void OnSuccess()
    {
        checkButton.gameObject.SetActive(false);
        if (continueButton != null) continueButton.gameObject.SetActive(true);

        // Kunci semua kartu agar tidak bisa digeser lagi
        foreach (var slot in slots)
        {
            if (slot.CurrentCard != null) slot.CurrentCard.IsLocked = true;
        }

        // Lapor ke MissionSystem
        if (MissionSystem.Instance != null)
        {
            MissionSystem.Instance.CompleteObjectiveById(objectiveIdToComplete);
        }
    }
}