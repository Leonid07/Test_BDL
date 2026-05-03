using UnityEngine;

/// <summary>
/// Represents a tool that can be used on interactables.
/// </summary>
public class ToolItem : HoldableItem, ITool
{
    [Header("Tool")]
    [SerializeField] private ToolType toolType;

    public ToolType ToolType => toolType;
}