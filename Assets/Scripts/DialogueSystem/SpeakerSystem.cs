using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SpeakerSystem : MonoBehaviour
{
    /// <summary>
    /// Contains predetermined positions for speakers
    /// </summary>
    /// <typeparam name="string">The name of the position</typeparam>
    /// <typeparam name="Vector3">The Vector3 position</typeparam>
    public static readonly Dictionary<string, Vector3> Positions = new Dictionary<string, Vector3>()
    {
        { "Left", new Vector3(0, 0) },
        { "Center1", new Vector3(500, 0) },
        { "Center2", new Vector3(920, 0) },
        { "Right", new Vector3(1500, 0) },
    };

    /// <summary>
    /// Contains predetermined sizes for use with the scaleSpeaker Yarn command
    /// </summary>
    /// <typeparam name="string">The scale's name</typeparam>
    /// <typeparam name="float">The scale's value</typeparam>
    public static readonly Dictionary<string, float> Sizes = new Dictionary<string, float>()
    {
        { "small", 0.714f },
        { "normal", 1f },
        { "large", 1.3f }
    };

    /// <summary>

    /// </summary>
    /// <typeparam name="string">The speaker's name</typeparam>
    /// <typeparam name="Vector3">The position of the speaker on the screen</typeparam>
    public readonly Dictionary<string, Vector3> speakers = new Dictionary<string, Vector3>();

    /// <summary>
    /// The prefab used to instantiate new speakers
    /// </summary>
    public Speaker characterEmotePrefab;

    /// <summary>
    /// The color to modulate the speake sprites with
    /// </summary>
    public Color modulation = Color.white;

    // Fade times for sprites
    private const float _fadeInTime = 0.5f;
    private const float _fadeOutTime = 0.4f;

    #region Parsing data

    private string[] _keys;

    private string[] _characterKeys;

    private string _speakerKey;

    private Speaker _currentSpeaker;

    private Image _currentEmote;
    private Image _emoteToShow;
    private string _emote;

    #endregion


    private void Awake()
    {
        // Make sure the alpha is 0 so the emotes won't be jittery when they first appear
        modulation = new Color(modulation.r, modulation.g, modulation.b, 0);
    }


    #region Methods
    /// <summary>
    /// Parse a line of dialogue and the set the speaker's emote accordingly
    /// </summary>
    /// <param name="line">The line of dialogue to parse</param>
    public void SetApproriateEmote(string line)
    {
        // Reset
        _currentSpeaker = null;

        _keys = line.Split(new char[] { ':' }, 2);

        // Stop if no speaker is specified
        if (_keys.Length == 1) return;

        // Get the dialogue _keys
        _characterKeys = _keys[0].Split(new char[] { ',' });

        // Stop if no emote is specified for the speaker
        if (_characterKeys.Length == 1) return;

        // The speaking character
        _speakerKey = _characterKeys[0].Trim();

        // Try to find existing speaker with the same name
        foreach (var character in transform.GetComponentsInChildren<Speaker>())
        {
            if (character.name == _speakerKey)
            {
                _currentSpeaker = character;

                if (!speakers.ContainsKey(character.name))
                {
                    _currentSpeaker.transform.localPosition = Positions[ConvertCountToPosition(speakers.Count)];

                    // Flip the sprite if it's on the right side of the screen
                    if (_currentSpeaker.transform.localPosition == Positions["Center2"] || _currentSpeaker.transform.localPosition == Positions["Right"])
                    {
                        _currentSpeaker.Flip();
                    }
                }
                break;
            }
        }

        if (_currentSpeaker == null)
        {
            // Create a new speaker

            Speaker newSpeaker = Instantiate(characterEmotePrefab, transform, false);

            newSpeaker.name = _speakerKey;
            newSpeaker.transform.localPosition = Positions[ConvertCountToPosition(speakers.Count)];

            // Flip the sprite if it's on the right side of the screen
            if (newSpeaker.transform.localPosition == Positions["Center2"] || newSpeaker.transform.localPosition == Positions["Right"])
            {
                newSpeaker.Flip();
            }

            // Modulate to EmoteSystem's modulation
            foreach (var sprite in newSpeaker.GetComponentsInChildren<Image>())
            {
                sprite.color = modulation;
            }

            _currentSpeaker = newSpeaker.GetComponent<Speaker>();
        }

        // Add to the speakers list
        speakers[_speakerKey] = _currentSpeaker.transform.localPosition;
        Debug.Log($"The current speaker is: {_currentSpeaker}");

        // The currently showing emote
        _currentEmote = _currentSpeaker.GetEmoteByIndex(_currentSpeaker.currentEmoteIndex);
        Debug.Log($"The speaker's emote is: {_currentEmote.transform.name}");

        _emote = _characterKeys[1].Trim().ToLower();

#if UNITY_EDITOR
        // Tests

        // The speaker should have a defined position
        Debug.Assert(_currentSpeaker.transform.localPosition != null, "The current speaker's position is null");

        // The speaker's name should match the dialogue line's specified speaker
        Debug.Assert(_currentSpeaker.transform.name == _speakerKey, "The current speaker's name does not match the speaker key defined in the dialogue line");

        // The speaker sprites' colors should be modulated to the specified modulation value
        // if (modulation != Color.white)
        // {
        //     Debug.Assert(_currentSpeaker.GetComponentsInChildren<Image>()[0].color == modulation, "The current speaker's sprites are not modulated to the SpeakerSystem's modulation");
        // }

        // if (_currentSpeaker.transform.localPosition == Positions["Center2"] || _currentSpeaker.transform.localPosition == Positions["Right"])
        // {
        //     // The speaker should be flipped if they're on the right side of the screen
        //     Debug.Assert(_currentSpeaker.transform.localScale.x < 0, "The current speaker is not flipped though it is on the right side of the screen");
        // }
        // else
        // {
        //     // The speaker should not be flipped otherwise
        //     Debug.Assert(_currentSpeaker.transform.localScale.x > 0, "The current speaker is flippe");
        // }

        // The speakers list should contain the current speaker
        Debug.Assert(speakers.Keys.Contains(_currentSpeaker.transform.name), "The speaker list does not contain the current speaker");

        // The speaker should already have an emote
        Debug.Assert(_currentEmote != null, "The speaker does not already have an emote");
#endif

        try
        {
            // Hide the current emote and show the next emote

            _emoteToShow = _currentSpeaker.GetEmoteByIndex(_currentSpeaker.nextEmoteIndex);
            _emoteToShow.sprite = transform.Find($"SpritesContainer/{_speakerKey}/{_emote}").GetComponent<SpriteRenderer>().sprite;

            if (!_currentSpeaker.isVisible)
            {
                _currentSpeaker.Show();
            }

            #if UNITY_EDITOR
              // Tests

              // The next emote should be defined
              Debug.Assert(_emoteToShow.sprite != null, "Could not find the emote to show for the current speaker");

              // The speaker should be visible
              Debug.Assert(_currentSpeaker.isVisible, "The speaker is not visible");
            #endif

            if (_currentEmote.sprite != _emoteToShow.sprite)
            {
                // Hide the current emote
                _currentEmote.DOFade(0f, _fadeOutTime);

                // Show the new emote
                _emoteToShow.DOFade(1f, _fadeInTime);

                // Update the index
                int temp = _currentSpeaker.currentEmoteIndex;
                _currentSpeaker.currentEmoteIndex = _currentSpeaker.nextEmoteIndex;
                _currentSpeaker.nextEmoteIndex = temp;

                #if UNITY_EDITOR
                  // Tests

                  // The speaker's emote should have changed
                  Debug.Assert(_currentSpeaker.GetEmoteByIndex(_currentSpeaker.currentEmoteIndex).sprite == _emoteToShow.sprite, "The speaker's emote did not change to a new emote");
                #endif
            }
            #if UNITY_EDITOR
              else
              {
                  // The speaker's emote should remain unchanged
                  Debug.Assert(_currentSpeaker.GetEmoteByIndex(_currentSpeaker.currentEmoteIndex).sprite == _currentEmote.sprite, "The speaker's emote changed to a new emote even though the specified emote in the dialogue line is the current emote");
              }
            #endif
        }
        catch
        {
            string err = $"Could not find emote <b>{_emote}</b> for <b>{_speakerKey}</b> in line: <b>{line}</b>";
            Debug.LogError(err);
            throw new System.ArgumentException(err);
        }
    }

    // Automatically determine where to place a new character sprite based on the number of onscreen speakers
    private string ConvertCountToPosition(int count)
    {
        switch (count)
        {
            case 0:
                return "Left";
            case 1:
                return "Center2";
            case 2:
                return "Center1";
            default:
                return "Right";
        }
    }
    #endregion
}
