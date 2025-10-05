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
        transform.DOKill();
        transform.DOMove(hiddenPos + Vector3.up * 0.5f, 0.2f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isShown) return;
        transform.DOKill();
        transform.DOMove(hiddenPos, 0.2f);
    }

    public async void OnPointerClick(PointerEventData eventData)
    {
        if (isShown)
        {
            await hideView();
        }
        else
        {
            await showView();
        }
    }

    private async UniTask showView()
    {
        transform.DOKill();
        await transform.DOMove(shownPos, 0.2f).ToUniTask();
        isShown = true;
    }

    private async UniTask hideView()
    {
        transform.DOKill();
        await transform.DOMove(hiddenPos, 0.2f).ToUniTask();
        isShown = false;
    }
}
