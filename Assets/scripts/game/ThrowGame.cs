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
    public SpriteRenderer target;
    public SpriteRenderer target2;
    public SpriteRenderer target3;
    public BoxCollider2D pointerCollider;

    public float speedInSec = 1f;
    
    private float bgwidth;
    private FishingFlow fishFlow;
    private float target1Start;
    private float target2Start;
    private float target3Start;
    private bool isStarted;

    private void Awake()
    {
        DI.sceneScope.register(this);
    }

    private void Start()
    {
        var bgSr = bg.GetComponent<SpriteRenderer>();
        var targetSr = target.GetComponent<SpriteRenderer>();
        
        bgwidth = bgSr.sprite.bounds.size.x;
        target1Start = target.transform.position.x - targetSr.sprite.bounds.size.x / 2f;
        target2Start = target2.transform.position.x - target2.sprite.bounds.size.x / 2f;
        target3Start = target3.transform.position.x - target3.sprite.bounds.size.x / 2f;
        
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
        if (pointerCollider.transform.position.x > target3Start)
        {
            gameObject.SetActive(false);
            return 4;
        } else if (pointerCollider.transform.position.x > target2Start)
        {
            gameObject.SetActive(false);
            return 3;
        } else if (pointerCollider.transform.position.x > target1Start)
        {
            gameObject.SetActive(false);
            return 2;
        } else {
            gameObject.SetActive(false);
            return 1;
        }
    } 
}
