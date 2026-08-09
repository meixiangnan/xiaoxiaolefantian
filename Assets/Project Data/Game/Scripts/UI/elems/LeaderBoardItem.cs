using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Watermelon;
using Watermelon.Message;

public class LeaderBoardItem : MonoBehaviour
{
    public Sprite Top1Bg;
    public Sprite Top2Bg;
    public Sprite Top3Bg;
    public Sprite otherBg;
    
    public Image BgIcon;
    public Image headIcon;
    public TextMeshProUGUI nickName;
    public TextMeshProUGUI Score;
    public TextMeshProUGUI Rank;
    

    public void SetData(LeadBoardInfo data, bool resetBg = true)
    {
        nickName.text = data.Name;
        int score = 0;
        int.TryParse(data.Score, out score);
        score = Mathf.Clamp(score, 0, 300);
        Score.text = score + "关";
        Rank.text = data.Rank.ToString();

        if (resetBg)
        {
            if (data.Rank == 1)
            {
                BgIcon.sprite = Top1Bg;
            }else if (data.Rank == 2)
            {
                BgIcon.sprite = Top2Bg;
            }
            else if (data.Rank == 3)
            {
                BgIcon.sprite = Top1Bg;
            }
            else
            {
                BgIcon.sprite = otherBg;
            }
        }

        if (!string.IsNullOrEmpty(data.HeadIcon))
        {
            headIcon.sprite = HeadIconController.GetHeadIcon(data.HeadIcon);
        }
    }
}
