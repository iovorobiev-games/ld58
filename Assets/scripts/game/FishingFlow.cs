using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using utils;
using Random = UnityEngine.Random;

namespace DefaultNamespace.game
{
    public class FishingFlow : AwaitableClickHandlerImpl
    {
        
        BoxCollider2D collider;
        private Fisher fisher;
        private Hook hook;
        private ThrowGame throwGame;
        private ReelGame reelGame;
        private ResultWindow resultWindow;
        private ChooseBait chooseBait;
        private FishSpawner spawner;

        private void Awake()
        {
            DI.sceneScope.register(this);
        }

        private void Start()
        {
            
            collider = GetComponent<BoxCollider2D>();
            fisher = DI.sceneScope.getInstance<Fisher>();
            hook = DI.sceneScope.getInstance<Hook>();
            throwGame = DI.sceneScope.getInstance<ThrowGame>();
            reelGame = DI.sceneScope.getInstance<ReelGame>();
            resultWindow = DI.sceneScope.getInstance<ResultWindow>();
            chooseBait = DI.sceneScope.getInstance<ChooseBait>();
            spawner = DI.sceneScope.getInstance<FishSpawner>();
            collider.enabled = true;

        }

        public async UniTask StartFlow()
        {
            var bait = await chooseBait.chooseBait();
            hook.bait = bait;
            await fisher.prepareThrow();
            int power = await throwGame.StartGame();
            await fisher.startFishing(power);
            var result = await UniTask.WhenAny(hook.startAttract(power), onClickAwaitable());
            Fish caughtFish = null;
            if (result.hasResultLeft)
            {
                caughtFish = result.result;
                var fishingResult= await UniTask.WhenAny(reelGame.StartGame(1 + caughtFish.data.Strength), caughtFish.releaseWhenItWants());
                if (!fishingResult.hasResultLeft)
                {
                    await reelGame.HideReel();
                    caughtFish.release().Forget();
                    caughtFish = null;
                }
                else
                {
                    
                    if (!fishingResult.result)
                    {
                        caughtFish.release().Forget();
                        caughtFish = null;   
                    }
                }
            }
            await fisher.pullHook(caughtFish != null);
            if (caughtFish != null)
            {
                var data = caughtFish.data;
                spawner.SpawnOnDepth(data.Depth);
                caughtFish.Destroy();
                await resultWindow.showWith(data);
            }
            else
            {
                var shouldSpawn = Random.Range(0f, 0.25f);
                if (shouldSpawn < 0.25f)
                {
                    var depth = shouldSpawn switch
                    {
                        < 0.05f => 2,
                        < 0.15f => 1,
                        _ => 0
                    };
                    spawner.SpawnOnDepth(depth);
                }
            }
        }
    }
}