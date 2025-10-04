using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DefaultNamespace.game
{
    public class Fisher : MonoBehaviour
    {
        private Animator animator;
        
        private void Start()
        {
            animator = GetComponent<Animator>();
            DI.sceneScope.register(this);
        }

        public async UniTask prepareThrow()
        {
            animator.Play("throw");
        }

        public async UniTask startFishing()
        {
            animator.Play("actually_throw");
        }
    }
}