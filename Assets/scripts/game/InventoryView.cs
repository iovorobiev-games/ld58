using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using utils;

namespace DefaultNamespace.game
{
    public class InventoryView : MonoBehaviour
    {
        private int offset;
        private bool shown;
        public List<InventoryCell> cells;

        public AwaitableClickHandlerImpl leftButton;
        public AwaitableClickHandlerImpl rightButton;

        private List<(FishData, int)> inventory;

        public void onShow(Player player)
        {
            offset = 0;
            Debug.Log("Populating");
            populateInventory(player.getSortedBaitInventory(), offset);
            Debug.Log("Populated");
            shown = true;
            Debug.Log("Listeneing for buttons");
            monitorLeftRight().Forget();
            Debug.Log("Finished listening for buttons");
            
        }

        public async UniTask onHide()
        {
            shown = false;
        }
        
        private async UniTask monitorLeftRight()
        {
            while (shown)
            {
                var result = await UniTask.WhenAny(leftButton.onClickAwaitable(), rightButton.onClickAwaitable());
                if (result == 0)
                {
                    offset = ((offset - 1) + inventory.Count) % inventory.Count;
                }
                else
                {
                    offset = (offset + 1) % inventory.Count;
                }
                populateInventory(inventory, offset);
            }
        }
        
        public void populateInventory(List<(FishData, int)> inventory, int offset)
        {
            this.inventory = inventory;
            
            for (var i = 0; i < cells.Count; i++)
            {
                cells[i].sprite.gameObject.SetActive(i < inventory.Count);
                var (fish, count) = inventory[(i + offset) % inventory.Count];
                cells[i].setFishData(fish, count);
            }
        }

        public async UniTask<FishData> selectBait()
        {
            var clickTasks = Enumerable.Select(cells, inventoryCell => inventoryCell.onClickAwaitable()).ToList();

            var selectedCell = await UniTask.WhenAny(clickTasks);
            return inventory[selectedCell].Item1;
        }
    }
}