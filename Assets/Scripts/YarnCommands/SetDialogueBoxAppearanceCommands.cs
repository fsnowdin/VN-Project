using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

/// <summary>
/// Yarn commands to manage the dialogue box's visual state
/// </summary>
public class SetDialogueBoxAppearanceCommands : MonoBehaviour
{
    // Possible positions for the dialogue box
    private readonly static List<string> _positionList = new List<string>()
    {
        "center",
        "bottom",
        "top"
    };

    private GameHandler gameHandler;
    private DialogueUI _dialogueUI;
    private DialogueRunner _dialogueRunner;

    private bool _screenTinted = false;

    private void Start()
    {
        gameHandler = GetComponent<GameHandler>();
        _dialogueUI = gameHandler.dialogueUI;
        _dialogueRunner = gameHandler.dialogueRunner;

        _dialogueRunner.AddCommandHandler("showBox", ShowDialogueBox);
        _dialogueRunner.AddCommandHandler("hideBox", HideDialogueBox);
        _dialogueRunner.AddCommandHandler("setBoxPosition", SetDialogueBoxPosition);
    }

    #region Commands
    public void ShowDialogueBox(string[] _param)
    {
        _dialogueUI.Show();
    }

    public void HideDialogueBox(string[] _param)
    {
        _dialogueUI.Hide();
    }

    public void SetDialogueBoxPosition(string[] param)
    {
        if (param.Length != 1)
        {
            Debug.LogError("Invalid parameter for a <b>setBoxPosition</b> command");
        }

        string position = param[0].Trim().ToLower();

        if (!_positionList.Contains(position))
        {
            Debug.LogError($"Could not found input position {position} to use for a setBoxPosition command");
        }

        _dialogueUI.SetPosition(position);
    }
    #endregion
}
