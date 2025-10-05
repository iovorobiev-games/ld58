using System;
using template.sprites;
using TMPro;
using UnityEngine;
using utils;

namespace DefaultNamespace.game
{
    public class InventoryCell : AwaitableClickHandlerImpl
    {
        public SpriteRenderer sprite;
        public TMP_Text countText;
        private SpriteLoader.SpriteSheet spriteSheet;

        private void Start()
        {
            spriteSheet = SpriteLoader.load("Sprites", "sprites");
        }

        public void setFishData(FishData fishData, int count)
        {
            if (count < 0)
            {
                countText.gameObject.SetActive(false);
            }
            else
            {
                countText.gameObject.SetActive(true);
                countText.text = count.ToString();
            }
            sprite.sprite = spriteSheet.getAtSuffix(fishData.IconPath);
        }
    }
}