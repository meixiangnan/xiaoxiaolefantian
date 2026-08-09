using System;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon.elems
{
    public class HeadIcon : MonoBehaviour
    {
        public Button button;
        public Image ImageHead;
        public GameObject SelectMask;
        
        private string HeadName;

        private bool canSelect = true;
        private bool isListenerAdded = false;
        
        public Action<string> OnSelect;
        
        private void EnsureListenerAdded()
        {
            if (!isListenerAdded && button != null)
            {
                isListenerAdded = true;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(OnButtonClick);
            }
        }

        private void OnButtonClick()
        {
            OnSelect?.Invoke(HeadName);
        }

        public void SetData(string headName, bool isSelect = false)
        {
            if (SelectMask != null)
            {
                SelectMask.SetActive(false);
            }
            
            if (string.IsNullOrEmpty(headName) || headName.Length < 5) 
            {
                return;
            }

            HeadName = headName;
            
            if (ImageHead != null)
            {
                ImageHead.sprite = GetSprite(headName);
            }
            
            if (SelectMask != null)
            {
                if (canSelect)
                {
                    SelectMask.SetActive(isSelect);
                }
                else
                {
                    SelectMask.SetActive(false);
                }
            }
            
            EnsureListenerAdded();
        }

        public void SetCanSelect(bool can)
        {
            this.canSelect = can;
            if (SelectMask != null && !can)
            {
                SelectMask.SetActive(false);
            }
        }

        public void SetSelected(bool isSelect)
        {
            if (SelectMask != null)
            {
                if (canSelect)
                {
                    SelectMask.SetActive(isSelect);
                }
                else
                {
                    SelectMask.SetActive(false);
                }
            }
        }
        
        private Sprite GetSprite(string headName)
        {
            return HeadIconController.GetHeadIcon(headName);
        }
    }
}
