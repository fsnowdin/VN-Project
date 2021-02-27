using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

/// <summary>
/// Contains logic for scenes
/// </summary>
public class GameHandler : MonoBehaviour
{
    /// <summary>
    /// CG manager class
    /// </summary>
    [HideInInspector]
    public CgHandler cgHandler;

    /// <summary>
    /// The script to start the scene with
    /// </summary>
    public YarnProgram startingScript;

    [HideInInspector]
    public string startingScriptNode = "Start";

    /// <summary>
    /// The dialogue system's logic handler
    /// </summary>
    [HideInInspector]
    public DialogueRunner dialogueRunner;

    /// <summary>
    /// The dialogue system's UI handler
    /// </summary>
    [HideInInspector]
    public DialogueUI dialogueUI;

    /// <summary>
    /// The settings menu
    /// </summary>
    [HideInInspector]
    public SettingsMenu settingsMenu;

    /// <summary>
    /// Whether to start the scene automatically
    /// </summary>
    public bool autoStartScene = true;

    /// <summary>
    /// The music for the scene
    /// </summary>
    public List<AudioClip> songs = new List<AudioClip>();
    public int currentSongIndex = 0;
    public bool playMusicOnStart = true;


    private void Awake()
    {
        cgHandler = GetComponentInChildren<CgHandler>();

        dialogueRunner = GetComponentInChildren<DialogueRunner>();
        dialogueUI = GetComponentInChildren<DialogueUI>();

        settingsMenu = GetComponentInChildren<SettingsMenu>();

        if (autoStartScene) StartScene();
    }

    // The main game loop
    private void Update()
    {
        if (dialogueRunner.IsDialogueRunning && !settingsMenu.canvasGroup.interactable)
        {
            #if UNITY_STANDALONE || UNITY_WEBGL
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    DialogueUI.isSkipModeEnabled = true;
                    dialogueUI.MarkLineComplete();
                }
                else
                {
                    DialogueUI.isSkipModeEnabled = false;

                    if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Return))
                    {
                        // Advance the dialogue
                        dialogueUI.MarkLineComplete();
                        return;
                    }

                    if (Input.GetKeyUp(KeyCode.F3))
                    {
                        dialogueUI.ToggleAutoMode();
                    }
                }
            #elif UNITY_ANDROID || UNITY_IOS
                if (Input.GetMouseButtonUp(0))
                {
                    // Advance the dialogue
                    dialogueUI.MarkLineComplete();
                    return;
                }
            #endif
        }

        // Open/Hide the Settings menu
        if (Input.GetKeyUp(KeyCode.Escape)) // Escape also doubles as the Back button on mobile
        {
            if (!settingsMenu.canvasGroup.interactable)
            {
                settingsMenu.Show();
            }
            else
            {
                settingsMenu.Hide();
            }
        }
    }

    /// <summary>
    /// Start a dialogue
    /// </summary>
    /// <param name="script">The script to use for the dialogue</param>
    /// <param name="startNode">The starting node of the script</param>
    public void Talk(YarnProgram script, string startNode)
    {
        dialogueRunner.Add(script);
        dialogueRunner.StartDialogue(startNode);
    }

    /// <summary>
    /// Change to a new scene
    /// </summary>
    /// <param name="nextScene">The new scene to switch to</param>
    /// <param name="time">The time it takes to change to the new scene</param>
    public void ChangeScene(string sceneToChangeTo, float time)
    {
        StartCoroutine(ChangeSceneCoroutine(sceneToChangeTo, time));
    }

    /// <summary>
    /// Start the scene
    /// </summary>
    public void StartScene()
    {
        StartCoroutine(nameof(SetupSceneCoroutine));
    }


    /// <summary>
    /// Setup the scene
    /// </summary>
    private IEnumerator SetupSceneCoroutine()
    {
        // Wait a bit so it won't feel jerky
        yield return new WaitForSecondsRealtime(1f);

        // Play the music for the scene
        if (playMusicOnStart)
        {
            AudioPlayer.Instance.Play(songs.ElementAtOrDefault(0));
        }
        else
        {
            AudioPlayer.Instance.SetMusic(songs.ElementAtOrDefault(0));
        }

        // Start the dialogue
        if (startingScript != null)
        {
            Talk(startingScript, startingScriptNode != "" ? startingScriptNode.Trim() : "Start");
        }
    }


    private IEnumerator ChangeSceneCoroutine(string sceneToChangeTo, float time)
    {
        // Stop the dialogue
        dialogueRunner.Stop();

        cgHandler.SetCg(CgHandler.BlackScreenSpriteName, time);

        yield return new WaitForSecondsRealtime(time + 0.5f);

        Debug.Log($"Loading new scene: {sceneToChangeTo}");

        SceneManager.LoadScene(sceneToChangeTo);
    }
}
