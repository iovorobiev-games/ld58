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
    private float speed = 0.3f;
    private bool isAlive = true;
    private bool metwall = false;
    private SpriteRenderer spriteRenderer;
    private Hook hook;
    private RodString rodString;
    public FishData data { get; set; }
    public int defaultSecondsBiting = 10;
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
        data = FishDB.getRandomFish();
        audioSource = GetComponent<AudioSource>();
        var spritesheet = SpriteLoader.load("Sprites", "sprites");

        spriteRenderer.sprite = spritesheet.getAtSuffix(data.SpritePath);
        move().Forget();
        if (isAlive)
        {
            spriteRenderer.color = Color.black.WithAlpha(0.75f);;
        }
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
            currentDirection = -currentDirection;
            metwall = false;
            return;
        }
        var xAxis = Random.Range(0f, 1f);
        var xDir = 0f;
        if (xAxis < 0.35f)
        {
            xDir = -1f;
        }
        else if (xAxis < 0.7f)
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

        if (xDir != 0)
        {
            transform.localScale = new Vector3(-1 * xDir, transform.localScale.y, transform.localScale.z);
        }

        currentDirection = new Vector3(xDir, yDir, 0f);
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
            Debug.Log(id + " is moving to " +transformPosition);
            await transform.DOMove(transformPosition, randomDistance)
                .SetEase(Ease.OutCubic).ToUniTask().AttachExternalCancellation(moveSource.Token);
            Debug.Log(id + " finished moving to " +transformPosition);
        }
        
    }

    public async void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("walls") || other.gameObject.layer == LayerMask.NameToLayer("water"))
        {
            metwall = true;
            transform.DOKill();
            moveSource?.Cancel();
            // isAlive = false;
            Debug.Log("Collision enter wall");
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Hook"))
        {
            await HandleFishHooking();
        }
        // else
        // {
        //     metwall = true;
        //     transform.DOKill();
        //     moveSource?.Cancel();
        //     // Debug.Log("Collision enter not wall " + LayerMask.LayerToName(other.gameObject.layer));
        // }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Collision happened");
    }

    private async UniTask HandleFishHooking()
    {
        Debug.Log(id + " The fish is hooked!");
        var bait = hook.getBait();
        
        if (bait.BaitStrength < data.Cunning || hook.HasPrey)
        {
            return;
        }
        isAlive = false;

        transform.DOKill();
        moveSource?.Cancel();
        Debug.Log(id + " Moving to hook");
        var distance = Vector3.Distance(transform.position, hook.transform.parent.position);
        Debug.Log(id + " Distance to hook: " + distance);
        drawGizmoToHook = true;
        var moveToHook = transform
            .DOMove(hook.transform.parent.position,
                distance / (speed * 2f)).SetEase(Ease.Linear)
            .ToUniTask(); 
        drawGizmoToHook = false;
        var result = await UniTask.WhenAny(moveToHook, hook.awaitIsHooked());
        Debug.Log(id + " Move finished because of " + result);
        if (rodString.isHookThrown() && result == 0)
        {
            audioSource.PlayOneShot(nomSounds[Random.Range(0, nomSounds.Count)]);;
            hook.hooked(this);
            transform.parent = hook.transform.parent;
            Debug.Log("setting Position " + hook.transform.parent.position);
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
        await UniTask.Never(source.Token);
        await UniTask.WaitForSeconds((float)defaultSecondsBiting / data.Cunning).AttachExternalCancellation(source.Token);
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