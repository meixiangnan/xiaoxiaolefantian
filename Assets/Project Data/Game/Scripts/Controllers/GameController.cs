using System;
using System.Collections.Generic;
using Project_Data.SDK;
using UnityEngine;
using Watermelon.Map;
using Random = System.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Watermelon
{
    public class GameController : MonoBehaviour
    {
        private static GameController gameController;

        [DrawReference]
        [SerializeField] GameData data;

        [LineSpacer]
        [SerializeField] UIController uiController;

        private LevelController levelController;
        private ParticlesController particlesController;
        private FloatingTextController floatingTextController;
        private CurrenciesController currenciesController;
        private PUController powerUpController;
        private TutorialController tutorialController;

        public static GameData Data => gameController.data;

        private static bool isGameActive;
        public static bool isGamePause = false;
        public static bool IsGameActive => isGameActive;

        private void Awake()
        {
            gameController = this;

            SaveController.Initialise(useAutoSave: false);

            // Cache components
            CacheComponent(out particlesController);
            CacheComponent(out floatingTextController);
            CacheComponent(out currenciesController);
            CacheComponent(out levelController);
            CacheComponent(out powerUpController); 
            CacheComponent(out tutorialController);
            
        }

        private void Start()
        {
            InitialiseGame();
        }

        public void InitialiseGame()
        {
            uiController.Initialise();

            particlesController.Initialise();
            floatingTextController.Inititalise();
            currenciesController.Initialise();

            powerUpController.Initialise();
            levelController.Initialise();
            tutorialController.Initialise();

            uiController.InitialisePages();

            if (!SDKProxy.Inst.IsAuthSucc())
            {       
                UIController.ShowPage<UISDKLogin>();
                return;
            }
            //AdsManager.DisableBanner();
            ITutorial tutorial = TutorialController.GetTutorial(TutorialID.FirstLevel);
            if(data.ShowTutorial && !tutorial.IsFinished)
            {
                // Start first level tutorial
                tutorial.StartTutorial();
            }
            else
            {
                //mapBehavior.Show();
                // Display default page
                UIController.ShowPage<UIMainMenu>();
                

#if UNITY_EDITOR
                CheckIfNeedToAutoRunLevel();
#endif
            }

            GameLoading.MarkAsReadyToHide();
        }

        public static void LoadLevel(int index, SimpleCallback onLevelLoaded = null)
        {
            //AdsManager.ShowInterstitial(null);

            //gameController.mapBehavior.Hide();

            UIController.HidePage<UIMainMenu>(() =>
            {
                //AdsManager.EnableBanner();

                UIController.ShowPage<UIGame>();

                gameController.levelController.LoadLevel(index, onLevelLoaded);

                isGameActive = true;
                isGamePause = false;
                LevelController.LevelStartTick = Time.timeAsDouble;
                LevelController.ResetPauseTimer();
            });
        }

        public static void LoadCustomLevel(LevelData levelData, PreloadedLevelData preloadedLevelData, BackgroundData backgroundData, bool animateDock, SimpleCallback onLevelLoaded = null)
        {
            UIController.ShowPage<UIGame>();

            gameController.levelController.LoadCustomLevel(levelData, preloadedLevelData, backgroundData, animateDock, onLevelLoaded);

            isGameActive = true;
        }

        private static List<PUType> GetRandomRewards()
        {
            var VaildRewards = new List<PUType>(){ PUType.Undo , PUType.Shuffle ,PUType.Hint, PUType.ExtraSlot};

            var pos = new Random().Next(0, VaildRewards.Count);
            VaildRewards.RemoveAt(pos);
            
            VaildRewards.Shuffle();

            return VaildRewards;
        }

        public static void OnLevelCancel()
        {
            if (!isGameActive)
                return;
            
            isGameActive = false;
            isGamePause = false;
        }

        public static void OnLevelCompleted()
        {
            if (!isGameActive)
                return;

            var succReward = GetRandomRewards();

            var roleMdl = GameGlobal.Instance.GetModule<RoleModule>();
            for (int i = 0; i < succReward.Count; i++)
            {
                roleMdl.AddPowerUp(succReward[i], 1);    
            }
            roleMdl.OnPassLevel(LevelController.LastCompletedLevelNumber);
            int unlockedHeroId = TryUnlockHeroByCompletedLevel(LevelController.LastCompletedLevelNumber);
            
            UIController.HidePage<UIGame>(() =>
            {
                UIController.ShowPage<UIComplete>(new ShowUICompleteParam(){rewards = succReward, unlockedHeroId = unlockedHeroId});
            });

            isGameActive = false;
        }

        private static int TryUnlockHeroByCompletedLevel(int completedLevelNumber)
        {
            if (completedLevelNumber <= 0 || completedLevelNumber > GameLevelConfig.TotalLevelCount)
            {
                return -1;
            }

            if (completedLevelNumber % GameLevelConfig.HeroUnlockInterval != 0)
            {
                return -1;
            }

            int heroId = completedLevelNumber / GameLevelConfig.HeroUnlockInterval;
            var roleMdl = GameGlobal.Instance.GetModule<RoleModule>();
            if (roleMdl == null || roleMdl.IsHeroUnlocked(heroId, false))
            {
                return -1;
            }

            roleMdl.UnlockHero(heroId);
            return heroId;
        }

        public static void OnLevelFailed(GameOverReason rea)
        {
            if (!isGameActive)
                return;

            isGamePause = true;
            
            UIController.HidePage<UIGame>(() =>
            {
                UIController.ShowPage<UIGameOver>(new UIGameOverParam(){ reason = rea});
            });

            isGameActive = false;
        }

        public static void LoadNextLevel(SimpleCallback onLevelLoaded = null)
        {
            LoadLevel(LevelController.DisplayedLevelIndex, onLevelLoaded);
        }

        public static void Return3Tile()
        {
            RaycastController.Disable();

            LevelController.SetBusyState(true);

            LevelController.ReturnTiles(3, () =>
            {
                RaycastController.Enable();

                LevelController.SetBusyState(false);
            });
        }

        public static void ContinueAfterWatchAd()
        {
            isGameActive = true;
            isGamePause = false;
        }

        public static void ReplayLevel()
        {
            isGameActive = false;
            isGamePause = false;

            UIController.ShowPage<UIMainMenu>();

            LoadLevel(LevelController.DisplayedLevelIndex);
        }

        public static void ReturnToMenu()
        {
            isGameActive = false;
            isGamePause = false;

            LevelController.UnloadLevel();


            //AdsManager.ShowInterstitial(null);

            UIController.ShowPage<UIMainMenu>();

            //AdsManager.DisableBanner();
        }

        public static void Revive()
        {
            isGameActive = true;

            LevelController.ReturnTiles(3, null);
        }

        #region Extensions
        public bool CacheComponent<T>(out T component) where T : Component
        {
            Component unboxedComponent = gameObject.GetComponent(typeof(T));

            if (unboxedComponent != null)
            {
                component = (T)unboxedComponent;

                return true;
            }

            Debug.LogError(string.Format("Scripts Holder doesn't have {0} script added to it", typeof(T)));

            component = null;

            return false;
        }
        #endregion

        #region Dev

#if UNITY_EDITOR

        private static readonly string AUTO_RUN_LEVEL_SAVE_NAME = "auto run level editor";

        public static bool AutoRunLevelInEditor
        {
            get { return EditorPrefs.GetBool(AUTO_RUN_LEVEL_SAVE_NAME, false); }
            set { EditorPrefs.SetBool(AUTO_RUN_LEVEL_SAVE_NAME, value); }
        }

        private void CheckIfNeedToAutoRunLevel()
        {
            if (AutoRunLevelInEditor)
                LoadLevel(LevelController.DisplayedLevelIndex);

            AutoRunLevelInEditor = false;
        }
#endif


        #endregion
    }
}