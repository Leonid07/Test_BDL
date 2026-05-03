using UnityEngine;

/// <summary>
/// Represents an item that can be picked up, held, dropped, and thrown by the player.
/// </summary>
public interface IHoldableItem
{
    /// <summary>
    /// Gets whether the item is currently held by the player.
    /// </summary>
    bool IsHeld { get; set; }

    /// <summary>
    /// Gets whether the item is currently available for pickup.
    /// </summary>
    bool IsAvailableForPickup { get; set; }

    /// <summary>
    /// Makes the item available for pickup.
    /// </summary>
    void MakeAvailableForPickup();

    /// <summary>
    /// Picks up the item and attaches it to the specified hold point.
    /// </summary>
    void PickUp(Transform holdPoint);

    /// <summary>
    /// Drops the item with the specified force.
    /// </summary>
    void Drop(Vector3 force);
}