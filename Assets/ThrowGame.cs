using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using DefaultNamespace.game;
using DG.Tweening;
using UnityEngine;

public class ThrowGame : MonoBehaviour
{
    public GameObject bg;
    public GameObject pointer;
    public GameObject target;

    public float speedInSec = 1f;
    
    private float bgwidth;
    private FishingFlow fishFlow;
    private float targetwidth;

    private void Awake()
    {
        DI.sceneScope.register(this);
    }

    private void Start()
    {
        var bgSr = bg.GetComponent<SpriteRenderer>();
        var targetSr = target.GetComponent<SpriteRenderer>();
        
        bgwidth = bgSr.sprite.bounds.size.x;
        targetwidth = targetSr.sprite.bounds.size.x;
        fishFlow = DI.sceneScope.getInstance<FishingFlow>();
        gameObject.SetActive(false);
    }

    public async UniTask<int> StartGame()
    {
        gameObject.SetActive(true);
        var cancellationSource = new CancellationTokenSource();
        pointer.transform.DOMoveX(bg.transform.position.x + bgwidth / 2f, speedInSec)
            .From(bg.transform.position.x - bgwidth / 2f).SetLoops(-1, LoopType.Yoyo).ToUniTask().AttachExternalCancellation(cancellationSource.Token).Forget();
        await fishFlow.onClickAwaitable();
        cancellationSource.Cancel();
        if (pointer.transform.position.x > target.transform.position.x - targetwidth / 2f &&
            pointer.transform.position.x < target.transform.position.x + targetwidth / 2f)
        {
            gameObject.SetActive(false);
            return 2;
        } else {
            gameObject.SetActive(false);
            return 1;
        }
    } 
}
