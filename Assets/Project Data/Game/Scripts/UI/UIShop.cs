using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    [RequireComponent(typeof(Canvas)), RequireComponent(typeof(GraphicRaycaster))]
    public class UIShop : UIPage
    {
        [SerializeField] Button returnBtn;
        [SerializeField] Button diamondBtn;
        [SerializeField] Button itemBtn;
        
        [SerializeField] GameObject diamondPanel;
        [SerializeField] GameObject itemPanel;
        
        [SerializeField] TextMeshProUGUI currentDiamondCount;
        [SerializeField] UIShopDiamondItem[] diamondItems;
        [SerializeField] UIShopItem[] itemItems;

        public override void Initialise()
        {
            returnBtn.onClick.AddListener(OnReturn);
            diamondBtn.onClick.AddListener(OnDiamondBtn);
            itemBtn.onClick.AddListener(OnItemBtn);
            
            foreach (var item in diamondItems)
            {
                item.Init(RefreshDiamondCount);
            }

            foreach (var item in itemItems)
            {
                item.Init(RefreshDiamondCount);
            }
        }

        public override void PlayShowAnimation(object param = null)
        {
            RefreshDiamondCount();
            ShowDiamondPanel();
            UIController.OnPageOpened(this);
        }

        public override void PlayHideAnimation()
        {
            UIController.OnPageClosed(this);
        }

        private void OnReturn()
        {
            UIController.HidePage<UIShop>();
        }

        private void OnDiamondBtn()
        {
            ShowDiamondPanel();
        }

        private void OnItemBtn()
        {
            ShowItemPanel();
        }

        private void ShowDiamondPanel()
        {
            diamondPanel.SetActive(true);
            itemPanel.SetActive(false);
        }

        private void ShowItemPanel()
        {
            diamondPanel.SetActive(false);
            itemPanel.SetActive(true);
        }

        private void RefreshDiamondCount()
        {
            var diamondModule = GameGlobal.Instance.GetModule<DiamondModule>();
            currentDiamondCount.text = diamondModule.DiamondCount.ToString();
        }
    }
}
