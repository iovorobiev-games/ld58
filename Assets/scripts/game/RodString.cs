using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace DefaultNamespace.game
{
    public class RodString : MonoBehaviour
    {
        private LineRenderer lineRenderer;
        private Hook hookBeh;
        public Transform hook;
        public Transform stableParent;
        private Transform hookForLine;

        private bool hookThrown;
        private void Awake()
        {
            DI.sceneScope.register(this);
        }

        private void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
            hookForLine = hook;
            hookBeh = DI.sceneScope.getInstance<Hook>();
        }

        public async UniTask throwHook(float depth, int power, int baitPower)
        {
            var newHookPosition = transform.position + Vector3.down * depth;
            hookForLine = stableParent;
            
            await hookForLine.DOMove(newHookPosition, 0.5f).SetEase(Ease.OutCubic).ToUniTask();
            hook.parent = stableParent;
            hookThrown = true;
        }

        public async UniTask pullHook()
        {
            hookThrown = false;
            var newHookPosition = transform.position + Vector3.down;
            hookBeh.cancelBiting();
            await hookForLine.DOMove(newHookPosition, 0.5f).SetEase(Ease.OutCubic).ToUniTask();
            hookForLine = hook;
            hook.parent = transform.parent;
            stableParent.position = Vector3.zero;
        }

        public bool isHookThrown()
        {
            return hookThrown;
        }
        
        private void Update()
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, hookForLine.position);
        }
    }
}