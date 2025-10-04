using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DefaultNamespace.game
{
    public class Hook : MonoBehaviour
    {
        private FishData bait;
        
        private CircleCollider2D mainAttract;
        private CircleCollider2D targetedAttract;

        public float defaultRadius = 5f;
        public float targetedRadius = 10f;
        
        private UniTaskCompletionSource<FishData> hookedSource = new();
        
        private void Awake()
        {
            DI.sceneScope.register(this);
        }

        private void Start()
        {
            var attracts = GetComponents<CircleCollider2D>();
            mainAttract = attracts[0];
            targetedAttract = attracts[1];
        }

        public async UniTask<FishData> startAttract(int power)
        {
            hookedSource.TrySetCanceled();
            hookedSource = new UniTaskCompletionSource<FishData>();
            mainAttract.radius = defaultRadius * power;
            targetedAttract.radius = targetedRadius * power;
            return await hookedSource.Task;
        }

        public void hooked(FishData fish)
        {
            hookedSource.TrySetResult(fish);
        }
    }
}