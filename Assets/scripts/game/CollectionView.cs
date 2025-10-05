using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using DefaultNamespace.game;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;

public class CollectionView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Vector3 hiddenPos;
    public Vector3 shownPos;
    
    private bool isShown = false;
    public bool enabled;

    private Tween hoverTween;
    private Tween exitHoverTween;

    public List<CollectionItem> items = new();

    void Awake()
    {
        DI.sceneScope.register(this);
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public async UniTask addData(FishData data)
    {
        await showView();
        var revealTasks = items.Select(item => item.revealIfNeeded(data)).ToList();
    
        await UniTask.WhenAll(revealTasks);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isShown) return;
        hoverTween?.Kill();
        exitHoverTween?.Kill();
        transform.DOMove(hiddenPos + Vector3.up * 0.5f, 0.2f);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (isShown) return;
        hoverTween?.Kill();
        exitHoverTween?.Kill();
        transform.DOMove(hiddenPos, 0.2f);
    }

    public async void OnPointerClick(PointerEventData eventData)
    {
        if (!enabled) return;
        if (isShown)
        {
            Debug.Log("click hide");
            await hideView();
        }
        else
        {
            Debug.Log("click show");

            await showView();
        }
    }

    public async UniTask showView()
    {
        Debug.Log("show");
        transform.DOKill();
        isShown = true;
        await transform.DOMove(shownPos, 0.2f).ToUniTask();
    }

    public async UniTask hideView()
    {
        Debug.Log("hide");
        transform.DOKill();
        await transform.DOMove(hiddenPos, 0.2f).ToUniTask();
        isShown = false;
    }
}
