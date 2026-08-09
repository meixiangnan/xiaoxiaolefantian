using System.Collections.Generic;
using UnityEngine;
using Watermelon.Map;

namespace Watermelon.MainMap
{
    public class ChapterLevelGroup : MonoBehaviour
    {
        private int StartLevel = 1;
        private int EndLevel = 6;

        public List<MapLevelBehavior> levelEntrys = new();

        public static readonly int LevelMaxNum = 10;

        public void SetData(int start, int end)
        {
            StartLevel = start;
            EndLevel = end;

            for (var i = 0; i < levelEntrys.Count; i++)
            {
                var level = levelEntrys[i];
                var setLevelId = i + StartLevel;
                if (setLevelId <= EndLevel)
                {
                    level.Init(setLevelId);
                    level.gameObject.SetActive(true);
                }
                else
                {
                    level.gameObject.SetActive(false);
                }
            }
        }
    }
}
