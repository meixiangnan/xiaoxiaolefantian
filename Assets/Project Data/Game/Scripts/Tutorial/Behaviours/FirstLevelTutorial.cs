using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Watermelon
{
    public class FirstLevelTutorial : BaseTutorial, ITutorial
    {
        private const int STEP_1_PICK_APPLES = 0;
        private const int STEP_2_PICK_CHEESE = 1;
        private const int STEP_3_DEPTH = 2;
        private const int STEP_4_PRE_HINT_DELAY = 3;
        private const int STEP_5_HINT = 4;
        private const int STEP_6_SHUFFLE = 5;
        private const int STEP_7_PICK_ELEMENT = 6;
        private const int STEP_8_UNDO = 7;
        private const int STEP_9_FINISH = 8;
        private const int STEP_10_FINISH = 9;

        private static FirstLevelTutorial tutorialController;

        [SerializeField] BackgroundData backgroundData;
        [SerializeField] Color tileDisableColor;

        [Header("Step I")]
        [SerializeField] LevelData firstLevelData;
        [SerializeField] PreloadedLevelData firstPreloadedLevelData;
        readonly string firstStepTitle = "操作方法";
        readonly string firstStepMessage = "点击要消除的卡牌放入消除区。\n积累三张相同的卡牌即可消除。";

        [Header("Step II")]
        readonly string secondStepTitle = "很棒";
        readonly string secondStepMessage = "继续消除更多的卡牌。";

        [Header("Step III")]
        [SerializeField] LevelData thirdLevelData;
        [SerializeField] PreloadedLevelData thirdPreloadedLevelData;
        readonly string thirdStepTitle = "解锁卡牌";
        readonly string thirdStepMessage = "解锁更深层的卡牌。";

        [Header("Step IV")]
        readonly string fourthStepTitle = "很棒";
        readonly string fourthStepMessage = "";

        [Header("Step V")]
        readonly string fifthStepTitle = "立即消除";
        readonly string fifthStepMessage = "立即在场上找到相同的卡牌。\n将消除区最左侧卡牌凑齐三张立即消除。";

        [Header("Step VI")]
        readonly string sixthStepTitle = "重新排列";
        readonly string sixthStepMessage = "将会对场上牌堆进行重新排列。";

        [Header("Step VII")]
        readonly string seventhStepTitle = "很棒";
        readonly string seventhStepMessage = "";

        [Header("Step IIX")]
        readonly string eighthStepTitle = "撤销操作";
        readonly string eighthStepMessage = "立即将上一步选中的卡牌放回场上牌堆中。";

        [Header("Step IX")]
        readonly string ninthStepTitle = "很棒";
        readonly string ninthStepMessage = "继续战斗吧。";
        
        [Header("Step X")]
        [SerializeField] LevelData KeyLevelData;
        [SerializeField] PreloadedLevelData KeyPreloadedLevelData;
        readonly string tenStepTitle = "快速通关秘诀";
        readonly string tenStepMessage = "找到绿色光芒笼罩的卡牌。";
        

        [Header("Finish")]
        readonly string finishTitle = "你是一名合格的勇者！";

        private bool isActive;
        public override bool IsActive => isActive;

        private int progress;
        public override int Progress => progress;

        public override bool IsFinished => saveData.isFinished;

        private TutorialBaseSave saveData;

        private UIGame gameUI;

        private List<TileBehavior> cheeseTiles;
        private List<TileBehavior> appleTiles;
        private List<TileBehavior> keyTiles;

        private TileBehavior pointerTile;
        private TileBehavior pyramidTile;
        private TileBehavior undoTile;
        private List<TileBehavior> undoClickableTiles;

        public override void Initialise()
        {
            tutorialController = this;

            saveData = SaveController.GetSaveObject<TutorialBaseSave>(string.Format(ITutorial.SAVE_IDENTIFIER, TutorialID.ToString()));

            gameUI = UIController.GetPage<UIGame>();
        }

        public override void StartTutorial()
        {
            if (isActive) return;

            isActive = true;
            progress = 0;

            
            cheeseTiles = null;
            appleTiles = null;

            pointerTile = null;
            pyramidTile = null;
            undoTile = null;
            undoClickableTiles = null;
            
            EnableStep(0);

            DockBehavior.MatchCombined += OnMatchCombined;
            DockBehavior.ElementAdded += OnElementAddedToDock;
            PUController.OnPowerUpUsed += OnPUUsed;

            //AdsManager.DisableBanner();
        }

        private void OnPUUsed(PUType powerUpType)
        {
            if(progress == STEP_5_HINT)
            {
                EnableStep(STEP_6_SHUFFLE);
            }
            else if (progress == STEP_6_SHUFFLE)
            {
                EnableStep(STEP_7_PICK_ELEMENT);
            }
            else if(progress == STEP_8_UNDO)
            {
                EnableStep(STEP_9_FINISH);
            }
        }

        private void EnableStep(int stepIndex)
        {
            if (stepIndex == STEP_1_PICK_APPLES)
            {
                gameUI.SetTutorialText(firstStepTitle, firstStepMessage);

                GameController.LoadCustomLevel(tutorialController.firstLevelData, tutorialController.firstPreloadedLevelData, tutorialController.backgroundData, true, () =>
                {
                    // Get cheese tiles
                    cheeseTiles = new List<TileBehavior>();
                    cheeseTiles.Add(LevelController.GetTile(new ElementPosition(0, 0, 1)));
                    cheeseTiles.Add(LevelController.GetTile(new ElementPosition(1, 0, 1)));
                    cheeseTiles.Add(LevelController.GetTile(new ElementPosition(2, 0, 1)));

                    foreach (var cheese in cheeseTiles)
                    {
                        cheese.SetBlockState(true);
                        cheese.SetColor(tileDisableColor, true);
                    }

                    // Get apple tiles
                    appleTiles = new List<TileBehavior>();
                    appleTiles.Add(LevelController.GetTile(new ElementPosition(0, 1, 1)));
                    appleTiles.Add(LevelController.GetTile(new ElementPosition(1, 1, 1)));
                    appleTiles.Add(LevelController.GetTile(new ElementPosition(2, 1, 1)));

                    foreach (var apple in appleTiles)
                    {
                        apple.SetBlockState(false);
                    }

                    ActivateTilePointer(appleTiles[0]);
                });
            }
            else if (stepIndex == STEP_2_PICK_CHEESE)
            {
                gameUI.SetTutorialText(secondStepTitle, secondStepMessage);

                foreach (var cheese in cheeseTiles)
                {
                    cheese.SetBlockState(false);
                    cheese.SetState(true, true);
                }

                ActivateTilePointer(cheeseTiles[0]);
            }
            else if (stepIndex == STEP_3_DEPTH)
            {
                gameUI.SetTutorialText(thirdStepTitle, thirdStepMessage);

               
                GameController.LoadCustomLevel(tutorialController.thirdLevelData, tutorialController.thirdPreloadedLevelData, tutorialController.backgroundData, false, () =>
                {
                    pyramidTile = LevelController.GetTile(new ElementPosition(1, 1, 0));

                    ActivateTilePointer(pyramidTile);

                    TileBehavior dockTile = LevelController.SpawnDockTile(0);

                    Vector3 tileSize = dockTile.transform.localScale;
                    dockTile.transform.localScale = Vector3.zero;
                    dockTile.transform.DOScale(tileSize, 0.5f).SetEasing(Ease.Type.BackOut);
                });
            }
            else if (stepIndex == STEP_4_PRE_HINT_DELAY)
            {
                gameUI.SetTutorialText(fourthStepTitle, fourthStepMessage);

                RaycastController.Disable();

                Tween.DelayedCall(0.5f, () =>
                {
                    EnableStep(STEP_5_HINT);
                });
            }
            else if (stepIndex == STEP_5_HINT)
            {
                gameUI.SetTutorialText(fifthStepTitle, fifthStepMessage);

                PUUIBehavior hintPanel = PUController.PowerUpsUIController.GetPanel(PUType.Hint);
                hintPanel.gameObject.SetActive(true);
                hintPanel.Settings.Save.Amount = 1;
                hintPanel.Redraw();

                Tween.NextFrame(() =>
                {
                    TutorialCanvasController.ActivatePointer(hintPanel.transform.position, TutorialCanvasController.POINTER_DEFAULT);
                });
            }
            else if (stepIndex == STEP_6_SHUFFLE)
            {
                
                PUController.PowerUpsUIController.HidePanel(PUType.Hint);

                TutorialCanvasController.HidePointer();
                Tween.DelayedCall(2.0f, () =>
                {
                    gameUI.SetTutorialText(sixthStepTitle, sixthStepMessage);
                    
                    PUUIBehavior shufflePanel = PUController.PowerUpsUIController.GetPanel(PUType.Shuffle);
                    shufflePanel.gameObject.SetActive(true);
                    shufflePanel.Settings.Save.Amount = 1;
                    shufflePanel.Redraw();
                    TutorialCanvasController.ResetPointer();

                    Tween.DelayedCall(0.3f, ()=>
                    {
                        TutorialCanvasController.ActivatePointer(shufflePanel.transform.position, TutorialCanvasController.POINTER_DEFAULT);
                    });

                });
            }
            else if (stepIndex == STEP_7_PICK_ELEMENT)
            {
                gameUI.SetTutorialText(seventhStepTitle, seventhStepMessage);

                TutorialCanvasController.ResetPointer();

                undoClickableTiles = new List<TileBehavior>(LevelController.LevelRepresentation.Tiles);
                for(int i = 0; i < undoClickableTiles.Count; i++)
                {
                    if (undoTile == null && undoClickableTiles[i].IsClickable)
                    {
                        undoTile = undoClickableTiles[i];

                        undoClickableTiles.RemoveAt(i);

                        break;
                    }
                }

                foreach (TileBehavior tile in undoClickableTiles)
                {
                    tile.SetBlockState(true);
                    tile.SetColor(tileDisableColor, true);
                }

                Tween.DelayedCall(0.3f, () =>
                {
                    ActivateTilePointer(undoTile);
                });

                PUController.PowerUpsUIController.HidePanel(PUType.Shuffle);
            }
            else if (stepIndex == STEP_8_UNDO)
            {
                gameUI.SetTutorialText(eighthStepTitle, eighthStepMessage);

                PUUIBehavior undoPanel = PUController.PowerUpsUIController.GetPanel(PUType.Undo);
                undoPanel.gameObject.SetActive(true);
                undoPanel.Settings.Save.Amount = 1;
                undoPanel.Redraw();

                Tween.NextFrame(() =>
                {
                    TutorialCanvasController.ActivatePointer(undoPanel.transform.position, TutorialCanvasController.POINTER_DEFAULT);
                });
            }
            else if (stepIndex == STEP_9_FINISH)
            {
                TutorialCanvasController.ResetPointer();

                foreach (TileBehavior tile in undoClickableTiles)
                {
                    tile.SetBlockState(false);
                    tile.SetState(LevelController.LevelRepresentation.IsTileUnconcealed(tile), true);
                }

                gameUI.SetTutorialText(ninthStepTitle, ninthStepMessage);

                PUController.PowerUpsUIController.HidePanel(PUType.Undo);
                
                

            }
            else if (stepIndex == STEP_10_FINISH)
            {
                Tween.DelayedCall(0.5f, () =>
                {
                    gameUI.SetTutorialText(tenStepTitle, tenStepMessage);
                
                    GameController.LoadCustomLevel(tutorialController.KeyLevelData, tutorialController.KeyPreloadedLevelData, tutorialController.backgroundData, true, () =>
                    {
                        keyTiles = new List<TileBehavior>();
                        keyTiles.Add(LevelController.GetTile(new ElementPosition(0, 0, 1)));
                        keyTiles.Add(LevelController.GetTile(new ElementPosition(1, 0, 1)));
                        keyTiles.Add(LevelController.GetTile(new ElementPosition(2, 0, 1)));
                    
                        var blocks = new List<TileBehavior>();
                    
                        blocks.Add(LevelController.GetTile(new ElementPosition(0, 1, 1)));
                        blocks.Add(LevelController.GetTile(new ElementPosition(1, 1, 1)));
                        blocks.Add(LevelController.GetTile(new ElementPosition(2, 1, 1)));

                        foreach (var key in blocks)
                        {
                            key.SetBlockState(true);
                            key.SetColor(tileDisableColor, true);
                        }
                    
                        ActivateTilePointer(keyTiles[0]);
                    });
                });
            }
            

            progress = stepIndex;
        }

        private void ActivateTilePointer(TileBehavior tileBehavior)
        {
            if(tileBehavior != null)
            {
                TutorialCanvasController.ActivatePointerWorld(tileBehavior.transform.position, TutorialCanvasController.POINTER_DEFAULT);

                pointerTile = tileBehavior;
            }
        }

        private void DisableTilePointer()
        {
            TutorialCanvasController.ResetPointer();

            pointerTile = null;
        }

        private void OnElementAddedToDock(ISlotable tile)
        {
            TileBehavior pickedTile = (TileBehavior)tile;
            if(pickedTile != null)
            {
                if (pickedTile == pointerTile)
                    DisableTilePointer();

                if(progress == STEP_1_PICK_APPLES)
                {
                    appleTiles.Remove(pickedTile);

                    if(appleTiles.Count > 0)
                    {
                        ActivateTilePointer(appleTiles[0]);
                    }
                    else
                    {
                        EnableStep(STEP_2_PICK_CHEESE);
                    }
                }
                else if(progress == STEP_2_PICK_CHEESE)
                {
                    cheeseTiles.Remove(pickedTile);

                    if (cheeseTiles.Count > 0)
                    {
                        ActivateTilePointer(cheeseTiles[0]);
                    }
                }
                else if(progress == STEP_3_DEPTH)
                {
                    if(pickedTile == pyramidTile)
                    {
                        EnableStep(STEP_4_PRE_HINT_DELAY);
                    }
                }
                else if (progress == STEP_7_PICK_ELEMENT)
                {
                    if (pickedTile == undoTile)
                    {
                        undoTile = null;
                        EnableStep(STEP_8_UNDO);
                    }
                }
            }
        }

        private void OnMatchCombined(List<ISlotable> tiles)
        {
            if (progress == STEP_2_PICK_CHEESE)
            {
                if (cheeseTiles.IsNullOrEmpty())
                {
                    EnableStep(STEP_3_DEPTH);
                }
            }
            else if(progress == STEP_9_FINISH)
            {
                if (LevelController.LevelRepresentation.Tiles.IsNullOrEmpty())
                {
                    EnableStep(STEP_10_FINISH);
                }
            }
            else if(progress == STEP_10_FINISH)
            {
                var keyFound = true;
                foreach(var slot in tiles)
                {
                    if (slot is TileBehavior tile && tile.Effect != null && tile.Effect.GetType() == typeof(KeyTileEffect))
                    {
                        keyFound = true;
                    }
                }
                if (keyFound)
                {
                    gameUI.SetTutorialText(finishTitle, "");

                    Tween.DelayedCall(2.0f, () =>
                    {
                        CompleteTutorial();
                    });
                }
            }
        }

        public void CompleteTutorial()
        {
            FinishTutorial();

            gameUI.DisableTutorial();

            DockBehavior.MatchCombined -= OnMatchCombined;
            DockBehavior.ElementAdded -= OnElementAddedToDock;
            PUController.OnPowerUpUsed -= OnPUUsed;


            LevelController.CompleteCustomLevel();

            GameController.LoadLevel(0, () =>
            {
                gameUI.PowerUpsUIController.ShowPanels();
            });
        }

        public override void FinishTutorial()
        {
            TutorialCanvasController.ResetPointer();

            PUBehavior[] powerUps = PUController.ActivePowerUps;
            foreach(var powerUp in powerUps)
            {
                powerUp.Settings.Save.Amount = powerUp.Settings.DefaultAmount;
            }

            var roleMdl =  GameGlobal.Instance.GetModule<RoleModule>();
            roleMdl.OnFinishTutorial();
            
            GameGlobal.Instance.UploadRoleData();
            
            saveData.isFinished = true;
            
            

            isActive = false;
        }

        public override void Unload()
        {
            TutorialCanvasController.HidePointer();
            DockBehavior.MatchCombined -= OnMatchCombined;
            DockBehavior.ElementAdded -= OnElementAddedToDock;
            PUController.OnPowerUpUsed -= OnPUUsed;
            isActive = false;
            Tween.RemoveAll();
        }

        public void OnSkipButtonClicked()
        {
            if(isActive && !saveData.isFinished)
            {
                CompleteTutorial();
            }
        }
    }
}
