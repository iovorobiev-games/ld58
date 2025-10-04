using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DefaultNamespace.game
{
    public class Fisher : MonoBehaviour
    {
        private Animator animator;
        private RodString rodString;

        private void Awake()
        {
            DI.sceneScope.register(this);
        }

        private void Start()
        {
            animator = GetComponent<Animator>();
            rodString = DI.sceneScope.getInstance<RodString>();
        }

        public async UniTask prepareThrow()
        {
            animator.Play("throw");
        }

        public async UniTask startFishing()
        {
            animator.Play("actually_throw");
            await UniTask.Delay(250);
            await rodString.throwHook(7f, 1, 1);
        }

        public async UniTask pullHook()
        {
            await rodString.pullHook();
            animator.Play("idle");
        }
    }
}