using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

/// <summary>
/// Yarn Command used to toggle VFXs through the public event parameters in the editor
/// </summary>
public class ToggleVfxCommand : MonoBehaviour
{
    // Public events for the editor to manage the implementation
    public UnityEvent onVfxAboutToActivate;
    public UnityEvent onVfxAboutToDeactivate;

    [YarnCommand("vfxActivate")]
    /// <summary>
    /// Activate the VFX
    /// </summary>
    public void ActivateVfx()
    {
        onVfxAboutToActivate?.Invoke();
    }

    [YarnCommand("vfxDeactivate")]
    /// <summary>
    /// Deactivate the VFX
    /// </summary>
    public void DeactivateVfx()
    {
        onVfxAboutToDeactivate?.Invoke();
    }
}
