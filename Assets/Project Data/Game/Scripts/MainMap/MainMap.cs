using System;
using System.Collections;
using System.Collections.Generic;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;
using Watermelon;

namespace Watermelon.MainMap
{
    


public class MainMap : MonoBehaviour
{
    private int StartLevel = 1;
    private int EndLevel = GameLevelConfig.LevelsPerChapter;
    

    private readonly int MaxChapterLevelNum = GameLevelConfig.LevelsPerChapter; 
    
    private int GroupCnt = 0;
    
    public Image chapterBg;

    [Header("Chapter Backgrounds")]
    public List<Sprite> chapterBackgrounds = new List<Sprite>();
    
    public ScrollRect scrollRect;
    public LoopListView2 scrollView;
    
    public List<ChapterSwitch> chapterSwitches;
    
    public void Initialized()
    {
        for (int i = 0; i < chapterSwitches.Count; i++)
        {
            chapterSwitches[i].OnSelect += (index, bg) =>
            {
                this.SwitchChapter(index, bg);
            };
        }
        
        scrollView.InitListView(GroupCnt, GetItemCount);
        
        SelectChapterByProgress();
    }

    public void Refresh()
    {
        SelectChapterByProgress();
        scrollView.RefreshAllShownItem();
    }

    private void SelectChapterByProgress()
    {
        int progressLevel = GameGlobal.Instance.GetModule<RoleModule>().PassLevelShow;
        progressLevel = Mathf.Clamp(progressLevel, 1, GameLevelConfig.TotalLevelCount);

        int chapterIndex = ((progressLevel - 1) / GameLevelConfig.LevelsPerChapter) + 1;
        chapterIndex = Mathf.Clamp(chapterIndex, 1, GameLevelConfig.ChapterCount);

        int levelInChapter = (progressLevel - 1) % GameLevelConfig.LevelsPerChapter;
        int targetGroupIndex = levelInChapter / ChapterLevelGroup.LevelMaxNum;

        ChapterSwitch chapterSwitch = chapterSwitches.Find(item => item != null && item.ChapterIndex == chapterIndex);
        if (chapterSwitch != null)
        {
            chapterSwitch.SetSelect();
            scrollView.RefreshAllShownItemWithFirstIndex(targetGroupIndex);
            return;
        }

        if (chapterSwitches.Count > 0 && chapterSwitches[0] != null)
        {
            chapterSwitches[0].SetSelect();
        }
    }


    public void SwitchChapter(int chapterIndex, Sprite bg)
    {
        if (chapterIndex < 1 || chapterIndex > GameLevelConfig.ChapterCount)
        {
            return;
        }

        StartLevel = 1 + (chapterIndex - 1) * MaxChapterLevelNum;
        EndLevel = StartLevel + MaxChapterLevelNum - 1;
        GroupCnt = ((EndLevel - StartLevel) / ChapterLevelGroup.LevelMaxNum) + 1;
        
        scrollRect.StopMovement();
        scrollView.SetListItemCount(-1, true);
        scrollView.SetListItemCount(GroupCnt,true);
        scrollView.ResetListView();
        scrollView.RefreshAllShownItemWithFirstIndex(0);

        UpdateChapterVisuals();
        UpdateChapterBackground(chapterIndex, bg);
    }

    private void UpdateChapterVisuals()
    {
        int progressLevel = GameGlobal.Instance.GetModule<RoleModule>().PassLevelShow;
        progressLevel = Mathf.Clamp(progressLevel, 1, GameLevelConfig.TotalLevelCount);
        int unlockedChapterIndex = ((progressLevel - 1) / GameLevelConfig.LevelsPerChapter) + 1;
        unlockedChapterIndex = Mathf.Clamp(unlockedChapterIndex, 1, GameLevelConfig.ChapterCount);

        for (int i = 0; i < chapterSwitches.Count; i++)
        {
            if (chapterSwitches[i] == null)
            {
                continue;
            }

            chapterSwitches[i].SetUnlockVisual(chapterSwitches[i].ChapterIndex <= unlockedChapterIndex);
        }
    }

    private void UpdateChapterBackground(int chapterIndex, Sprite fallbackBg)
    {
        int bgIndex = chapterIndex - 1;
        if (chapterBackgrounds != null && bgIndex >= 0 && bgIndex < chapterBackgrounds.Count && chapterBackgrounds[bgIndex] != null)
        {
            chapterBg.sprite = chapterBackgrounds[bgIndex];
            return;
        }

        chapterBg.sprite = fallbackBg;
    }

    private LoopListViewItem2 GetItemCount(LoopListView2 loopListView, int index)
    {
        if (index < 0 || index >= GroupCnt) return null;

        int startLevel = StartLevel + index * ChapterLevelGroup.LevelMaxNum;
        int endLevel = startLevel + ChapterLevelGroup.LevelMaxNum - 1;
        if (endLevel > EndLevel)
        {
            endLevel = EndLevel;
        }

        LoopListViewItem2 item = loopListView.NewListViewItem("ChapterLevelGroup");
        var _itemInfo = item.GetComponent<ChapterLevelGroup>();
        _itemInfo.SetData(startLevel, endLevel);
        return item;
    }

    public void Show()
    {
    }

    public void Hide()
    {
        
    }
}


}
