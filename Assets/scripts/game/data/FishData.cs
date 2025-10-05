using UnityEngine;

namespace DefaultNamespace.game
{
    public class FishData
    {
        public string Name
        {
            get;
        }

        public string SpritePath
        {
            get;
        }
        
        public string IconPath
        {
            get;
        }


        public string Description;
        public int Cunning
        {
            get;
        }

        public int Strength
        {
            get;
        }

        public int Rarity
        {
            get;
        }

        public int BaitStrength
        {
            get;
        }

        public string HiddenDescription
        {
            get;
        }

        public int Depth
        {
            get;
        }

        public FishData(string name = null, string spritePath = null, string hiddenDescription = null, string iconPath = null, int cunning = default, int strength = default, int rarity = default, int baitStrength = default, string description = null, int depth = default)
        {
            Name = name;
            Cunning = Mathf.Max(cunning, 1);
            Strength = strength;
            Rarity = rarity;
            BaitStrength = baitStrength;
            SpritePath = spritePath;
            IconPath = iconPath;
            Description = description;
            HiddenDescription = hiddenDescription;
            Depth = depth;
        }
    }
}