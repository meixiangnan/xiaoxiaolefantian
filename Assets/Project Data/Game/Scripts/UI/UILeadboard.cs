using System;
using System.Collections.Generic;
using SuperScrollView;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Watermelon.Message;
using Button = UnityEngine.UI.Button;

namespace Watermelon
{
    public class UILeadBoard : MonoBehaviour
    {
        public LoopListView2 scrollView;
        public LeaderBoardItem MyRank;
        public Button closeBtn;
        
        private List<LeadBoardInfo> curData;
        private LeadBoardInfo myData;
        
        public Action OnClickCloseBtn;

        private bool isInit = false;

        public void InitData()
        {
            var mdl = GameGlobal.Instance.GetModule<RankModule>(); 
            this.curData = mdl.rankData;
            this.myData = mdl.myRandData;
            
            if (!isInit)
            {
                isInit = true;
                closeBtn.onClick.AddListener(this.ClosePanel);
                scrollView.InitListView(this.curData.Count, GetItemCount);
            }

            if (this.curData.Count > 0)
            {
                scrollView.ResetListView(true);
            }
            MyRank.SetData(myData, false);
        }
        
        private LoopListViewItem2 GetItemCount(LoopListView2 loopListView, int index)
        {
            
            if (index < 0 || index >= curData.Count) return null;

            LoopListViewItem2 item = loopListView.NewListViewItem("LeadboardItem");
            var _itemInfo = item.GetComponent<LeaderBoardItem>();
            _itemInfo.SetData(curData[index]);
            return item;
            
        }

        private void ClosePanel()
        {
            this.gameObject.SetActive(false);
            OnClickCloseBtn?.Invoke();
        }
    }
}