using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace DefaultNamespace.game
{
    public class Hook : MonoBehaviour
    {
        public FishData bait;
        
        private CircleCollider2D mainAttract;

        public float defaultRadius = 2f;
        private bool hasPrey;
        
        private UniTaskCompletionSource<Fish> hookedSource = new();
        private Tween bitingTween;
        
        private void Awake()
        {
            DI.sceneScope.register(this);
        }

        private void Start()
        {
            var attracts = GetComponents<CircleCollider2D>();
            mainAttract = attracts[0];
        }

        public async UniTask<Fish> startAttract(int power)
        {
            hookedSource.TrySetCanceled();
            hookedSource = new UniTaskCompletionSource<Fish>();
            mainAttract.enabled = true;
            mainAttract.radius = defaultRadius * 2;
            var fish = await hookedSource.Task;
            mainAttract.radius = 0;
            hasPrey = true;
            animateCapture();
            return fish;
        }

        private void animateCapture()
        {
            bitingTween = DOTween.Sequence()
                .Append(transform.parent.DOMove(transform.parent.position + Vector3.down * 0.1f, 0.2f)
                    .SetLoops(2, LoopType.Yoyo).SetId("hookBiting"))
                .AppendInterval(0.5f)
                .SetLoops(-1, LoopType.Yoyo).SetId("hookBitingTop");
        }

        public void cancelBiting()
        {
            DOTween.Kill(bitingTween);
            DOTween.Kill("hookBiting");
            DOTween.Kill("hookBitingTop");
            hasPrey = false;
        }
        
        public FishData getBait()
        {
            return bait;
        }

        public bool HasPrey
        {
            get => hasPrey;
        }

        public async UniTask<bool> awaitIsHooked()
        {
            return await hookedSource.Task != null;
        }
        
        public void hooked(Fish fish)
        {
            hookedSource.TrySetResult(fish);
        }
    }
}