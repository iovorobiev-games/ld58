using System;
using System.Collections.Generic;
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
        private Player player;
        private CollectionView collection;
        private Pond pond;

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
            player = DI.globalScope.getInstance<Player>();
            collection = DI.sceneScope.getInstance<CollectionView>();
            pond = DI.sceneScope.getInstance<Pond>();
            collider.enabled = true;

        }

        public async UniTask StartFlow()
        {
            var bait = await chooseBait.chooseBait();
            hook.bait = bait;
            await fisher.prepareThrow();
            collection.hideView().Forget();
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
            FishData spawned = null;
            if (caughtFish != null)
            {
                var data = caughtFish.data;
                pond.fish.Remove(caughtFish.data);
                if (pond.getFishCount("Stardiva") < 2)
                {
                    spawned = FishDB.findByName("Stardiva");
                    spawner.Spawn(spawned);
                } else if (pond.getFishCount("Beauty-poppy") < 2)
                {
                    spawned = FishDB.findByName("Beauty-poppy");
                    spawner.Spawn(spawned);
                } else if (Random.Range(0f, 1f) > 0.5f)
                    spawned = spawner.SpawnOnDepth(data.Depth);
                else
                {
                    spawned = FishDB.getRandomNotIn(player.collection);
                    spawner.Spawn(spawned);
                }
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
                    spawned = spawner.SpawnOnDepth(depth);
                }
            }

            if (spawned != null)
            {
                pond.fish.Add(spawned);
            }
        }
    }
}