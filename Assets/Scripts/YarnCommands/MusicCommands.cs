using System.Linq;
using UnityEngine;
using Yarn.Unity;

/// <summary>
/// Yarn command to manage the music
/// </summary>
public class MusicCommands : MonoBehaviour
{
    private GameHandler gameHandler;
    private DialogueRunner dialogueRunner;

    void Start()
    {
        gameHandler = GetComponent<GameHandler>();
        dialogueRunner = gameHandler.dialogueRunner;

        // Add the commands
        dialogueRunner.AddCommandHandler("musicNext", NextSong);
        dialogueRunner.AddCommandHandler("musicStop", StopSong);
        dialogueRunner.AddCommandHandler("musicContinue", ContinueSong);
    }


    public void NextSong(string[] param)
    {
        AudioClip nextSong = gameHandler.songs.ElementAtOrDefault(++gameHandler.currentSongIndex);
        
        if (nextSong != null)
            AudioPlayer.Instance.Play(nextSong);
        else
        {
            gameHandler.currentSongIndex--;
            Debug.LogError($"A musicNext command failed. The currentSongIndex is {gameHandler.currentSongIndex}");
        }
    }

    public void StopSong(string[] param)
    {
        AudioPlayer.Instance.Stop();
    }

    public void ContinueSong(string[] param)
    {
        AudioPlayer.Instance.Continue();
    }
}
