using Cysharp.Threading.Tasks;
using DefaultNamespace.game;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class SceneManager : MonoBehaviour
    {
        public TMP_Text title;
        public Renderer curtain;
        
        async void Start()
        {
            waitForRestart().Forget();
            DI.globalScope.getInstance<BackgroundMusicController>().PlayMusic(0, false);
            // Waiting for end frame for every component to register itself
            await UniTask.WaitForEndOfFrame();
            startScene().Forget();
        }

        private async UniTask startScene()
        {
            
            
            var fishingFlow = DI.sceneScope.getInstance<FishingFlow>();
            var spawner = DI.sceneScope.getInstance<FishSpawner>();
            await spawner.InitialSpawn(3);
            await fishingFlow.onClickAwaitable();
            while (true)
            {
                await fishingFlow.StartFlow();    
            }
            
        }
        async UniTask waitForRestart()
        {
            await UniTask.WaitUntil(() => Input.GetKeyDown("r"));
            await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(0);
        }
    }
}