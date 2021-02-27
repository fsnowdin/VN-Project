using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class LocationPopup : MonoBehaviour
{
    public List<string> showList = new List<string>()
    {
        "Time",
        "Location Name"
    };

    public float charactersPerSecond = 1;

    private TextMeshProUGUI tmp;
    private GameHandler gameHandler;


    private void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        tmp.text = string.Empty;
        showList.ForEach(item => item.Trim());

        gameHandler = FindObjectOfType<GameHandler>();
        gameHandler.autoStartScene = false;
    }

    private void Start()
    {
        // Start playing the location popup animation
        Play();
    }

    private async Task Play()
    {
        var locationPopup = gameHandler.cgHandler.sprites[CgHandler.BlackScreenSpriteName].GetComponentInChildren<LocationPopup>();

        // Stop the music
        AudioPlayer.Instance.Stop();

        // Delay a bit to not make it so sudden
        await System.Threading.Tasks.Task.Delay(1000);

        // Show the lines gradually one by one
        {
            StringBuilder stringBuilder;

            foreach (string item in showList)
            {
                stringBuilder = new StringBuilder(item.Length);

                // Show
                foreach (char character in item)
                {
                    stringBuilder.Append(character);
                    tmp.text = stringBuilder.ToString();
                    await Task.Delay(Mathf.FloorToInt(charactersPerSecond * 1000));
                }

                await Task.Delay(2500);

                // Hide
                foreach (char character in item)
                {
                    stringBuilder.Remove(stringBuilder.Length - 1, 1);
                    tmp.text = stringBuilder.ToString();
                    await Task.Delay(Mathf.FloorToInt(charactersPerSecond * 500));
                }

                await Task.Delay(1000);
            }
        }

        Destroy(locationPopup.gameObject);

        gameHandler.StartScene();
    }
}
