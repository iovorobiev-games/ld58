using System;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using DefaultNamespace.game;
using TMPro;
using UnityEngine;
using utils;

public class ChooseBait : MonoBehaviour
{
    private Player player;
    public TMP_Text description;
    public InventoryView inventory;
    public AwaitableClickHandlerImpl closeButton;
    public TMP_Text title;
    
    public void Awake()
    {
        DI.sceneScope.register(this);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = DI.globalScope.getInstance<Player>();
        gameObject.SetActive(false);
    }

    public async UniTask<FishData> chooseBait()
    {
        gameObject.SetActive(true);
        inventory.onShow(player);
        FishData bait = FishDB.canOfWorms;
        description.text = bait.Description;
        title.SetText("Use " + bait.Name);
        while (true)
        {
            var result = await UniTask.WhenAny(inventory.selectBait(), closeButton.onClickAwaitable());
            if (result.hasResultLeft)
            {
                bait = result.result;
                description.text = bait.Description;
                title.SetText("Use " + bait.Name);
            }
            else
            {
                player.removeFish(bait);
                title.SetText("Use " + bait.Name);
                await inventory.onHide();
                gameObject.SetActive(false);
                return bait;
            }
        }
        
    }
    
}
