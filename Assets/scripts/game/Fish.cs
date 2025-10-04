using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using DefaultNamespace.game;
using DG.Tweening;
using template.sprites;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using ColorUtility = Unity.VisualScripting.ColorUtility;
using Random = UnityEngine.Random;

public class Fish : MonoBehaviour
{
    private Vector3 initDirection;
    private Vector3 currentDirection;
    private float speed = 0.3f;
    private bool isAlive = true;
    private bool metTop = false;
    private SpriteRenderer spriteRenderer;
    private Hook hook;
    private RodString rodString;
    private FishData data;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hook = DI.sceneScope.getInstance<Hook>();
        rodString = DI.sceneScope.getInstance<RodString>();
        SetDirection(Vector3.left);
        SetData(new FishData("Stavrida!", "9"));
        var spritesheet = SpriteLoader.load("Sprites", "sprites");

        spriteRenderer.sprite = spritesheet.getAtSuffix(data.SpritePath);
        move().Forget();
        if (isAlive)
        {
            spriteRenderer.color = ColorUtility.WithAlpha(Color.black, 0.75f);;
        }
    }

    public void SetData(FishData data)
    {
        this.data = data;
    }

    public void SetDirection(Vector3 direction)
    {
        initDirection = direction;
        currentDirection = initDirection;
    }

    private void changeDecision()
    {
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
        if (yAxis < 0.25f || metTop)
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

        if (metTop)
        {
            metTop = false;
        }

        currentDirection = new Vector3(xDir, yDir, 0f);
    }

    private async UniTask move()
    {
        while (isAlive)
        {
            Debug.Log("Changing decision ");
            changeDecision();
            var randomDistance = Random.Range(2f, 5f);
            await transform.DOMove(transform.position + currentDirection * speed * randomDistance, randomDistance)
                .SetEase(Ease.OutCubic).ToUniTask();
        }
        Debug.Log("Is alive false ");
    }

    public async void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("walls"))
        {
            isAlive = false;
            Debug.Log("Collision enter wall");
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Hook"))
        {
            await HandleFishHooking();
        }
        else
        {
            metTop = true;
            transform.DOKill();
            Debug.Log("Collision enter not wall " + LayerMask.LayerToName(other.gameObject.layer));
        }
    }

    private async UniTask HandleFishHooking()
    {
        Debug.Log("The fish is hooked!");
        var bait = hook.getBait();
        
        if (bait.BaitStrength < data.Cunning || hook.HasPrey)
        {
            return;
        }

        transform.DOKill();
        isAlive = false;
        Debug.Log("Moving to hook");
        var moveToHook = transform
            .DOMove(hook.transform.position,
                Vector3.Distance(transform.position, hook.transform.position) / speed).SetEase(Ease.OutCubic)
            .ToUniTask();

        var result = await UniTask.WhenAny(moveToHook, hook.awaitIsHooked());
        Debug.Log("Move finished because of " + result);
        if (rodString.isHookThrown() && result == 0)
        {
            hook.hooked(data);
            transform.parent = hook.transform;
            DOTween.Sequence().Append(
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
            isAlive = true;
            move().Forget();
        }
    }
}