using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using utils;

namespace DefaultNamespace.game
{
    public class FishingFlow : AwaitableClickHandlerImpl
    {
        
        BoxCollider2D collider;
        private Fisher fisher;
        private Hook hook;
        private ThrowGame throwGame;
        private ReelGame reelGame;

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
        }

        public async UniTask StartFlow()
        {
            collider.enabled = true;
            await onClickAwaitable();
            await fisher.prepareThrow();
            int power = await throwGame.StartGame();
            await fisher.startFishing(power);
            var result = await UniTask.WhenAny(hook.startAttract(power), onClickAwaitable());
            Fish caughtFish = null;
            if (result.hasResultLeft)
            {
                caughtFish = result.result;
                await reelGame.ShowReel();
                var fishingResult= await UniTask.WhenAny(onClickAwaitable(), caughtFish.releaseWhenItWants());
                if (fishingResult == 1)
                {
                    caughtFish.release().Forget();
                    caughtFish = null;
                }
                else
                {
                    var isCaught = await reelGame.StartGame(3 + caughtFish.data.Strength);
                    if (!isCaught)
                    {
                        caughtFish.release().Forget();
                        caughtFish = null;   
                    }
                }

                await reelGame.HideReel();
            }
            await fisher.pullHook();
            Debug.Log("Caught a fish: " + (caughtFish?.data?.Name ?? "none"));
            if (caughtFish != null)
            {
                caughtFish.Destroy();
            }
        }
    }
}