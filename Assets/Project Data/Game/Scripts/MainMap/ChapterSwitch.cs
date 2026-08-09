using UnityEngine;
using UnityEngine.UI;

namespace Watermelon.MainMap
{
    public class ChapterSwitch : MonoBehaviour
    {
        public int ChapterIndex = 0;
        public Sprite bg;
        public Button btn;

        [Header("Chapter Icons")]
        public Image iconImage;
        public Sprite normalIcon;
        public Sprite selectedIcon;
        
        public delegate void SelectAction(int index, Sprite bg);
        public event SelectAction OnSelect;

        public void SetSelect()
        {
            OnSelect?.Invoke(ChapterIndex, bg);
        }

        public void SetUnlockVisual(bool unlocked)
        {
            if (iconImage == null)
            {
                return;
            }

            Sprite targetSprite = unlocked ? selectedIcon : normalIcon;
            if (targetSprite != null)
            {
                iconImage.sprite = targetSprite;
            }
        }

        void Start()
        {
            btn.onClick.AddListener(() =>
            {
                OnSelect?.Invoke(ChapterIndex, bg);
            });
        }
    }
}
