using UnityEngine;
using DG.Tweening;
using System.Collections;

/// <summary>
/// Contains the SettingsMenu logic
/// </summary>
public class SettingsMenu : MonoBehaviour
{
    /// <summary>
    /// CanvasGroup to manage the SettingsMenu visual state 
    /// </summary>
    [HideInInspector]
    public CanvasGroup canvasGroup;


    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }


    /// <summary>
    /// Show the SettingsMenu
    /// </summary>
    public void Show()
    {
        canvasGroup.DOFade(1f, 0.5f);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }


    /// <summary>
    /// Hide the SettingsMenu
    /// </summary>
    public void Hide()
    {
        StartCoroutine(nameof(HideCoroutine));
    }


    /// <summary>
    /// Return to the TitleScreen
    /// </summary>
    public void Quit()
    {
        FindObjectOfType<GameHandler>().ChangeScene("Assets/Scenes/TitleScreen.unity", 1f);
    }


    private IEnumerator HideCoroutine()
    {
        canvasGroup.DOFade(0, 0.2f);

        yield return new WaitForSecondsRealtime(0.2f);

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
