using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class UIHeroBookItem : MonoBehaviour
    {
        [SerializeField] Image heroIcon;
        [SerializeField] GameObject lockObj;
        [SerializeField] TextMeshProUGUI heroNameText;
        [SerializeField] TextMeshProUGUI heroIdText;
        [SerializeField] Button clickButton;

        private HeroBookData data;
        private int heroId;
        public int HeroId => heroId;

        public Action<HeroBookData> OnClickHero;

        private void Awake()
        {
            if (clickButton == null)
            {
                clickButton = GetComponent<Button>();
            }

            if (clickButton != null)
            {
                clickButton.onClick.AddListener(OnClick);
            }
        }

        public void SetData(HeroBookData data, bool unlocked)
        {
            this.data = data;
            heroId = data.heroId;

            if (heroIcon != null)
            {
                heroIcon.sprite = data.heroSprite;
            }

            if (lockObj != null)
            {
                lockObj.SetActive(!unlocked);
            }

            if (heroNameText != null)
            {
                heroNameText.text = data.heroName;
            }

            if (heroIdText != null)
            {
                heroIdText.text = data.heroId.ToString();
            }
        }

        private void OnClick()
        {
            if (data != null)
            {
                OnClickHero?.Invoke(data);
            }
        }
    }
}
