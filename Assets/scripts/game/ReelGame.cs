using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace.game
{
    public class ReelGame : MonoBehaviour
    {
        public GameObject handle;
        public GameObject pointer;
        
        public Collider2D handleCollider;
        public Collider2D pointerCollider;
        public float speedSec = 1f;
        private FishingFlow fishFlow;
        SpriteRenderer[] allChildrenSRs = new SpriteRenderer[0];
        
        
        private void Awake()
        {
            DI.sceneScope.register(this);
        }

        private void Start()
        {
            fishFlow = DI.sceneScope.getInstance<FishingFlow>();
            allChildrenSRs = GetComponentsInChildren<SpriteRenderer>();
            gameObject.SetActive(false);
        }

        public async UniTask ShowReel()
        {
            gameObject.SetActive(true);
            foreach (var allChildrenSR in allChildrenSRs)
            {
                allChildrenSR.DOColor(allChildrenSR.color.WithAlpha(1f), 0.25f).From(allChildrenSR.color.WithAlpha(0f));
            }
        }
        
        public async UniTask<bool> StartGame(int times)
        {
            await ShowReel();
            handle.transform.DOKill();
            var tween =
                // DOTween.Sequence().
                handle.transform.DORotate(new Vector3(0f, 0f, -360f), speedSec, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);

            for (int i = 0; i < times; i++)
            {
                pointer.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(1, 36) * 10f);
                await fishFlow.onClickAwaitable();
                if (!pointerCollider.IsTouching(handleCollider))
                {
                    tween.Kill();
                    await HideReel();
                    return false;
                }
            }
            tween.Kill();
            await HideReel();
            return true;
        }

        public async UniTask HideReel()
        {
            foreach (var allChildrenSR in allChildrenSRs)
            {
                allChildrenSR.DOColor(allChildrenSR.color.WithAlpha(0f), 0.25f).From(allChildrenSR.color.WithAlpha(1f));
            }

            await UniTask.WaitForSeconds(0.26f);
            gameObject.SetActive(false);
        }
    }
}