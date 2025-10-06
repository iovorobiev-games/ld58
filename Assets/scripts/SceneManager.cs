using Cysharp.Threading.Tasks;
using DefaultNamespace.game;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class SceneManager : MonoBehaviour
    {
        public TMP_Text title;
        public Renderer curtain;
        public Book book;
        public GameObject thoughts;
        public TMP_Text thoughtsText;
        
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
            var collectionView = DI.sceneScope.getInstance<CollectionView>();
            var allFish = await spawner.InitialSpawn(4);
            var pond = DI.sceneScope.getInstance<Pond>();
            pond.fish = allFish;
            title.gameObject.SetActive(true);
            title.text = "<sketchy>Click</sketchy>";
            await fishingFlow.onClickAwaitable();
            title.gameObject.SetActive(false);
            await book.swim();
            
            thoughts.SetActive(true);
            thoughtsText.text = "<sketchy>??</sketchy>";
            await UniTask.WaitForSeconds(2f);
            book.gameObject.SetActive(false);

            collectionView.gameObject.SetActive(true);
            await collectionView.showView();
            await UniTask.WaitForSeconds(2f);
            thoughtsText.text = "<sketchy>!!</sketchy>";
            title.gameObject.SetActive(true);
            title.text = "Ah, gotta catch 'em all!";
            var textTask = title.DOColor(Color.white.WithAlpha(1f), 1f).From(Color.white.WithAlpha(0f)).ToUniTask();
            await textTask;
            title.text = "<sketchy>Ah, gotta catch 'em all!</>";
            thoughts.SetActive(false);
            await UniTask.WaitForSeconds(1f);
            var curtainTask = curtain.material.DOFloat(0f, "_Progress", 2f).From(1f).ToUniTask();
            await curtainTask;
            await fishingFlow.onClickAwaitable();
            collectionView.enabled = true;
            await collectionView.hideView();
            
            await title.DOColor(Color.white.WithAlpha(0f), 2f).From(Color.white.WithAlpha(1f)).ToUniTask();
            title.gameObject.SetActive(false);

            var player = DI.globalScope.getInstance<Player>();
            while (player.collection.Count < FishDB.allFish.Count)
            {
                await fishingFlow.StartFlow();    
            }
            title.gameObject.SetActive(true);
            title.text = "Thanks for playing!";
            await title.DOColor(Color.white.WithAlpha(1f), 2f).From(Color.white.WithAlpha(0f)).ToUniTask();
            title.text = "<sketchy>Thanks for playing!</sketchy>";
            await collectionView.showView();
        }
        async UniTask waitForRestart()
        {
            await UniTask.WaitUntil(() => Input.GetKeyDown("r"));
            await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(0);
        }
    }
}