using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Controls the current repair stage and allows objects to validate interaction availability.
/// </summary>
public class RepairStageController : MonoBehaviour
{
    [SerializeField] private RepairStage currentStage;
    [Header("UI")]
    [SerializeField] private TMP_Text stageText;

    [Header("Stage Descriptions")]
    [TextArea]
    [SerializeField] private List<string> stageDescriptions;
    /// <summary>
    /// Gets the current repair stage.
    /// </summary>
    public RepairStage CurrentStage => currentStage;

    private void Start()
    {
        UpdateStageText();
    }

    /// <summary>
    /// Moves to the next stage in the enum.
    /// </summary>
    public void NextStage()
    {
        currentStage++;

        if ((int)currentStage >= System.Enum.GetValues(typeof(RepairStage)).Length)
            currentStage = RepairStage.Complete;

        Debug.Log($"Stage changed to: {currentStage}");
        UpdateStageText();
    }

    /// <summary>
    /// Checks if the current stage matches the required stage.
    /// </summary>
    public bool IsStage(RepairStage stage)
    {
        return currentStage == stage;
    }
    /// <summary>
    /// Updates UI text based on current stage.
    /// </summary>
    private void UpdateStageText()
    {
        if (stageText == null) return;

        int index = (int)currentStage;

        if (stageDescriptions != null && index < stageDescriptions.Count)
        {
            stageText.text = stageDescriptions[index];
        }
        else
        {
            stageText.text = currentStage.ToString();
        }
    }
    /// <summary>
    /// Allows setting custom text manually.
    /// </summary>
    public void SetCustomText(string text)
    {
        if (stageText != null)
            stageText.text = text;
    }
}