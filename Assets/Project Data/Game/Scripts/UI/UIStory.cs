using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class UIStory : UIPage
    {
        public List<Sprite> StoryBgs = new ();
        public List<GameObject> StoryBgObjs = new ();
        public List<GameObject> StoryTexts = new ();

        public Image StoryImg;
        public Button nextBtn;
        public int curStory = 0;

        public override void Initialise()
        {
            nextBtn.onClick.AddListener(ToNext);
        }

        private void ToNext()
        {
            ++curStory;
            if (curStory >= StoryTexts.Count)
            {
                UIController.HidePage<UIStory>();
                
                ITutorial tutorial = TutorialController.GetTutorial(TutorialID.FirstLevel);
                tutorial.StartTutorial();
                
                return;
            }

            curStory = curStory % StoryBgObjs.Count;
            Refresh();
        }

        private void Refresh()
        {
            Debug.Log("Refresh"+curStory);
            //StoryImg.sprite = StoryBgs[curStory];
            for (int i = 0; i < StoryBgObjs.Count; i++)
            {
                if (i == curStory)
                {
                    StoryBgObjs[i].gameObject.SetActive(true);
                }
                else
                {
                    StoryBgObjs[i].gameObject.SetActive(false);
                }
            }
            for (int i = 0; i < StoryTexts.Count; i++)
            {
                if (i == curStory)
                {
                    StoryTexts[i].gameObject.SetActive(true);
                }
                else
                {
                    StoryTexts[i].gameObject.SetActive(false);
                }
            }
        }

        #region Show/Hide

        public override void PlayShowAnimation(object param = null)
        {
            this.curStory = 0;
            Refresh();
            
            UIController.OnPageOpened(this);
        }

        public override void PlayHideAnimation()
        {
            if (!isPageDisplayed)
                return;

            UIController.OnPageClosed(this);
        }

        #endregion
        
    }
}