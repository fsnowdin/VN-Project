using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

[ExecuteInEditMode]
/// <summary>
/// Button to skip dialogue for mobile platforms
/// </summary>
public class SkipModeButton : MonoBehaviour
{
    void Start()
    {
        #if UNITY_ANDROID || UNITY_IOS
            gameObject.SetActive(true);

            GetComponent<Image>().enabled = true;
            GetComponentInChildren<Text>().enabled = true;

            gameHandler = FindObjectOfType<GameHandler>();
        #elif UNITY_STANDALON || UNITY_WEBGL
            GetComponent<Image>().enabled = false;
            GetComponentInChildren<Text>().enabled = false;
            #if !UNITY_EDITOR
                Destroy(gameObject);
            #endif
        #endif
    }

    #if UNITY_ANDROID || UNITY_IOS
        public bool isHeldDown = false;

        private GameHandler gameHandler = null;

        void Update()
        {
            if (isHeldDown)
            {
                DialogueUI.isSkipModeEnabled = true;
                gameHandler.dialogueUI.MarkLineComplete();
            }
        }

        public void OnPointerUp()
        {
            isHeldDown = false;
            DialogueUI.isSkipModeEnabled = false;
        }

        public void OnPointerDown()
        {
            isHeldDown = true;
        }
    #endif
}
