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

        public FishData(string name = null, string spritePath = null, int cunning = default, int strength = default, int rarity = default, int baitStrength = default)
        {
            Name = name;
            Cunning = cunning;
            Strength = strength;
            Rarity = rarity;
            BaitStrength = baitStrength;
            SpritePath = spritePath;
        }
    }
}