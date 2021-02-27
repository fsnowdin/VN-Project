using System.Globalization;
using System.Linq;
using UnityEngine;
using Yarn.Unity;

/// <summary>
/// Yarn commands to manage speakers
/// </summary>
public class SpeakerCommands : MonoBehaviour
{
    private const string _showCommandName = "showSpeaker";
    private const string _hideCommandName = "hideSpeaker";
    private const string _moveCommandName = "moveSpeaker";
    private const string _flipCommandName = "flipSpeaker";
    private const string _scaleCommandName = "scaleSpeaker";
    private const string _setSpeakerEmoteCommandName = "setSpeakerEmote";

    private const float _defaultMoveTime  = 1.0f;

    private DialogueRunner _dialogueRunner;
    private SpeakerSystem _speakerSystem;

    void Start()
    {
        _dialogueRunner = FindObjectOfType<GameHandler>().dialogueRunner;
        _speakerSystem = FindObjectOfType<SpeakerSystem>();

        // Initialize the Yarn commands
        _dialogueRunner.AddCommandHandler(_showCommandName, Show);
        _dialogueRunner.AddCommandHandler(_hideCommandName, Hide);
        _dialogueRunner.AddCommandHandler(_moveCommandName, Move);
        _dialogueRunner.AddCommandHandler(_flipCommandName, Flip);
        _dialogueRunner.AddCommandHandler(_scaleCommandName, Scale);
        _dialogueRunner.AddCommandHandler(_setSpeakerEmoteCommandName, SetEmote);
    }

    /// <summary>
    /// Show the speaker
    /// </summary>
    /// <param name="param">The speaker's name (Optional: an initial emote)</param>
    public void Show(string[] param)
    {
        if (param.Length == 0 || param.Length > 2)
        {
            Debug.LogError($"A {_showCommandName} command failed. Its parameters were invalid.");
        }
        else
        {
            try
            {
                param[0] = ReplaceUnderscoreWithSpace(param[0]);

                Transform speaker = transform.Find(param[0]);

                if (param.ElementAtOrDefault(1) == null) // Check if any emote is specified
                {
                    if (speaker == null)
                    {
                        // Create and show the speaker with the default emote

                        // The input string must be like this so don't change it
                        _speakerSystem.SetApproriateEmote($"{param[0]}, normal:");
                    }
                    else
                    {
                        // Just show the speaker if they already exist
                        speaker.GetComponent<Speaker>().Show();
                    }
                }
                else
                {
                    // Show the speaker with the specified emote

                    // The input string must be like this so don't change it
                    _speakerSystem.SetApproriateEmote($"{param[0]}, {param[1]}:");
                }
            }
            catch
            {
                Debug.LogError($"A <b>{_showCommandName}</b> command failed. Could not find specified speaker {param[0]}.");
            }
        }
    }

    /// <summary>
    /// Hide the speaker
    /// </summary>
    /// <param name="param">The speaker's name</param>
    public void Hide(string[] param)
    {
        if (param.Length != 1)
        {
            Debug.LogError($"A <b>{_hideCommandName}</b> command failed. Its parameters were invalid.");
        }
        else
        {
            try
            {
                Debug.Log($"Hiding speaker {param[0]}");

                param[0] = ReplaceUnderscoreWithSpace(param[0]);

                transform.Find(param[0]).GetComponent<Speaker>().Hide();
            }
            catch
            {
                Debug.LogError($"A <b>{_hideCommandName}</b> command failed. Could not find specified speaker {param[0]}.");
            }
        }
    }

