using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using DefaultNamespace.game;
using DG.Tweening;
using template.sprites;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;
using Sequence = DG.Tweening.Sequence;

public class Fish : MonoBehaviour
{
    private Vector3 initDirection;
    private Vector3 currentDirection;
    private float speed = 0.2f;
    private bool isAlive = true;
    private bool metwall = false;
    private SpriteRenderer spriteRenderer;
    private Hook hook;
    private RodString rodString;
    private FishData _data;
    public FishData data {
        get => _data;
        set
        {
            _data = value;
            speed *= Math.Max(value.Strength, 1);
        }
    }
    public int defaultSecondsBiting = 15;
    private int id;
    public List<AudioClip> nomSounds = new();

    private CancellationTokenSource source = new();
    private CancellationTokenSource moveSource = new();
    private Sequence caughtTween;
    private AudioSource audioSource;

    private Transform originParent;
    private bool drawGizmoToHook = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        id = Random.Range(0, 1000000);
        originParent = transform.parent;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        hook = DI.sceneScope.getInstance<Hook>();
        rodString = DI.sceneScope.getInstance<RodString>();
        SetDirection(Vector3.left);
        audioSource = GetComponent<AudioSource>();
        var spritesheet = SpriteLoader.load("Sprites", "sprites");

        spriteRenderer.sprite = spritesheet.getAtSuffix(data.SpritePath);
        Appear().Forget();
        move().Forget();
        if (isAlive)
        {
            spriteRenderer.color = Color.black.WithAlpha(0.75f);;
        }
    }

    public async UniTask Appear()
    {
        spriteRenderer.DOColor(Color.black.WithAlpha(0.75f), 0.25f).From(Color.black.WithAlpha(0f));
    }
    
    public void SetDirection(Vector3 direction)
    {
        initDirection = direction;
        currentDirection = initDirection;
    }

    private void changeDecision()
    {
        if (metwall)
        {
            currentDirection = -currentDirection.normalized;
            metwall = false;
            Debug.Log(id + "Met wall");
            return;
        }
        var xAxis = Random.Range(0f, 1f);
        var xDir = 0f;

        var rightProb = 0.7f;

        if (transform.position.x < -3f)
        {
            rightProb = 0.85f;
        } else if (transform.position.x > 2f)
        {
            rightProb = 0.15f;
        }
        
        if (xAxis < (1 - rightProb))
        {
            xDir = -1f;
        }
        else if (xAxis < rightProb)
        {
            xDir = 1f;
        }

        var yDir = 0f;
        var yAxis = Random.Range(0f, 1f);
        if (yAxis < 0.25f)
        {
            yDir = -1;
        }
        else if (yAxis < 0.5f)
        {
            yDir = 1;
        }

        currentDirection = new Vector3(xDir, yDir, 0f).normalized;
    }

    private async UniTask move()
    {
        while (isAlive)
        {
            moveSource?.Cancel();
            moveSource = new CancellationTokenSource();
            changeDecision();
            var randomDistance = Random.Range(2f, 5f);
            
            var transformPosition = transform.position + currentDirection * speed * randomDistance;
            await transform.DOMove(transformPosition, randomDistance)
                .SetEase(Ease.OutCubic).ToUniTask();
        }
        Debug.Log(id + " " + data.Name + " is not alive ");
    }

    private Vector3 lastFramePosition = Vector3.zero;

    public void Update()
    {
        transform.localScale = new Vector3(Mathf.Sign(lastFramePosition.x - transform.position.x), 1, 1);
        lastFramePosition = transform.position;
    }

    public async void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("walls") || other.gameObject.layer == LayerMask.NameToLayer("water"))
        {
            metwall = true;
            transform.DOKill();
            moveSource?.Cancel();
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Hook"))
        {
            await HandleFishHooking().SuppressCancellationThrow();
        }
        // else
        // {
        //     metwall = true;
        //     transform.DOKill();
        //     moveSource?.Cancel();
        //     // Debug.Log("Collision enter not wall " + LayerMask.LayerToName(other.gameObject.layer));
        // }
    }

    private async UniTask HandleFishHooking()
    {
        var bait = hook.getBait();

        
        if (bait.BaitStrength < data.Cunning || hook.HasPrey)
        {
            return;
        }
        
        isAlive = false;

        transform.DOKill();
        moveSource?.Cancel();
        var distance = Vector3.Distance(transform.position, hook.transform.parent.position);
        drawGizmoToHook = true;
        await UniTask.WaitForSeconds(Random.Range(1f / data.Cunning, 0.5f + 1f / data.Cunning));

        if (hook.HasPrey)
        {
            release().Forget();
        }
        
        var moveToHook = transform
            .DOMove(hook.transform.parent.position,
                distance / (speed * 2f)).SetEase(Ease.Linear)
            .ToUniTask(); 
        drawGizmoToHook = false;
        var result = await UniTask.WhenAny(moveToHook, hook.awaitIsHooked());
        if (rodString.isHookThrown() && result == 0)
        {
            audioSource.PlayOneShot(nomSounds[Random.Range(0, nomSounds.Count)]);;
            hook.hooked(this);
            transform.parent = hook.transform.parent;
            transform.position = hook.transform.parent.position;
            caughtTween = DOTween.Sequence().Append(
                DOTween.Sequence()
                    .Append(
                        transform.DORotate(Vector3.forward * 10, 0.1f).SetLoops(2, LoopType.Yoyo)
                    )
                    .Append(
                        transform.DORotate(Vector3.back * 10, 0.1f).SetLoops(2, LoopType.Yoyo)
                    ).SetLoops(Random.Range(2, 4), LoopType.Yoyo)
            ).AppendInterval(1f).SetLoops(-1, LoopType.Yoyo);
            
        }
        else
        {
            release().Forget();
        }
    }

    public async UniTask releaseWhenItWants()
    {
        source?.Cancel();
        source = new CancellationTokenSource();
        await UniTask.WaitForSeconds(10 + (float)(defaultSecondsBiting - 10) / Mathf.Max(data.Cunning, 1)).AttachExternalCancellation(source.Token);
    }

    public async UniTask release()
    {
        drawGizmoToHook = false;
        caughtTween?.Kill();
        transform.DOKill();
        transform.parent = originParent;
        isAlive = true;
        move().Forget();
    }
    
    public void Destroy()
    {
        drawGizmoToHook = false;
        isAlive = false;
        transform.DOKill();
        caughtTween?.Kill();
        source?.Cancel();
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if ( hook != null && hook.transform.parent != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, hook.transform.parent.position);
        }
    }
}