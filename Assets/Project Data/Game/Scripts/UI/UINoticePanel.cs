using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class UINoticePanel : MonoBehaviour
    {
        public Button closeBtn;
        public ScrollRect scrollRect;
        public TextMeshProUGUI title;
        public TextMeshProUGUI content;
        //public Text content;
        
        private bool isInit = false;
        private Action onClose;

        public void SetData(string _title, string _content)
        {
            SetData(_title, _content, null);
        }

        public void SetData(string _title, string _content, Action _onClose)
        {
            if (!isInit)
            {
                isInit = true;
                closeBtn.onClick.AddListener(this.OnClickCloseBtn);
            }
            
            onClose = _onClose;
            title.text = _title;
            content.text = _content;
            scrollRect.normalizedPosition = new Vector2(scrollRect.normalizedPosition.x, 1f);
        }
        
        

        private void OnClickCloseBtn()
        {
            this.gameObject.SetActive(false);
            Action callback = onClose;
            onClose = null;
            callback?.Invoke();
        }
    }
}