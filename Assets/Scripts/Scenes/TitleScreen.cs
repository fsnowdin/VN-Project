using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreen : GameHandler
{
    public string startingScene;
    public string continueScene;

    // Override to nullify default behavior
    void Awake() {}


    // Override to nullify default behavior
    void Update() {}


    void Start()
    {
        #if UNITY_ANDROID || UNITY_IOS
            // Prevent screen from sleeping on mobile
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        #endif

        cgHandler = FindObjectOfType<CgHandler>();

        // Transition
        // cgHandler.SetCg(CgHandler.BlackScreenSpriteName);

        // Play the title song
        AudioPlayer.Instance.Play(songs[0]);
    }


    public void OnStartButtonClicked()
    {
        FindObjectOfType<Button>().interactable = false;

        StartCoroutine(nameof(StartGame)); 
    }


    public void ContinueGame()
    {
        StartCoroutine(nameof(ContinueGameCoroutine));
    }

    // Quit the game
    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator StartGame()
    {
        // cgHandler.SetCg(CgHandler.BlackScreenSpriteName);

        yield return new WaitForSecondsRealtime(2f);

        SceneManager.LoadScene(startingScene);
    }
    
    private IEnumerator ContinueGameCoroutine()
    {
        cgHandler.SetCg(CgHandler.BlackScreenSpriteName);

        yield return new WaitForSecondsRealtime(2f);

        SceneManager.LoadScene(continueScene);
    }
}
