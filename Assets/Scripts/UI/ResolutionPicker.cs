using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages game resolution
/// </summary>
public class ResolutionPicker : MonoBehaviour
{
    /// <summary>
    /// The current game resolution
    /// </summary>
    public static int currentResolutionIndex = 2;

    // The resolutions to support
    private Vector2[] resolutions;


    void Start()
    {
        // Initialize the supported resolutions
        resolutions = new Vector2[] {
            new Vector2(1920, 1080),
            new Vector2(1366, 768),
            new Vector2(1024, 576) 
        };

        var dropdown = GetComponent<Dropdown>();

        // Populate the dropdown list
        dropdown.options = new List<Dropdown.OptionData>() {
            new Dropdown.OptionData() { text = "1920x1080" },
            new Dropdown.OptionData() { text = "1366x768" },
            new Dropdown.OptionData() { text = "1024x576" },
        };

        // Set the default resolution
        dropdown.value = currentResolutionIndex;

        dropdown.onValueChanged.AddListener(index => 
        {
            if (index < resolutions.Length)
            {
                Screen.SetResolution(
                    Mathf.RoundToInt(resolutions[index].x), 
                    Mathf.RoundToInt(resolutions[index].y), 
                    Screen.fullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);

                currentResolutionIndex = index;
            }
        });
    }
}
