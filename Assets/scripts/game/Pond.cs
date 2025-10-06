using System.Collections.Generic;
using System.Linq;

namespace DefaultNamespace.game
{
    public class Pond
    {
        public List<FishData> fish = new();

        public int getFishCount(string name)
        {
            return fish.Count(data => data.Name == name);
        }
    }
}