using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace.game
{
    public static class FishDB
    {
        public static HashSet<FishData> allFish = new()
        {
            new FishData(
                "Stardiva", 
                "10",
                hiddenDescription: 
                "Stardiva is mispronounced stavrida, which is a Trachurus in Russian", 
                iconPath: "30", 
                cunning:1, 
                baitStrength: 7, 
                depth:0,
                description: "Stardiva is a decent bait, although still not for all the fish."
                ),
            new FishData(
                "Bubblington", 
                "8", 
                iconPath: 
                "26", 
                strength:2, 
                depth: 1,
                hiddenDescription: "Bubblington was the first fish that I drew for this game",
                description: "Bubblington has more air than meat, other fish woudn't bother to catch it."),
            new FishData("Somus", 
                "11", 
                iconPath: "27", 
                strength:3, 
                depth: 2,
                hiddenDescription: "Catfish (which is an inspiration for somus) are named like this because of prominent barbels, resembling cat's whiskers.",
                cunning: 8, description: "Somus are too big and terrifying to be a bait."),
            new FishData(
                "Beauty-poppy",
                "12", 
                iconPath:"31", 
                baitStrength: 10, 
                cunning: 6, 
                depth: 1,
                hiddenDescription: "This fish has nothing to do with poppy, but it just sounds cute",
                description: "Bright beauty-poppy is especially attractive, no fish will miss a chance to bite it."),
            new FishData(
                "Punkfish", 
                "9", 
                iconPath:"32", 
                cunning:9, 
                depth: 2,
                hiddenDescription: "Punkfish doesn't care it can't be used as bait. It's just cool to be poisonous.",
                description: "Punkfish are poisonous, it is unlikely to attract any fish with it."),
        };
        
        public static FishData canOfWorms = new FishData(
            "Worms", 
            "21", 
            iconPath: "21",
            description: "Worms attract small fish. They also never end.",
            baitStrength: 5);

        public static FishData getRandomFish()
        {
            return allFish.ToList()[Random.Range(0, allFish.Count)];
        }

        public static FishData getRandomFishOfDepth(int depth)
        {
            var allFishOfDepth = allFish.Where((data) => data.Depth == depth).ToList();
            return allFishOfDepth[Random.Range(0, allFishOfDepth.Count)];
        }
    }
}