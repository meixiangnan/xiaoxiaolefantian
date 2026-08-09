using System;
using System.Collections.Generic;
using SuperScrollView;
using TMPro;
using UnityEngine;
using Watermelon.elems;
using Watermelon.Message;
using Button = UnityEngine.UI.Button;

namespace Watermelon
{
    public class UIGameDispatch : MonoBehaviour
    {
        public TextMeshProUGUI Account;
        public HeadIcon Header;
        
        public Button GoLevelBtn;
        public Button LeaderBoardBtn;
        public Button LogoutBtn;
        public Button QuitGameBtn;
        public Button ChangeNameBtn;
        public Button ShopBtn;
        public Button HeroBookBtn;
        public Button SettingBtn;
        public UISettingPanal SettingPanel;
        
        private List<LeadBoardInfo> curData;
        private LeadBoardInfo myData;

        public Action ClickGoLevelBtn;
        public Action ClickLeaderBoardBtn;
        public Action ClickLogoutBtn;
        public Action ClickChangeNameBtn;
        
        
        void Start()
        {
            GoLevelBtn.onClick.AddListener(this.OnGoLevel);
            LeaderBoardBtn.onClick.AddListener(this.OnLeaderBoard);
            LogoutBtn.onClick.AddListener(this.OnLogout);
            QuitGameBtn.onClick.AddListener(this.OnQuitGame);
            ChangeNameBtn.onClick.AddListener(this.OnChangeName);
            ShopBtn.onClick.AddListener(this.OnShop);
            HeroBookBtn.onClick.AddListener(this.OnHeroBook);
            if (SettingBtn != null)
            {
                SettingBtn.onClick.AddListener(this.OnSetting);
            }
            if (SettingPanel != null)
            {
                SettingPanel.gameObject.SetActive(false);
            }
            
            Header.SetCanSelect(false);
            Header.button.onClick.RemoveAllListeners();
            Header.button.onClick.AddListener(this.OnChangeName);
            
            this.InitData();
        }

        public void InitData()
        {
            var mdl = GameGlobal.Instance.GetModule<RoleModule>();
            Header.SetData(mdl.userData.HeadIcon, false);
            Account.text = mdl.userData.Nickname;
        }

        private void OnGoLevel()
        {
            this.ClickGoLevelBtn?.Invoke();    
        }
        private void OnLeaderBoard()
        {
            this.ClickLeaderBoardBtn?.Invoke();    
        }
        private void OnLogout()
        {
            this.ClickLogoutBtn?.Invoke(); 
        }
        private void OnChangeName()
        {
            this.ClickChangeNameBtn?.Invoke(); 
        }
        private void OnQuitGame()
        {
            Application.Quit(0);    
        }
        private void OnShop()
        {
            UIController.ShowPage<UIShop>();
        }

        private void OnHeroBook()
        {
            UIController.ShowPage<UIHeroBook>();
        }

        private void OnSetting()
        {
            if (SettingPanel == null)
            {
                return;
            }

            SettingPanel.gameObject.SetActive(true);
            SettingPanel.Init();
        }
    }
}
