using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contains the logic for the button to toggle the game's fullscreen state
/// </summary>
public class FullscreenButton : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => 
        {
            Screen.SetResolution(
                Screen.currentResolution.width, 
                Screen.currentResolution.height, 
                Screen.fullScreenMode == FullScreenMode.FullScreenWindow ? FullScreenMode.Windowed : FullScreenMode.FullScreenWindow);
        });
    }
}
