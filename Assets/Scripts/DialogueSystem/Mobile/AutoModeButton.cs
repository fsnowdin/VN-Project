using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
/// <summary>
/// Button to toggle Auto mode for mobile platforms
/// </summary>
public class AutoModeButton : MonoBehaviour
{
    private GameHandler gameHandler = null;

    void Start()
    {
        #if UNITY_ANDROID || UNITY_IOS
            gameObject.SetActive(true);

            gameHandler = FindObjectOfType<GameHandler>();

            GetComponent<Image>().enabled = true;
            GetComponentInChildren<Text>().enabled = true;

            GetComponent<Button>().onClick.AddListener(OnAutoModeButtonClicked);
        #elif UNITY_STANDALONE || UNITY_WEBGL
            GetComponent<Image>().enabled = false;
            GetComponentInChildren<Text>().enabled = false;
            #if !UNITY_EDITOR
                Destroy(gameObject);
            #endif
        #endif
    }

    // Toggle Auto mode on click
    private void OnAutoModeButtonClicked()
    {
        gameHandler.dialogueUI.ToggleAutoMode();
    }
}