    /// <summary>
    /// Move the speaker to a specified position
    /// </summary>
    /// <param name="param">A predetermined position or a relative position in X and Y (Optional: the movement time)</param>
    public void Move(string[] param)
    {
        if (param.Length <= 1)
        {
            Debug.LogError($"A <b>{_moveCommandName}</b> command failed. Its parameters were invalid.");
            return;
        }

        Debug.Log($"Moving the speaker {param[0]}");

        param[0] = ReplaceUnderscoreWithSpace(param[0]);

        // See if the input position is one of the predetermined ones
        if (SpeakerSystem.Positions.ContainsKey(param.ElementAtOrDefault(1)) && param.Length < 4)
        {
            try
            {
                var speaker = transform.Find(param[0]).GetComponent<Speaker>();

                string positionName = param[1];
                float time = param.ElementAtOrDefault(2) != null ? float.Parse(param[2], CultureInfo.InvariantCulture.NumberFormat) : _defaultMoveTime;

                speaker.SetPosition(positionName, time);
            }
            catch
            {
                Debug.LogError($"A <b>{_moveCommandName}</b> command failed.");
            }
        }
        else if (param.Length <= 4 && param.ElementAtOrDefault(1) != null)
        {
            // Input position is the new X and Y relative position

            try
            {
                var speaker = transform.Find(param[0]).GetComponent<Speaker>();

                float positionX = param.ElementAtOrDefault(1) != null ? float.Parse(param[1], CultureInfo.InvariantCulture.NumberFormat) : 0;
                float positionY = param.ElementAtOrDefault(2) != null ? float.Parse(param[2], CultureInfo.InvariantCulture.NumberFormat) : 0;

                float time = param.ElementAtOrDefault(3) != null ? float.Parse(param[3], CultureInfo.InvariantCulture.NumberFormat) : _defaultMoveTime;

                speaker.SetPosition(new Vector3(positionX, positionY, 0), time);
            }
            catch
            {
                Debug.LogError($"A <b>{_moveCommandName}</b> command failed.");
            }
        }
    }

    /// <summary>
    /// Flip the speaker's sprite
    /// </summary>
    /// <param name="param">The speaker's name</param>
    public void Flip(string[] param)
    {
        if (param.Length != 1)
        {
            Debug.LogError($"A <b>{_flipCommandName}</b> command failed. Its parameters were invalid.");
            return;
        }

        Debug.Log($"Flipping the sprite of the speaker {param[0]}");

        try
        {
            param[0] = ReplaceUnderscoreWithSpace(param[0]);

            transform.Find(param[0]).GetComponent<Speaker>().Flip();
        }
        catch
        {
            Debug.LogError($"A <b>{_flipCommandName}</b> command failed.");
        }
    }

    /// <summary>
    /// Scale the speaker's sprite
    /// </summary>
    /// <param name="param">The speaker's name and how much to scale (with predetermined values or float values) (Optional: The time for the scale to complete)</param>
    public void Scale(string[] param)
    {
        if (param.Length < 2 || param.Length > 3)
        {
            Debug.LogError($"A <b>{_scaleCommandName}</b> command failed. Its parameters were invalid.");
            return;
        }

        Debug.Log($"Scaling the speaker {param[0]}");

        try
        {
            param[0] = ReplaceUnderscoreWithSpace(param[0]);

            var speaker = transform.Find(param[0]).GetComponent<Speaker>();

            float time = param.Length == 3 ? float.Parse(param[2], CultureInfo.InvariantCulture.NumberFormat) : 1f;

            string size = param[1].Trim().ToLower();

            if (SpeakerSystem.Sizes.ContainsKey(size)) // Size modifier is small, normal, or large
            {
                speaker.Scale(SpeakerSystem.Sizes[size], time);
            }
            else // Size modifier is a number
            {
                speaker.Scale(float.Parse(size, CultureInfo.InvariantCulture.NumberFormat), time);
            }
        }
        catch
        {
            Debug.LogError($"A <b>{_scaleCommandName}</b> command failed.");
        }
    }

    /// <summary>
    /// Set the emote for the speaker
    /// </summary>
    /// <param name="param">The speaker's name and the emote to set</param>
    public void SetEmote(string[] param)
    {
        if (param.Length != 2)
        {
            Debug.LogError($"A <b>{_setSpeakerEmoteCommandName}</b> command failed. Its parameters were invalid.");
            return;
        }

        try
        {
            param[0] = ReplaceUnderscoreWithSpace(param[0]);

            transform.Find(param[0]).GetComponent<Speaker>().SetCurrentEmote(transform.Find($"SpritesContainer/{param[0]}/{param[1].Trim()}").GetComponent<SpriteRenderer>().sprite);
        }
        catch
        {
            Debug.LogError($"A <b>{_setSpeakerEmoteCommandName}</b> command failed.");
        }
    }

    /* Replace underscores in a string with spaces because
    we use underscores to indicate spaces for speaker names in Yarn scripts */
    private string ReplaceUnderscoreWithSpace(string input)
    {
        if (input.IndexOf('_') != -1)
        {
            input = input.Replace('_', ' ').Trim();
        }
        return input;
    }
}
