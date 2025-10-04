using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using utils;

namespace DefaultNamespace.game
{
    public class FishingFlow : AwaitableClickHandlerImpl
    {
        
        BoxCollider2D collider;
        private void Awake()
        {
            DI.sceneScope.register(this);
        }

        private void Start()
        {
            collider = GetComponent<BoxCollider2D>();
        }

        public async UniTask StartFlow()
        {
            var fisher = DI.sceneScope.getInstance<Fisher>();
            collider.enabled = true;
            await onClickAwaitable();
            await fisher.prepareThrow();
            await onClickAwaitable();
            await fisher.startFishing();
        }
    }
}