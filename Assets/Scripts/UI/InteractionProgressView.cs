using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays progress for hold-based interactions.
/// </summary>
public class InteractionProgressView : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private Image progressFill;

    /// <summary>
    /// Shows the progress bar.
    /// </summary>
    public void Show()
    {
        root.SetActive(true);
        SetProgress(0f);
    }

    /// <summary>
    /// Hides the progress bar.
    /// </summary>
    public void Hide()
    {
        root.SetActive(false);
    }

    /// <summary>
    /// Updates the progress bar fill amount.
    /// </summary>
    public void SetProgress(float value)
    {
        progressFill.fillAmount = Mathf.Clamp01(value);
    }
}