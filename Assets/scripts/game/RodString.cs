using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace DefaultNamespace.game
{
    public class RodString : MonoBehaviour
    {
        LineRenderer lineRenderer;
        public Transform hook;
        public Transform stableParent;
        private Transform hookForLine;
        private void Awake()
        {
            DI.sceneScope.register(this);
        }

        private void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
            hookForLine = hook;
        }

        public async UniTask throwHook(float depth, int power, int baitPower)
        {
            var newHookPosition = transform.position + Vector3.down * depth;
            hookForLine = stableParent;
            
            await hookForLine.DOMove(newHookPosition, 0.5f).SetEase(Ease.OutCubic).ToUniTask();
            hook.parent = stableParent;
        }

        public async UniTask pullHook()
        {
            var newHookPosition = transform.position + Vector3.down;
            await hookForLine.DOMove(newHookPosition, 0.5f).SetEase(Ease.OutCubic).ToUniTask();
            hookForLine = hook;
            hook.parent = transform.parent;
        }
        
        private void Update()
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, hookForLine.position);
        }
    }
}