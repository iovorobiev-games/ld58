using System;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using DefaultNamespace.game;
using DG.Tweening;
using template.sprites;
using TMPro;
using UnityEngine;
using utils;

public class ResultWindow : MonoBehaviour
{
    public TMP_Text header;

    public SpriteRenderer icon;
    public TMP_Text label;
    public AwaitableClickHandlerImpl keepIt;

    public GameObject addToCollectionObject;
    public AwaitableClickHandlerImpl addToCollection;
    private SpriteLoader.SpriteSheet spriteSheet;
    private Player player;
    private CollectionView collectionView;

    public void Awake()
    {
        DI.sceneScope.register(this);
    }

    public void Start()
    {
        spriteSheet = SpriteLoader.load("Sprites", "sprites");
        player = DI.globalScope.getInstance<Player>();
        collectionView = DI.sceneScope.getInstance<CollectionView>();
        gameObject.SetActive(false);
    }

    public async UniTask showWith(FishData fishData)
    {
        if (player.hasInCollection(fishData))
        {
            Debug.Log("Player has " + fishData.Name + " in collection");;
            addToCollectionObject.gameObject.SetActive(false);
        }
        else
        {
            addToCollectionObject.gameObject.SetActive(true);
            Debug.Log("Player doesn't have " + fishData.Name + " in collection");
        }
        label.text = "<sketchy>" + fishData.Name + "</sketchy>";
        icon.sprite = spriteSheet.getAtSuffix(fishData.SpritePath);
        gameObject.SetActive(true);
        await transform.DOScale(Vector3.one, 0.2f).From(Vector3.zero).ToUniTask();
        var result = await UniTask.WhenAny(keepIt.onClickAwaitable(), addToCollection.onClickAwaitable());
        if (result == 0)
        {
            Debug.Log("Adding to inventory");
            player.addToInventory(fishData);
        }
        else
        {
            Debug.Log("Adding to collection");
            player.addFishToCollection(fishData);
            await collectionView.addData(fishData);
        }
        await transform.DOScale(Vector3.zero, 0.2f).From(Vector3.one).ToUniTask();
        gameObject.SetActive(false);
    }
}
