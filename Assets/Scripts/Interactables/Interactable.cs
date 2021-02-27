using UnityEngine;

/// <summary>
/// Base Interactable class used for objects and characters
/// </summary>
public abstract class Interactable : MonoBehaviour
{
    /// <summary>
    /// The script for the Interactable
    /// </summary>
    public YarnProgram script;

    /// <summary>
    /// The starting node of the script
    /// </summary>
    public string startNode = "Start";
}
