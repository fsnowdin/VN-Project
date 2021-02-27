using System.Collections;
using System.Globalization;
using System.Linq;
using UnityEngine;
using Yarn.Unity;

/// <summary>
/// Yarn commands to manage CG flow
/// </summary>
public class CgFlowCommands : MonoBehaviour
{
    private readonly float _defaultTime = 1.5f;

    private DialogueRunner dialogueRunner;
    private GameHandler gameHandler;


    private void Start()
    {
        // Init references
        gameHandler = GetComponent<GameHandler>();
        dialogueRunner = gameHandler.dialogueRunner;

        // Add the Yarn commands
        dialogueRunner.AddCommandHandler("cgPrevious", PreviousCg);
        dialogueRunner.AddCommandHandler("cgSet", SetCg);

        dialogueRunner.AddCommandHandler("cgFadeBlackToNext", FadeBlackToNextCg);
    }


    /// <summary>
    /// Show the previous CG
    /// </summary>
    /// <param name="param">The time it takes for the CG to fully show</param>
    public void PreviousCg(string[] param)
    {
        if (param != null)
        {
            if (param.Length != 1) return;
        }

        Debug.Log("Returning to the previous CG");

        try
        {
            float time = param != null ? float.Parse(param[0], CultureInfo.InvariantCulture.NumberFormat) : _defaultTime;
            gameHandler.cgHandler.ShowPreviousCg(time);
        }
        catch
        {
            Debug.LogError($"A cgPrevious command failed. Its parameter was {param[0]}. Unpause the game and fix it.");
        }
    }

    /// <summary>
    /// Show the CG with the specified index
    /// </summary>
    /// <param name="param">The index of the CG to show (Optional: The time it takes for the CG to fully show)</param>
    public void SetCg(string[] param)
    {
        if (param.Length == 0 || param.Length > 2) return;

        Debug.Log("Setting a new CG");

        try
        {
            string name = param[0];
            float time = param.Length == 2 ? float.Parse(param[1], CultureInfo.InvariantCulture.NumberFormat) : _defaultTime;

            gameHandler.cgHandler.SetCg(name, time);
        }
        catch
        {
            Debug.LogError($"A cgSet command failed. Its parameters were {param[0]} and {param[1]}. Unpause the game and fix it.");
        }
    }

    /// <summary>
    /// Smoothly fade to the next CG using existing Yarn commands
    /// </summary>
    public void FadeBlackToNextCg(string[] param, System.Action onComplete)
    {
        if (param.Length != 1)
        {
            Debug.LogError($"Not enough parameters were given for a cgFadeBlackToNext command.");
            return;
        }

        Debug.Log("Fade black to a new CG");

        StartCoroutine(FadeBlackToNextCgCoroutine(param[0].ToLower().Trim(), onComplete));
    }

    private IEnumerator FadeBlackToNextCgCoroutine(string name, System.Action onComplete)
    {
        gameHandler.dialogueUI.Hide();

        // Fade in the black CG
        gameHandler.cgHandler.SetCg(CgHandler.BlackScreenSpriteName, 0.5f);

        yield return new WaitForSecondsRealtime(1f);

        // Don't store the black screen CG in the CG stack
        gameHandler.cgHandler.cgStack.RemoveAt(gameHandler.cgHandler.cgStack.Count() - 1);

        // Show the specified CG
        gameHandler.cgHandler.SetCg(name, 1.5f);

        yield return new WaitForSecondsRealtime(1f);

        gameHandler.dialogueUI.Show();

        onComplete();
    }
}
