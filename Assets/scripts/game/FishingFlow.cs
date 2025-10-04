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
        
        private void Awake()
        {
            DI.sceneScope.register(this);
        }

        private void Start()
        {
            
            collider = GetComponent<BoxCollider2D>();
            fisher = DI.sceneScope.getInstance<Fisher>();
            hook = DI.sceneScope.getInstance<Hook>();
        }

        public async UniTask StartFlow()
        {
            collider.enabled = true;
            await onClickAwaitable();
            await fisher.prepareThrow();
            await onClickAwaitable();
            await fisher.startFishing();
            var result = await UniTask.WhenAny(hook.startAttract(1), onClickAwaitable());
            FishData caughtFish = null;
            if (result.hasResultLeft)
            {
                await onClickAwaitable();
            }
            await fisher.pullHook();
        }
    }
}