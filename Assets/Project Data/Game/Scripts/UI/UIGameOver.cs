using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public enum GameOverReason
    {
        Timeout,
        FailedRetry,
        Failed,
    }

    public class UIGameOverParam
    {
        public GameOverReason reason;
    }

    public class UIGameOver : UIPage
    {
        [SerializeField] RectTransform safeAreaRectTransform;
        
        [SerializeField] UIScaleAnimation levelFailed;
        [SerializeField] UIFadeAnimation backgroundFade;

        [SerializeField] Button replayButton;
        [SerializeField] Button FailedWatchAdButton;
        [SerializeField] Button FailedGiveUpButton;
        [SerializeField] Button TimoutReplayButton;
        [SerializeField] Button TimoutWatchAdButton;

        public GameObject FailedRoot;
        public GameObject FailedRetryRoot;
        public GameObject TimoutRoot;
        public TextMeshProUGUI LevelName;

        [SerializeField] UIScaleAnimation replayButtonScalable;

        [SerializeField] LivesIndicator livesIndicator;
        [SerializeField] AddLivesPanel addLivesPanel;


        public override void Initialise()
        {
            replayButton.onClick.AddListener(ReplayButton);
            FailedWatchAdButton.onClick.AddListener(FailedWatchAd);
            TimoutReplayButton.onClick.AddListener(ReplayButton);
            TimoutWatchAdButton.onClick.AddListener(WatchAd);
            FailedGiveUpButton.onClick.AddListener(GiveUpButton);
        }

        
        
        #region Show/Hide

        private void SetState(GameOverReason reason)
        {
            FailedRoot.SetActive(true);
            FailedRetryRoot.SetActive(false);
            TimoutRoot.SetActive(false);
        }

        public override void PlayShowAnimation(object param = null)
        {
            GameController.isGamePause = true;
            LevelName.text = UILevelNumberText.GetLevelName(LevelController.DisplayedLevelIndex);
            
            if (param != null && param is UIGameOverParam showParam)
            {
                SetState(showParam.reason);
            }

            levelFailed.Hide(immediately: true);
            replayButtonScalable.Hide(immediately: true);

            float fadeDuration = 0.3f;
            backgroundFade.Show(fadeDuration);

            Tween.DelayedCall(fadeDuration * 0.8f, delegate
            {
                levelFailed.Show();

                replayButtonScalable.Show(scaleMultiplier: 1.05f, delay: 0.75f);


                UIController.OnPageOpened(this);
            });

        }

        public override void PlayHideAnimation()
        {
            GameController.isGamePause = false;
            backgroundFade.Hide(0.3f);

            Tween.DelayedCall(0.3f, delegate
            {
                UIController.OnPageClosed(this);
            });
        }

        #endregion

        #region Buttons 

        private void ReviveButton()
        {
            AudioController.PlaySound(AudioController.Sounds.buttonSound);

            AdsManager.ShowRewardBasedVideo(ReviveCallback);
        }

        private void ReviveCallback(bool watchedRV)
        {
            if (!watchedRV) return;

            UIController.HidePage<UIGameOver>();
            UIController.ShowPage<UIGame>();

            GameController.Revive();
        }

        private void WatchAd()
        {
            UIController.HidePage<UIGameOver>();
            AudioController.PlaySound(AudioController.Sounds.buttonSound);
            LevelController.instance.ContinueWithLeftSecond(60);
            GameController.ContinueAfterWatchAd();
            UIController.ShowPage<UIGame>();
            
        }

        private void FailedWatchAd()
        {
            UIController.HidePage<UIGameOver>();
            AudioController.PlaySound(AudioController.Sounds.buttonSound);
            GameController.ContinueAfterWatchAd();
            GameController.Return3Tile();
            UIController.ShowPage<UIGame>();
        }

        public void ReplayButton()
        {
            AudioController.PlaySound(AudioController.Sounds.buttonSound);

            UIController.HidePage<UIGameOver>();
            GameController.ReplayLevel();
        }

        public void GiveUpButton()
        {
            this.SetState(GameOverReason.Failed);
        }

        private void MenuButton()
        {
            AudioController.PlaySound(AudioController.Sounds.buttonSound);

            UIController.HidePage<UIGameOver>(() =>
            {
                GameController.ReturnToMenu();
            });
        }

        #endregion
    }
}