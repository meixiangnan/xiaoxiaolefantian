using System.Collections.Generic;
using SuperScrollView;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Watermelon
{
    [RequireComponent(typeof(Canvas)), RequireComponent(typeof(GraphicRaycaster))]
    public class UIHeroBook : UIPage
    {
        private static UIHeroBook instance;

        [SerializeField] Button returnBtn;
        [SerializeField] LoopGridView heroGridView;
        [SerializeField] string itemPrefabName = "HeroBookItem";
        [SerializeField] HeroBookData[] heroDatas;
        [SerializeField] int maxAutoHeroId = 30;

        [Header("Hero Detail")]
        [SerializeField] GameObject heroPanel;
        [SerializeField] Image heroImage;
        [SerializeField] TextMeshProUGUI heroNameText;
        [SerializeField] TextMeshProUGUI heroDescriptionText;
        [SerializeField] Button heroPanelReturnBtn;

        private void Awake()
        {
            instance = this;
        }

        public static HeroBookData GetHeroDataById(int heroId)
        {
            if (instance == null || instance.heroDatas == null) return null;
            foreach (var data in instance.heroDatas)
            {
                if (data.heroId == heroId) return data;
            }
            return null;
        }

        public override void Initialise()
        {
            returnBtn.onClick.AddListener(OnReturn);
            if (heroPanelReturnBtn != null)
            {
                heroPanelReturnBtn.onClick.AddListener(HideHeroPanel);
            }
            if (heroPanel != null)
            {
                heroPanel.SetActive(false);
            }
            heroGridView.InitGridView(GetHeroCount(), GetHeroItem);
        }

        public override void PlayShowAnimation(object param = null)
        {
            HideHeroPanel();
            heroGridView.SetListItemCount(GetHeroCount());
            heroGridView.RefreshAllShownItem();
            UIController.OnPageOpened(this);
        }

        public override void PlayHideAnimation()
        {
            UIController.OnPageClosed(this);
        }

        private int GetHeroCount()
        {
            return heroDatas == null ? 0 : heroDatas.Length;
        }

        private LoopGridViewItem GetHeroItem(LoopGridView gridView, int index, int row, int column)
        {
            if (index < 0 || index >= heroDatas.Length)
            {
                return null;
            }

            HeroBookData data = heroDatas[index];
            LoopGridViewItem item = gridView.NewListViewItem(itemPrefabName);
            UIHeroBookItem heroItem = item.GetComponent<UIHeroBookItem>();
            heroItem.OnClickHero = OnClickHeroItem;
            heroItem.SetData(data, IsHeroUnlocked(data.heroId, data.defaultUnlocked));
            return item;
        }

        private bool IsHeroUnlocked(int heroId, bool defaultUnlocked)
        {
            RoleModule roleModule = GameGlobal.Instance.GetModule<RoleModule>();
            return roleModule != null && roleModule.IsHeroUnlocked(heroId, defaultUnlocked);
        }

        public static string GetHeroUnlockKey(int heroId)
        {
            int userId = -1;
            RoleModule roleModule = GameGlobal.Instance.GetModule<RoleModule>();
            if (roleModule != null && roleModule.userData != null)
            {
                userId = roleModule.userData.UserId;
            }

            if (userId > 0)
            {
                return $"HeroUnlocked_{userId}_{heroId}";
            }

            return $"HeroUnlocked_{heroId}";
        }

        private void OnReturn()
        {
            UIController.HidePage<UIHeroBook>();
        }

        private void OnClickHeroItem(HeroBookData data)
        {
            if (data == null)
            {
                return;
            }

            if (heroImage != null)
            {
                heroImage.sprite = data.heroDetailSprite != null ? data.heroDetailSprite : data.heroSprite;
            }

            if (heroNameText != null)
            {
                heroNameText.text = data.heroName;
            }

            if (heroDescriptionText != null)
            {
                heroDescriptionText.text = data.heroDescription;
            }

            ShowHeroPanel();
        }

        private void ShowHeroPanel()
        {
            if (heroPanel != null)
            {
                heroPanel.SetActive(true);
            }
        }

        private void HideHeroPanel()
        {
            if (heroPanel != null)
            {
                heroPanel.SetActive(false);
            }
        }

#if UNITY_EDITOR
        [ContextMenu("重新生成英雄数据（会覆盖名字和描述）")]
        private void RegenerateHeroDatas()
        {
            List<HeroBookData> datas = new List<HeroBookData>();
            for (int i = 1; i <= maxAutoHeroId; i++)
            {
                string cardAssetPath = $"Assets/Project Data/Game/Images_new/hero_card/Collection_{i}.png";
                string detailAssetPath = $"Assets/Project Data/Game/Images_new/hero/hero_{i}.png";
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(cardAssetPath);
                Sprite detailSprite = AssetDatabase.LoadAssetAtPath<Sprite>(detailAssetPath);
                if (sprite == null)
                {
                    continue;
                }

                datas.Add(new HeroBookData
                {
                    heroId = i,
                    heroName = $"英雄{i}",
                    heroSprite = sprite,
                    heroDetailSprite = detailSprite,
                    defaultUnlocked = false
                });
            }

            heroDatas = datas.ToArray();
            EditorUtility.SetDirty(this);
        }

        [ContextMenu("刷新英雄图片（保留名字和描述）")]
        private void RefreshHeroSprites()
        {
            if (heroDatas == null) return;

            for (int i = 0; i < heroDatas.Length; i++)
            {
                int heroId = heroDatas[i].heroId;
                string cardAssetPath = $"Assets/Project Data/Game/Images_new/hero_card/Collection_{heroId}.png";
                string detailAssetPath = $"Assets/Project Data/Game/Images_new/hero/hero_{heroId}.png";
                heroDatas[i].heroSprite = AssetDatabase.LoadAssetAtPath<Sprite>(cardAssetPath);
                heroDatas[i].heroDetailSprite = AssetDatabase.LoadAssetAtPath<Sprite>(detailAssetPath);
            }

            EditorUtility.SetDirty(this);
        }
#endif
    }
}
