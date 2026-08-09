using System;
using UnityEngine;

namespace Watermelon
{
    [Serializable]
    public class HeroBookData
    {
        public int heroId;
        public string heroName;
        [TextArea(2, 5)]
        public string heroDescription;
        public Sprite heroSprite;
        public Sprite heroDetailSprite;
        public bool defaultUnlocked;
    }
}
