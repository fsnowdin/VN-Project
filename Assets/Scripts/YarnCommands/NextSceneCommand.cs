using System.Globalization;
using UnityEngine;
using Yarn.Unity;

/// <summary>
/// Yarn command to advance to the next scene
/// </summary>
public class NextSceneCommand : MonoBehaviour
{
    private DialogueRunner dialogueRunner;
    private GameHandler gameHandler;

    void Start()
    {
        // Init references
        gameHandler = GetComponent<GameHandler>();
        dialogueRunner = gameHandler.dialogueRunner;

        // Add the command
        dialogueRunner.AddCommandHandler("nextScene", NextScene);
    }


    public void NextScene(string[] param)
    {
        if (param.Length != 2)
        {
            Debug.LogError($"A nextScene command received the wrong number of parameters. The parameters were {param}");
            return;
        }

        try
        {
            var sceneToChangeTo = param[0];
            var time = float.Parse(param[1], CultureInfo.InvariantCulture.NumberFormat);

            gameHandler.ChangeScene(sceneToChangeTo, time);
        }
        catch
        {
            Debug.LogError($"A nextScene command just failed. Its parameters were {param[0]} and {param[1]}. Unpause the game and fix it.");
        }
    }
}
