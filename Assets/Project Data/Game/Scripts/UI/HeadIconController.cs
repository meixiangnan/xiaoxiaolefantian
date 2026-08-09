using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class HeadIconController : MonoBehaviour
    {
        static HeadIconController instance;
        
        public List<Sprite> headIcons;

        public Dictionary<string, Sprite> headIconsMap;
        
        public static Sprite GetHeadIcon(string name)
        {
            if (instance.headIconsMap==null || !instance.headIconsMap.ContainsKey(name))
            {
                return instance.headIconsMap["tile_1"];
            }
            return instance.headIconsMap[name];
        }
        
        void Start()
        {
            instance = this;
            headIconsMap = new Dictionary<string, Sprite>();
            foreach (Sprite sprite in headIcons)
            {
                headIconsMap.Add(sprite.name, sprite);
            }
        }
    }
}