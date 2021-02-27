using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Contains the logic for managing game CGs
/// </summary>
[ExecuteInEditMode]
public class CgHandler : MonoBehaviour
{
    #region Properties
    /// <summary>
    /// The index of the BlackScreen sprite
    /// </summary>
    public const string BlackScreenSpriteName = "black_screen";

    /// <summary>
    /// The CGs for the scene
    /// </summary>
    public Dictionary<string, Image> sprites = new Dictionary<string, Image>();

    /// <summary>
    /// The color to modulate children CGs
    /// </summary>
    public Color modulation;

    /// <summary>
    /// The currently showing CG's index
    /// </summary>
    public string currentCgName;

    /// <summary>
    /// The CG stack
    /// </summary>
    public List<string> cgStack = new List<string>();
    #endregion


    private void Awake()
    {
        foreach (var image in GetComponentsInChildren<Image>())
        {
            // Only store first level sprites children
            if (image.transform.parent == transform)
            {
                sprites.Add(image.transform.name, image);

                // Hide the CG and modulate it based on the specified modulation
                if (modulation != Color.white)
                {
                    image.color = new Color(modulation.r, modulation.g, modulation.b, 0);
                }
                else
                {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
                }
            }
        }

        if (Application.IsPlaying(gameObject))
        {
            // Bring the black screen sprite to front
            sprites[BlackScreenSpriteName].color = Color.white;
            sprites[BlackScreenSpriteName].transform.SetAsLastSibling();
        }
    }


    #region Methods
    /// <summary>
    /// Hide the current CG and decrement the CG index
    /// </summary>
    /// <param name="time">The time it takes for the previous CG to fully show</param>
    public void ShowPreviousCg(float time = 1f)
    {
        if (cgStack.Count() == 1)
        {
            Debug.LogError("Cannot backtrack on the first CG.", this);
            return;
        }

        Debug.Log("Returning to the previous CG");

        // Pop the current CG off the stack
        cgStack.RemoveAt(cgStack.Count() - 1);

        // Show the CG
        SetCg(cgStack.ElementAt(cgStack.Count() - 1), time);

        // Also pop the modified CG stack to keep it as is
        cgStack.RemoveAt(cgStack.Count() - 1);
    }

    /// <summary>
    /// Show the CG with the specified name
    /// </summary>
    /// <param name="index">The CG's name</param>
    /// <param name="time">The time it takes for the CG to fully show</param>
    public void SetCg(string name, float time = 1.5f)
    {
        name = name.ToLower();

        // Helper for my crappy keyboard that can't type the underscore character
        if (name == "blackscreen" || name == "black_screen") name = CgHandler.BlackScreenSpriteName;

        if (!sprites.Keys.Contains(name))
        {
            Debug.LogError($"The CG with the name <b>{name}</b> cannot be found", this);
            return;
        }

        // Complete any running tweens
        DOTween.Complete(transform);

        Image cg = transform.Find(name).GetComponent<Image>();

        // Add the CG to the top of the stack
        cgStack.Add(name);

        // Set the color
        if (modulation == Color.white)
        {
            cg.color = new Color(cg.color.r, cg.color.g, cg.color.b, 0);
        }
        else
        {
            cg.color = new Color(modulation.r, modulation.g, modulation.b, 0);
        }

        // Bring the CG to front
        cg.transform.SetAsLastSibling();

        // Fade in the CG
        cg.DOFade(1f, time).SetTarget(transform);

        // Also fade in any child sprites
        foreach (var child in cg.GetComponentsInChildren<Image>())
        {
            child.DOFade(1f, time).SetTarget(transform);
        }

        currentCgName = name;
    }

    public async Task ToggleTint()
    {
        Debug.Log("Tint the screen");

        var blackScreen = transform.Find(BlackScreenSpriteName).GetComponent<Image>();

        // Complete any running tint job
        DOTween.Complete(blackScreen);

        if (blackScreen.color.a == 0.8f)
        {
            // Untint

            blackScreen.DOFade(0, 1f).SetTarget(blackScreen);
            await Task.Delay(1000);

            blackScreen.GetComponent<Canvas>().sortingOrder = 0;

            blackScreen.transform.SetAsFirstSibling();
        }
        else
        {
            // Tint

            blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 0f);

            blackScreen.GetComponent<Canvas>().sortingOrder = 999;

            // Wait a bit
            await Task.Delay(1000);

            blackScreen.DOFade(0.8f, 1f).SetTarget(blackScreen);
        }
    }
    #endregion
}
