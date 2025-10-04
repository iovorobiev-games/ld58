using System;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using DefaultNamespace.game;
using DG.Tweening;
using template.sprites;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
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
    private FishData data;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hook = DI.sceneScope.getInstance<Hook>();
        SetDirection(Vector3.left);
        SetData(new FishData("Stavrida!", "9"));
        var spritesheet = SpriteLoader.load("Sprites", "sprites");

        spriteRenderer.sprite = spritesheet.getAtSuffix(data.SpritePath);
        move().Forget();
        if (isAlive)
        {
            spriteRenderer.color = Color.black.WithAlpha(0.75f);
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
            changeDecision();
            var randomDistance = Random.Range(2f, 5f);
            await transform.DOMove(transform.position + currentDirection * speed * randomDistance, randomDistance).SetEase(Ease.OutCubic).ToUniTask();
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("walls"))
        {
            isAlive = false;
            Debug.Log("Collision enter wall");
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("hook"))
        {
            hook.hooked(data);
        }
        else
        {
            metTop = true;
            transform.DOKill();
            Debug.Log("Collision enter not wall");
        }
    }
}

