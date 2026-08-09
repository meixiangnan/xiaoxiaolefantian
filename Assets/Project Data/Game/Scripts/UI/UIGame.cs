using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class UIGame : UIPage
    {
        [SerializeField] RectTransform safeAreaRectTransform;
        [SerializeField] CurrencyUIPanelSimple coinsPanel;
        [SerializeField] UILevelQuitPopUp quitPopUp;
        [SerializeField] UILevelNumberText levelNumberText;

        [SerializeField] PUUIController powerUpsUIController;
        public PUUIController PowerUpsUIController => powerUpsUIController;

        [SerializeField] UILevelQuitPopUp exitPopUp;
        [SerializeField] Button exitButton;
        [SerializeField] Button SettingButton;
        [SerializeField] UIFadeAnimation exitButtonFadeAnimation;

        [SerializeField] GameObject devOverlay;

        [LineSpacer("Tutorial")]
        [SerializeField] GameObject tutorialPanelObject;
        [SerializeField] TextMeshProUGUI tutorialTitleText;
        [SerializeField] TextMeshProUGUI tutorialDescriptionText;
        [SerializeField] Button tutorialSkipButton;

        public GameObject TimeLimitObj;
        public TextMeshProUGUI TimeLimitText;
        public UISettingPanal settingPanal;
        public GameObject titleBg;
        
        
        public override void Initialise()
        {
            coinsPanel.Initialise();
            
            exitButton.onClick.AddListener(ShowExitPopUp);
            exitButtonFadeAnimation.Hide(immediately: true);

            NotchSaveArea.RegisterRectTransform(safeAreaRectTransform);
            NotchSaveArea.RegisterRectTransform((RectTransform)tutorialPanelObject.transform);

            DevPanelEnabler.RegisterPanel(devOverlay);

            tutorialSkipButton.onClick.AddListener(OnTutorialSkipButtonClicked);
            SettingButton.onClick.AddListener(OnSettingPanelClicked);
            if (settingPanal != null)
            {
                settingPanal.SetDevOverlay(devOverlay);
            }
            
            tutorialSkipButton.gameObject.SetActive(false);
            tutorialTitleText.gameObject.SetActive(false);
            tutorialDescriptionText.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            exitPopUp.OnConfirmExitEvent += ExitPopUpConfirmExitButton;
            exitPopUp.OnCancelExitEvent += ExitPopCloseButton;
        }

        private void OnDisable()
        {
            exitPopUp.OnConfirmExitEvent -= ExitPopUpConfirmExitButton;
            exitPopUp.OnCancelExitEvent += ExitPopCloseButton;
        }

        #region Show/Hide

        public override void PlayShowAnimation(object param = null)
        {
            settingPanal.gameObject.SetActive(false);
            coinsPanel.Activate();
            exitButtonFadeAnimation.Show();

            
            
            UILevelNumberText.Show();
            if (titleBg != null) titleBg.SetActive(GameGlobal.Instance.GetModule<RoleModule>().IsTutorialOver());

            UIController.OnPageOpened(this);
        }

        public override void PlayHideAnimation()
        {
            coinsPanel.Disable();
            exitButtonFadeAnimation.Hide();

            UILevelNumberText.Hide();

            UIController.OnPageClosed(this);
        }

        public void UpdateLevelNumber(int levelNumber)
        {
            levelNumberText.UpdateLevelNumber(levelNumber);
        }
        #endregion

        public void ShowExitPopUp()
        {
            exitPopUp.Show();
            AudioController.PlaySound(AudioController.Sounds.buttonSound);
        }

        public void ExitPopCloseButton()
        {
            exitPopUp.Hide();
        }

        public void ExitPopUpConfirmExitButton()
        {
            UIController.HidePage<UIGame>();

            GameController.ReturnToMenu();

            exitPopUp.Hide();
        }

        public void UpdateTimeLeft(double leftSecond)
        {
            if (null == TimeLimitObj)
            {
                return;
            }

            if (leftSecond > 0)
            {
                if (!TimeLimitObj.active)
                {
                    TimeLimitObj.SetActive(true);
                }
                TimeSpan timeSpan = TimeSpan.FromSeconds(leftSecond);
                TimeLimitText.text = timeSpan.ToString("mm\\:ss");
            }
            else
            {
                if (TimeLimitObj.active)
                {
                    TimeLimitObj.SetActive(false);
                }
            }
        }

        #region Tutorial
        public void ActivateTutorial()
        {
            tutorialPanelObject.SetActive(true);

            exitButton.gameObject.SetActive(false);
            levelNumberText.gameObject.SetActive(false);
            SettingButton.gameObject.SetActive(false);

            powerUpsUIController.HidePanels();
            if (titleBg != null) titleBg.SetActive(false);
        }

        public void DisableTutorial()
        {
            tutorialPanelObject.SetActive(false);

            levelNumberText.gameObject.SetActive(true);
            SettingButton.gameObject.SetActive(true);
            if (titleBg != null) titleBg.SetActive(true);
        }

        public void SetTutorialText(string title, string description)
        {
            
            tutorialPanelObject.SetActive(true);
            tutorialTitleText.gameObject.SetActive(true);
            tutorialDescriptionText.gameObject.SetActive(true);
            
            tutorialTitleText.text = title;
            tutorialDescriptionText.text = description;

            tutorialTitleText.transform.localScale = Vector3.one * 0.6f;
            tutorialTitleText.transform.DOScale(1.0f, 0.3f).SetEasing(Ease.Type.BackOut);

            tutorialDescriptionText.transform.localScale = Vector3.one * 0.6f;
            tutorialDescriptionText.transform.DOScale(1.0f, 0.3f).SetEasing(Ease.Type.BackOut);
            
            SettingButton.gameObject.SetActive(false);
        }

        private void OnSettingPanelClicked()
        {
            this.settingPanal.gameObject.SetActive(true);
            this.settingPanal.Init();
            GameController.isGamePause = true;
        }

        private void OnTutorialSkipButtonClicked()
        {
            ITutorial tutorial = TutorialController.GetTutorial(TutorialID.FirstLevel);
            if(tutorial != null)
            {
                FirstLevelTutorial firstLevelTutorial = (FirstLevelTutorial)tutorial;
                firstLevelTutorial.OnSkipButtonClicked();
            }
        }
        #endregion

        #region Development

        public void ReloadDev()
        {
            GameController.ReplayLevel();
        }

        public void HideDev()
        {
            devOverlay.SetActive(false);
        }

        public void OnLevelInputUpdatedDev(string newLevel)
        {
            int level = -1;

            if (int.TryParse(newLevel, out level))
            {
                LevelSave levelSave = SaveController.GetSaveObject<LevelSave>("level");
                levelSave.DisplayLevelIndex = Mathf.Clamp((level - 1), 0, GameLevelConfig.TotalLevelCount - 1);
                levelSave.RealLevelIndex = levelSave.DisplayLevelIndex;

                GameController.ReplayLevel();
            }
        }

        public void SetCompletedLevelDev(string completedLevel)
        {
            if (!int.TryParse(completedLevel, out int level))
            {
                FloatingMessage.ShowMessage("关卡输入错误。");
                return;
            }

            level = Mathf.Clamp(level, 0, GameLevelConfig.TotalLevelCount);
            int nextLevel = Mathf.Clamp(level + 1, 1, GameLevelConfig.TotalLevelCount + 1);

            LevelSave levelSave = SaveController.GetSaveObject<LevelSave>("level");
            levelSave.MaxReachedLevelIndex = Mathf.Clamp(level, 0, GameLevelConfig.TotalLevelCount - 1);
            levelSave.DisplayLevelIndex = Mathf.Clamp(nextLevel - 1, 0, GameLevelConfig.TotalLevelCount - 1);
            levelSave.RealLevelIndex = levelSave.DisplayLevelIndex;
            levelSave.IsPlayingRandomLevel = false;

            var roleModule = GameGlobal.Instance.GetModule<RoleModule>();
            roleModule.PassLevel = nextLevel;
            roleModule.UnlockHeroesByCompletedLevel(level);

            SaveController.Save(true);
            StartCoroutine(GameGlobal.Instance.GetModule<RankModule>().UploadRoleData());
            FloatingMessage.ShowMessage($"已完成到第{level}关。");
        }

        public void ResetProgressDev()
        {
            LevelSave levelSave = SaveController.GetSaveObject<LevelSave>("level");
            levelSave.MaxReachedLevelIndex = 0;
            levelSave.DisplayLevelIndex = 0;
            levelSave.RealLevelIndex = 0;
            levelSave.IsPlayingRandomLevel = false;
            levelSave.LastPlayerLevelIndex = -1;

            var roleModule = GameGlobal.Instance.GetModule<RoleModule>();
            roleModule.PassLevel = 1;
            roleModule.ClearUnlockedHeroes();

            SaveController.Save(true);
            FloatingMessage.ShowMessage("进度已重置到第1关。");
        }

        public void WinCurrentLevelDev()
        {
            LevelController.instance.OnMatchCompleted(true);
        }

        public void FailCurrentLevelDev()
        {
            GameController.OnLevelFailed(GameOverReason.Failed);
        }

        public void PrevLevelDev()
        {
            LevelSave levelSave = SaveController.GetSaveObject<LevelSave>("level");
            levelSave.DisplayLevelIndex = Mathf.Clamp(levelSave.DisplayLevelIndex - 1, 0, GameLevelConfig.TotalLevelCount - 1);
            levelSave.RealLevelIndex = levelSave.DisplayLevelIndex;

            GameController.ReplayLevel();
        }

        public void NextLevelDev()
        {
            LevelSave levelSave = SaveController.GetSaveObject<LevelSave>("level");
            levelSave.DisplayLevelIndex = Mathf.Clamp(levelSave.DisplayLevelIndex + 1, 0, GameLevelConfig.TotalLevelCount - 1);
            levelSave.RealLevelIndex = levelSave.DisplayLevelIndex;

            GameController.ReplayLevel();
        }

        public void ClearAllServerDataDev()
        {
            StartCoroutine(ClearAllServerDataCoroutine());
        }

        private System.Collections.IEnumerator ClearAllServerDataCoroutine()
        {
            FloatingMessage.ShowMessage("正在清除服务器数据...");

            string url = Watermelon.Message.ServerHelper.GetClearAllDataUrl();

            bool requestDone = false;
            bool requestSuccess = false;

            HttpManager.Instance.PostJson(url, "{}",
                (response) =>
                {
                    requestSuccess = true;
                    requestDone = true;
                },
                (error) =>
                {
                    requestSuccess = false;
                    requestDone = true;
                });

            while (!requestDone)
            {
                yield return null;
            }

            if (requestSuccess)
            {
                PlayerPrefs.DeleteAll();
                SaveController.DeleteSaveFile();
                FloatingMessage.ShowMessage("服务器数据已清除，请重启游戏。");
            }
            else
            {
                FloatingMessage.ShowMessage("清除失败，请检查网络。");
            }
        }

        #endregion
    }
}
