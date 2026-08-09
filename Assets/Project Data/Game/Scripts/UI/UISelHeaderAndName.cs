using System;
using System.Collections.Generic;
using SuperScrollView;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Watermelon.elems;
using Watermelon.Message;
using Button = UnityEngine.UI.Button;

namespace Watermelon
{
    public class UISelectHeadIconAndName : MonoBehaviour
    {
        private static readonly List<string> HeaderIcons = new List<string>()
        {
            "tile_1",
            "tile_2",
            "tile_3",
            "tile_4",
            "tile_5",
            "tile_6",
            "tile_7",
            "tile_8",
        };
        
        public TextMeshProUGUI TitleText;
        public TextMeshProUGUI RoleNameInput;
        public LoopGridView HeadIconView;
        
        public Button confirmBtn;
        public Button RandomRoleNameBtn;
        
        public Action<string, string> OnOver;

        private string SelHeaderName;
        
        private bool isInit = false;
        
        public void Init()
        {
            if (isInit)
            {
                return;
            }
            
            isInit = true;
            
            HeadIconView.InitGridView(HeaderIcons.Count, this.GetHeadIconItem);
            
            if (confirmBtn != null)
            {
                confirmBtn.onClick.RemoveAllListeners();
                confirmBtn.onClick.AddListener(this.OnConfirmBtnClick);
            }
            
            if (RandomRoleNameBtn != null)
            {
                RandomRoleNameBtn.onClick.RemoveAllListeners();
                RandomRoleNameBtn.onClick.AddListener(this.RandomRoleName);
            }
        }
        
        public void Show(string oldHeader, string oldNick)
        {
            SelHeaderName = oldHeader;
            RoleNameInput.text = oldNick;
            if (string.IsNullOrEmpty(oldNick))
            {
                TitleText.text = "创建角色";
                RandomRoleName();
            }
            else
            {
                TitleText.text = "变更角色";
            }

            HeadIconView.RefreshAllShownItem();
        }

        private LoopGridViewItem GetHeadIconItem(LoopGridView gridView, int index, int row, int column)
        {
            if (index < 0 || index >= HeaderIcons.Count)
            {
                return null;
            }

            var data = HeaderIcons[index];
            if (data == null)
            {
                return null;
            }

            var item = gridView.NewListViewItem("HeadIcon");
            var headIcon = item.GetComponent<HeadIcon>();
            
            headIcon.OnSelect = OnHeadIconClicked;
            headIcon.SetData(data, SelHeaderName == data);

            return item;
        }
        
        private void OnHeadIconClicked(string headName)
        {
            if (headName == null)
            {
                return;
            }

            SelHeaderName = headName;

            HeadIconView.RefreshAllShownItem();
        }
        
        private void RandomRoleName()
        {
            RoleNameInput.text = RandomNameHelper.GetRandomName();
        }

        private void OnConfirmBtnClick()
        {
            this.OnOver?.Invoke(SelHeaderName, RoleNameInput.text);
        }
    }
}
