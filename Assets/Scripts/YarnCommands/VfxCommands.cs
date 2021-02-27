using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class VfxCommands : MonoBehaviour
{
    private GameHandler gameHandler;
    private Image cgToShake;
    private Vector3 cgToShakeOriginalPosition;
    private float screenShakeAmount = 0;


    private void Start()
    {
        gameHandler = GetComponent<GameHandler>();

        gameHandler.dialogueRunner.AddCommandHandler("vfxScreenShake", ShakeScreen);
        gameHandler.dialogueRunner.AddCommandHandler("vfxToggleTintScreen", TintScreen);

        // Disable Update()
        this.enabled = false;
    }

    private void Update()
    {
        if (screenShakeAmount != 0 && cgToShake != null)
        {
            cgToShake.transform.localPosition = cgToShakeOriginalPosition + Random.insideUnitSphere * screenShakeAmount;
        }
    }


    private void TintScreen(string[] _param)
    {
        gameHandler.cgHandler.ToggleTint();
    }

    private async void ShakeScreen(string[] param)
    {
        if (param.Length > 2)
        {
            Debug.LogError("Invalid parameters for a vfxScreenShake command.");
            return;
        }

        // StartCoroutine(nameof(ShakeScreenCoroutine), param);

        screenShakeAmount = float.Parse(param[0], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
        cgToShake = gameHandler.cgHandler.sprites[gameHandler.cgHandler.currentCgName];
        cgToShakeOriginalPosition = cgToShake.transform.localPosition;

        // Start the screen shaking
        this.enabled = true;

        // Start a DOTween so Skip Mode can skip the shake when it calls DOTween.CompleteAll
        await DOTween.To(() => screenShakeAmount, x => screenShakeAmount = x, 0, param.Length > 1 ? float.Parse(param[1], System.Globalization.CultureInfo.InvariantCulture.NumberFormat) : 1.5f).AsyncWaitForCompletion();

        // Stop
        this.enabled = false;

        // Reset stuff
        cgToShake.transform.localPosition = cgToShakeOriginalPosition;
        cgToShake = null;
    }
}
