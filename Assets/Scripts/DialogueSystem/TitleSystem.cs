using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class TitleSystem : MonoBehaviour
{
    // The title text colors for the game's characters
    private static Dictionary<string, Color32> TitleTextColors = new Dictionary<string, Color32>()
    {
        {"Helen", new Color32(255, 73, 87, 0)},
        {"Evan", new Color32(102, 214, 140, 0)},
    };

    private Text titleText;


    // Must be Awake instead of Start because this GameObject will be disabled by the DialogueContainer by the time Start is called
    private void Awake()
    {
        titleText = GetComponentInChildren<Text>();

        // Resetting the active state solves a visibility bug somehow. Don't change it.
        titleText.gameObject.SetActive(false);
        titleText.gameObject.SetActive(true);

        titleText.color = new Color(255, 255, 255, 0);
        titleText.text = string.Empty;
    }


    public void SetTitleText(string text)
    {
        string[] keys = text.Split(new char[] { ':' }, 2);

        if (keys.Length == 1)
        {
            if (titleText.color.a != 0)
            {
                DOTween.Pause(titleText);
                Hide();
            }
        }
        else
        {
            Show();

            string[] characterKeys = keys[0].Split(new char[] { ',' });

            string speakerName = characterKeys[0].Trim();

            // Set the title text on the dialogue box
            titleText.text = $"{speakerName}:";

            // Set the title text color based on the character
            if (TitleTextColors.TryGetValue(speakerName, out Color32 titleTextColor))
            {
                titleText.CrossFadeColor(titleTextColor, 0, true, false);
            }
            else
            {
                titleText.CrossFadeColor(Color.white, 0, true, false);
            }
        }
    }

    public void Show()
    {
        DOTween.ToAlpha(() => titleText.color, x => titleText.color = x, 1f, 0.5f).SetTarget(titleText);
    }

    public void Hide()
    {
        titleText.DOFade(0, 0);
    }
}
