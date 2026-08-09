using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public enum DialogState
    {
        Notice,
        NoticeConfirmOnly,
        QuitGame,
    }

    public class NotifyDialog : MonoBehaviour
    {
        private static NotifyDialog sDialog;

        [SerializeField] public Button okBtn;
        [SerializeField] public Button closeBtn;
        [SerializeField] public ScrollRect scrollRect;
        [SerializeField] public TextMeshProUGUI title;
        [SerializeField] public TextMeshProUGUI content;
        
        private bool isInit = false;
        private Action onClose;
        private TextAlignmentOptions defaultContentAlignment;
        public void Initialise()
        {
            sDialog = this;
            defaultContentAlignment = content.alignment;
            sDialog.gameObject.SetActive(false);
        }

        public static void NotifyClose(DialogState state, string title, string content)
        {
            NotifyClose(state, title, content, null);
        }

        public static void NotifyClose(DialogState state, string title, string content, Action onClose)
        {
            NotifyClose(state, title, content, onClose, sDialog.defaultContentAlignment);
        }

        public static void NotifyClose(DialogState state, string title, string content, Action onClose, TextAlignmentOptions contentAlignment)
        {
            sDialog.onClose = onClose;
            sDialog.SetData(title, content, contentAlignment);
            sDialog.SetState(state);
            sDialog.gameObject.SetActive(true);
        }

        public static void CloseActive()
        {
            if (sDialog == null)
            {
                return;
            }

            sDialog.onClose = null;
            sDialog.gameObject.SetActive(false);
        }

        private void SetState(DialogState state)
        {
            closeBtn.onClick.RemoveAllListeners();
            okBtn.onClick.RemoveAllListeners();
            
            if (state == DialogState.Notice)
            {
                okBtn.gameObject.SetActive(true);
                closeBtn.gameObject.SetActive(true);
                
                closeBtn.onClick.AddListener(this.OnClickCloseBtn);
                okBtn.onClick.AddListener(this.OnClickCloseBtn);
            }
            else if (state == DialogState.NoticeConfirmOnly)
            {
                okBtn.gameObject.SetActive(true);
                closeBtn.gameObject.SetActive(false);
                
                okBtn.onClick.AddListener(this.OnClickCloseBtn);
            }
            else //if (state != DialogState.QuitGame)
            {
                okBtn.gameObject.SetActive(true);
                closeBtn.gameObject.SetActive(false);
                
                okBtn.onClick.AddListener(this.QuitGame);
            }
        }

        public void SetData(string _title, string _content)
        {
            SetData(_title, _content, defaultContentAlignment);
        }

        public void SetData(string _title, string _content, TextAlignmentOptions contentAlignment)
        {
            if (!isInit)
            {
                isInit = true;
            }
            
            title.text = _title;
            content.text = _content;
            content.alignment = contentAlignment;
            scrollRect.normalizedPosition = new Vector2(scrollRect.normalizedPosition.x, 1f);
        }
        
        private void OnClickCloseBtn()
        {
            sDialog.gameObject.SetActive(false);
            Action callback = onClose;
            onClose = null;
            callback?.Invoke();
        }

        private void QuitGame()
        {
            ITutorial tutorial = TutorialController.GetTutorial(TutorialID.FirstLevel);
            if(tutorial != null)
            {
                FirstLevelTutorial firstLevelTutorial = (FirstLevelTutorial)tutorial;
                firstLevelTutorial.Unload();
            }
            
            sDialog.gameObject.SetActive(false);
            onClose = null;
            LevelController.UnloadLevel();
            GameController.OnLevelCancel();
            UIController.HidePage<UIMainMenu>();
            UIController.HidePage<UIGame>();
            UIController.HidePage<UIGameOver>();
            UIController.HidePage<UIComplete>();
            
            
            UIController.ShowPage<UISDKLogin>(new ShowUISDKLoginParam(){ InitState = UILoginState.Login});
            
        }
    }
}