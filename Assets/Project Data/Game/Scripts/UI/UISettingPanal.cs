using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class UISettingPanal : MonoBehaviour
    {
        public Button closeBtn;
        public Button openSound;
        public Button closeSound;
        public Button openShakeBtn;
        public Button closeShakeBtn;
        public Button Logout;
        public Button QuitLevel;
        public Button OpenDevOverlayBtn;
        public GameObject DevOverlay;
        
        private bool isInit = false;

        public void SetDevOverlay(GameObject devOverlay)
        {
            DevOverlay = devOverlay;
        }

        public void Init()
        {
            if (!isInit)
            {
                isInit = true;
                closeBtn.onClick.AddListener(this.OnClickCloseBtn);
                openSound.onClick.AddListener(this.OnClickOpenSoundBtn);
                closeSound.onClick.AddListener(this.OnClickCloseSoundBtn);
                openShakeBtn.onClick.AddListener(this.OnClickOpenShakeBtn);
                closeShakeBtn.onClick.AddListener(this.OnClickCloseShakeBtn);
                Logout.onClick.AddListener(this.OnClickLogOutBtn);
                QuitLevel.onClick.AddListener(this.OnClickQuitLevelBtn);
                if (OpenDevOverlayBtn != null)
                {
                    OpenDevOverlayBtn.onClick.AddListener(this.OnClickOpenDevOverlayBtn);
                }
            }

            if (AudioController.GetSoundsVolume() > 0) {
                OnClickOpenSoundBtn();
            } else {
                OnClickCloseSoundBtn();
            }
            
            if (AudioController.GetMusicVolume() > 0) {
                OnClickOpenShakeBtn();
            } else {
                OnClickCloseShakeBtn();
            }
        }

        private void OnClickCloseBtn()
        {
            GameController.isGamePause = false;
            this.gameObject.SetActive(false);
        }
        private void OnClickOpenSoundBtn()
        {
            openSound.image.enabled = true;
            closeSound.image.enabled = false;
            AudioController.SetSoundsVolume(1);
        }
        private void OnClickCloseSoundBtn()
        {
            openSound.image.enabled = false;
            closeSound.image.enabled = true;
            AudioController.SetSoundsVolume(0);
        }
        private void OnClickOpenShakeBtn()
        {
            openShakeBtn.image.enabled = true;
            closeShakeBtn.image.enabled = false;
            AudioController.SetMusicVolume(1);
        }
        private void OnClickCloseShakeBtn()
        {
            openShakeBtn.image.enabled = false;
            closeShakeBtn.image.enabled = true;
            AudioController.SetMusicVolume(0);
        }

        private void OnClickOpenDevOverlayBtn()
        {
            if (DevOverlay != null)
            {
                DevOverlay.SetActive(true);
            }
        }

        private void OnClickLogOutBtn()
        {
            LevelController.UnloadLevel();
            GameController.OnLevelCancel();
            UIController.HidePage<UIMainMenu>();
            UIController.HidePage<UIGame>();
            UIController.HidePage<UIGameOver>();
            UIController.HidePage<UIComplete>();
            
            
            UIController.ShowPage<UISDKLogin>(new ShowUISDKLoginParam(){ InitState = UILoginState.GameDispatch});
        }
        private void OnClickQuitLevelBtn()
        {
            this.gameObject.SetActive(false); 
            LevelController.UnloadLevel();
            GameController.OnLevelCancel();
            
            UIController.HidePage<UIGame>();
            UIController.HidePage<UIGameOver>();
            UIController.HidePage<UIComplete>();
            UIController.ShowPage<UIMainMenu>();
        }


    }
}