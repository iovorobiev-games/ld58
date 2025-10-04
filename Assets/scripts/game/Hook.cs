using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace DefaultNamespace.game
{
    public class Hook : MonoBehaviour
    {
        private FishData bait;
        
        private CircleCollider2D mainAttract;

        public float defaultRadius = 2f;
        private bool hasPrey;
        
        private UniTaskCompletionSource<FishData> hookedSource = new();
        private Tween bitingTween;
        
        private void Awake()
        {
            DI.sceneScope.register(this);
        }

        private void Start()
        {
            var attracts = GetComponents<CircleCollider2D>();
            bait = new FishData("Worm", baitStrength: 10);
            mainAttract = attracts[0];
        }

        public async UniTask<FishData> startAttract(int power)
        {
            hookedSource.TrySetCanceled();
            hookedSource = new UniTaskCompletionSource<FishData>();
            mainAttract.radius = defaultRadius * power;
            var fish = await hookedSource.Task;
            hasPrey = true;
            animateCapture().Forget();
            return fish;
        }

        private async UniTask animateCapture()
        {
            bitingTween = DOTween.Sequence()
                .Append(transform.parent.DOMove(transform.parent.position + Vector3.down * 0.1f, 0.2f).SetLoops(2, LoopType.Yoyo))
                .AppendInterval(0.5f);
        }

        public void cancelBiting()
        {
            DOTween.Kill(bitingTween);
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
        
        public void hooked(FishData fish)
        {
            hookedSource.TrySetResult(fish);
        }
    }
}