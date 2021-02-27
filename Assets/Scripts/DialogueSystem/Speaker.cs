using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Speaker : MonoBehaviour
{
    public int currentEmoteIndex = 1;
    public int nextEmoteIndex = 2;

    public bool isVisible = false;

    private const float _fadeInTime = 0.5f;
    private const float _fadeOutTime = 0.4f;

    private const float _baseSpriteSize = 1400f;

    private CanvasGroup _canvasGroup;
    private SpeakerSystem _speakerSystem;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _speakerSystem = GetComponentInParent<SpeakerSystem>();
    }

    /// <summary>
    /// Show the speaker
    /// </summary>
    public void Show()
    {
        _canvasGroup.DOFade(1f, _fadeInTime);
        isVisible = true;
    }

    /// <summary>
    /// Hide the speaker
    /// </summary>
    public async void Hide()
    {
        isVisible = false;

        // Remove itself from the speakers list
        _speakerSystem.speakers.Remove(gameObject.name.Trim());

        await _canvasGroup.DOFade(0, _fadeOutTime).AsyncWaitForCompletion();

        // Reset the position
        transform.localPosition = Vector3.zero;

        // Reset the scale
        foreach(var imageTransform in GetComponentsInChildren<RectTransform>())
        {
            // Don't modify the container's transform
            if (imageTransform != transform)
            {
                imageTransform.sizeDelta = new Vector2(_baseSpriteSize, _baseSpriteSize);
            }
        }

        // Reset the emotes
        foreach (var emote in GetComponentsInChildren<Image>())
        {
            emote.transform.localScale = Vector3.one;
            emote.sprite = null;
            emote.color = new Color(emote.color.r, emote.color.g, emote.color.b, 0);
        }
    }

    /// <summary>
    /// Move from to a predetermined position in the SpeakerSystem.Positions list
    /// </summary>
    /// <param name="positionName">The position to move to</param>
    /// <param name="time">The time it takes to move to the specified position</param>
    public void SetPosition(string positionName, float time)
    {
        if (SpeakerSystem.Positions.ContainsKey(positionName))
        {
            // Complete any running DOLocalMove
            DOTween.Complete(transform);

            transform.DOLocalMove(SpeakerSystem.Positions[positionName], time).SetTarget(transform);
        }
        else
        {
            Debug.LogError($"Could not set the position with the name {positionName}");
        }
    }

    /// <summary>
    /// Move relative to the current position 
    /// </summary>
    /// <param name="position">How much to move relative to the current position</param>
    /// <param name="time">The time it takes for the movement to complete</param>
    public void SetPosition(Vector3 position, float time)
    {
        // Complete any running DOLocalMove
        DOTween.Complete(transform);

        transform.DOLocalMove(new Vector3(
            transform.localPosition.x + position.x,
            transform.localPosition.y + position.y,
            0
        ), time).SetTarget(transform);
    }

    /// <summary>
    /// Scale the speaker
    /// </summary>
    /// <param name="modifier">How much to scale the speaker</param>
    /// <param name="time">The time it takes the scale to complete</param>
    public void Scale(float modifier, float time)
    {
        foreach(var imageTransform in GetComponentsInChildren<RectTransform>())
        {
            // Complete any running DOSizeDelta
            DOTween.Complete(imageTransform);

            if (imageTransform != transform) // Don't modify the container's transform
            {
                imageTransform.DOSizeDelta(
                    new Vector3(_baseSpriteSize * modifier, _baseSpriteSize * modifier, 0),
                    time).SetTarget(imageTransform);
            }
        }
    }

    /// <summary>
    /// Flip the speaker's sprite
    /// </summary>
    public void Flip()
    {
        foreach(var emote in GetComponentsInChildren<Image>())
        {
            emote.transform.localScale = new Vector3(-emote.transform.localScale.x, emote.transform.localScale.y, emote.transform.localScale.z);
        }
    }

    /// <summary>
    /// Set the emote for a speaker
    /// </summary>
    /// <param name="emote">The emote to set</param>
    public void SetCurrentEmote(Sprite emote)
    {
        transform.Find(currentEmoteIndex.ToString()).GetComponent<Image>().sprite = emote;
    }

    /// <summary>
    /// Get a speaker's emote with the Current or Next index
    /// </summary>
    /// <param name="index">The Current or Next index</param>
    /// <returns>The emote image</returns>
    public Image GetEmoteByIndex(int index)
    {
        if (index == currentEmoteIndex || index == nextEmoteIndex)
        {
            return transform.Find(index.ToString()).GetComponent<Image>();
        }
        return null;
    }
}
