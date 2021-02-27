using UnityEngine;

namespace Assets.Scripts.VFXs
{
    public class TitleCard : MonoBehaviour
    {
        private GameHandler gameHandler;

        private void Awake()
        {
           gameHandler = FindObjectOfType<GameHandler>();
           gameHandler.autoStartScene = false;
        }

        // Listener when the animation is finished
        public void OnAnimationFinish()
        {
            gameHandler.StartScene();
        }
    }
}
