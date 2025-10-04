using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace.game
{
    public static class FishDB
    {
        public static HashSet<FishData> allFish = new()
        {
            new FishData("Stardiva", "10", 1),
            new FishData("Bubblington", "8", strength:2),
            new FishData("Somus", "11", strength:3),
            new FishData("Beauty-poppy", "12"),
            new FishData("Punkfish", "9", cunning:6),
        };

        public static FishData getRandomFish()
        {
            return allFish.ToList()[Random.Range(0, allFish.Count)];
        }
    }
}