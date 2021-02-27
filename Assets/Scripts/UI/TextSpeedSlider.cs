using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

/// <summary>
/// Contains the logic for the slider used to set the game's text speed
/// </summary>
public class TextSpeedSlider : MonoBehaviour
{
    private Text label;
    private Slider slider;

    // Criterias to update the label by
    private float FastCriteria;
    private float NormalCriteria;
    private float SlowCriteria;

    void Start()
    {
        label = GetComponentInChildren<Text>();
        slider = GetComponent<Slider>();

        FastCriteria = slider.minValue * 200;
        NormalCriteria = (float)(slider.minValue * 3.5);
        SlowCriteria = (float)(slider.maxValue / 1.3);

        slider.onValueChanged.AddListener(value =>
        {
            if (value < FastCriteria)
            {
                label.text = "Text Speed: Fast";
            }
            else if (value > NormalCriteria && value < SlowCriteria)
            {
                label.text = "Text Speed: Normal";
            }
            else
            {
                label.text = "Text Speed: Slow";
            }

            DialogueUI.textSpeed = value;
        });

        slider.value = DialogueUI.textSpeed;
    }
}
