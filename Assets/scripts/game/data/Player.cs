using System.Collections.Generic;

namespace DefaultNamespace.game
{
    public class Player
    {
        private int maxInventorySize = 5;
        private HashSet<FishData> collection = new();
        private Dictionary<FishData, int> inventory = new ();

        public void addToInventory(FishData fish)
        {
            var count = inventory.GetValueOrDefault(fish, 0);
            inventory[fish] = count + 1;
        }

        public bool hasSpace()
        {
            return inventory.Count < maxInventorySize;
        }

        public bool hasInCollection(FishData fish)
        {
            return collection.Contains(fish);
        }
        
        public void addFishToCollection(FishData fish)
        {
            collection.Add(fish);
        }

        public void removeFish(FishData fish)
        {
            if (inventory.ContainsKey(fish))
            {
                inventory[fish]--;
                if (inventory[fish] < 1)
                {
                    inventory.Remove(fish);
                }
            }
        }
        
        public List<(FishData, int)> getSortedBaitInventory()
        {
            var list = new List<(FishData, int)>();
            foreach (var (data, count) in inventory)
            {
                list.Add((data, count));
            }
            list.Insert(0, (FishDB.canOfWorms, -1));
            return list;
        }
    }
}