using System;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;
using Watermelon.GameModule;
using Watermelon.Message;

public class UserData : ISaveObject
{
    public string HeadIcon = "";
    public string Nickname = "";
    public int    UserId   = -1;
    public string Account  = "";
    public string token    = "";
    
    public void Flush()
    {
        
    }
}

public class RoleModule : GameModuleBase
{
    public UserData userData = new();
    
    public int IsFinishTutorial = 0;
    public int PassLevel;
    public int Age;
    public int CanPlaySeconds = 0;
    private int serverLeftSeconds = 0;
    public int ItemNum_Undo = 0;
    public int ItemNum_Shuffle = 0;
    public int ItemNum_Hint = 0;
    public int ItemNum_ExtraSlot = 0;
    public int ItemNum_AddTime = 0;
    public string UnlockedHeroes = "";

    private readonly HashSet<int> unlockedHeroIds = new();


    
    private bool isLogin = false;
    private bool isNotifyLeft = false;
    private bool isNotifyTimeOver = false;
    private DateTime LoginTick;
    private bool isNeedShowAdultNotify = false;
    private bool isShowAdultNotify = false;

    private const double AntiAddictionLeftNotifySeconds = 15 * 60;


    public int PassLevelShow
    {
        get
        {
            //return 501;
            return PassLevel;
        }
    }

    public void RecordLogin()
    {
        isLogin = true;
        CanPlaySeconds = serverLeftSeconds;
        LoginTick = DateTime.Now;
    }

    public void Logout()
    {
        isLogin = false;
    }

    public void OnPassLevel(int completedLevelNumber)
    {
        if (!IsTutorialOver())
        {
            return;
        }

        if (PassLevel >= GameLevelConfig.TotalLevelCount + 1)
        {
            return;
        }

        if (completedLevelNumber != PassLevel)
        {
            return;
        }

        ++PassLevel;
    }

    public void AddPowerUp(PUType pu, int num)
    {
        switch (pu)
        {
            case PUType.Undo      : ItemNum_Undo += num; break;
            case PUType.Hint      : ItemNum_Hint += num; break;
            case PUType.Shuffle   : ItemNum_Shuffle += num; break;
            case PUType.ExtraSlot : ItemNum_ExtraSlot += num; break;
            case PUType.AddTime   : ItemNum_AddTime += num; break;
        }
        CheckAllItemNum();
        SavePowerUpsLocal();
    }

    public void SavePowerUpsLocal()
    {
        GameGlobal.Instance?.UploadRoleData();
    }

    public void LoadPowerUpsLocal()
    {
    }

    public void SetServerExtraData(int diamondCount, string monthlyRechargeMonth, int monthlyRechargeAmount, string unlockedHeroes)
    {
        GameGlobal.Instance.GetModule<DiamondModule>()?.SetServerData(diamondCount, monthlyRechargeMonth, monthlyRechargeAmount);
        SetUnlockedHeroes(unlockedHeroes);
    }

    public bool IsHeroUnlocked(int heroId, bool defaultUnlocked)
    {
        return defaultUnlocked || unlockedHeroIds.Contains(heroId);
    }

    public void UnlockHero(int heroId)
    {
        if (heroId <= 0 || !unlockedHeroIds.Add(heroId))
        {
            return;
        }

        RefreshUnlockedHeroesString();
        GameGlobal.Instance?.UploadRoleData();
    }

    public void SetUnlockedHeroes(string unlockedHeroes)
    {
        unlockedHeroIds.Clear();
        UnlockedHeroes = unlockedHeroes ?? "";
        if (!string.IsNullOrEmpty(UnlockedHeroes))
        {
            string[] ids = UnlockedHeroes.Split(',');
            foreach (string id in ids)
            {
                if (int.TryParse(id, out int heroId) && heroId > 0)
                {
                    unlockedHeroIds.Add(heroId);
                }
            }
        }

        RefreshUnlockedHeroesString();
    }

    private void RefreshUnlockedHeroesString()
    {
        List<int> ids = new List<int>(unlockedHeroIds);
        ids.Sort();
        UnlockedHeroes = string.Join(",", ids);
    }

    public void UnlockHeroesByCompletedLevel(int completedLevel)
    {
        int unlockedHeroCount = completedLevel / GameLevelConfig.HeroUnlockInterval;
        for (int heroId = 1; heroId <= unlockedHeroCount; heroId++)
        {
            unlockedHeroIds.Add(heroId);
        }

        RefreshUnlockedHeroesString();
        GameGlobal.Instance?.UploadRoleData();
    }

    public void ClearUnlockedHeroes()
    {
        unlockedHeroIds.Clear();
        RefreshUnlockedHeroesString();
        GameGlobal.Instance?.UploadRoleData();
    }

    public void OnFinishTutorial()
    {
        IsFinishTutorial = 1;
        ItemNum_Undo     = 3;
        ItemNum_Shuffle  = 3;
        ItemNum_Hint     = 3;
        ItemNum_ExtraSlot = 3;
        ItemNum_AddTime  = 3;
        SavePowerUpsLocal();
    }

    public bool IsTutorialOver()
    {
        //return false;
        return IsFinishTutorial > 0;
    }

    public void OnRegSucc(MsgCreateRsp resp)
    {
        userData.UserId = resp.UserId;
        userData.token = resp.Token;
        
        PassLevel         = resp.PassLevel;
        IsFinishTutorial  = resp.IsFinishTutorial;
        ItemNum_Undo      = resp.ItemNum_Undo;
        ItemNum_Shuffle   = resp.ItemNum_Shuffle;
        ItemNum_Hint      = resp.ItemNum_Hint;
        ItemNum_ExtraSlot = resp.ItemNum_ExtraSlot;
        ItemNum_AddTime   = resp.ItemNum_AddTime;
        LoadPowerUpsLocal();
        SetServerExtraData(resp.DiamondCount ?? 0, resp.MonthlyRechargeMonth, resp.MonthlyRechargeAmount ?? 0, resp.UnlockedHeroes);
        
        InitAge(resp.BirthYear);
    }

    private void InitAge(string birthYear)
    {
        if (string.IsNullOrEmpty(birthYear))
        {
            Age = 18;
            return;
        }

        Age =  DateTime.Now.Year - int.Parse(birthYear);

        if (!IsAdult())
        {
            this.isNeedShowAdultNotify = true;
            this.isShowAdultNotify = false;
            isNotifyLeft = false;
            isNotifyTimeOver = false;
        }
    }

    public void ShowAdultNotify()
    {
        NotifyDialog.NotifyClose(DialogState.Notice, "防沉迷提示", "    您的账号已被纳入防沉迷系统。根据国家新闻出版署《关于防止未成年人沉迷网络游戏的通知》与《关于进一步严格管理 切实防止未成年人沉迷网络游戏的通知》，您可在周五、周六、周日和法定节假日的20:00-21:00登入游戏，其他时间将无法为未成年人用户提供游戏服务。");
        isShowAdultNotify = true;
    }

    public void OnCreateRoleSucc(MsgCreateRoleReq resp)
    {
        userData.Nickname = resp.NickName;
        userData.HeadIcon =  resp.HeadIcon;
    }
    
    public void OnLoginSucc(MsgLoginRsp resp)
    {
        userData.HeadIcon  = resp.HeadIcon;
        userData.Nickname = resp.Nickname;
        userData.UserId = resp.UserId;
        userData.token = resp.Token;
        userData.Account = resp.Account;
        
        PassLevel         = resp.PassLevel;
        IsFinishTutorial  = resp.IsFinishTutorial;
        ItemNum_Undo      = resp.ItemNum_Undo;
        ItemNum_Shuffle   = resp.ItemNum_Shuffle;
        ItemNum_Hint      = resp.ItemNum_Hint;
        ItemNum_ExtraSlot = resp.ItemNum_ExtraSlot;
        ItemNum_AddTime   = resp.ItemNum_AddTime;
        LoadPowerUpsLocal();
        SetServerExtraData(resp.DiamondCount ?? 0, resp.MonthlyRechargeMonth, resp.MonthlyRechargeAmount ?? 0, resp.UnlockedHeroes);
        serverLeftSeconds    = resp.LeftSeconds;

        InitAge(resp.BirthYear);

        SaveLocalRoleInfo();
    }

    public bool IsAdult()
    {
        return Age >= 18;
    }

    public bool IsNeedShowUnAdult()
    {
        if (IsAdult())
        {
            return false;
        }

        if (!isNeedShowAdultNotify)
        {
            return false;
        }

        if (isShowAdultNotify)
        {
            return false;
        }

        return true;
    }

    public void OnQuickLoginSucc(MsgQuickLoginRsp resp)
    {
        if (userData == null)
        {
            userData = new UserData();
        }
        userData.HeadIcon  = resp.HeadIcon;
        userData.Nickname = resp.Nickname;
        userData.UserId = resp.UserId;
        userData.token = resp.Token;
        
        PassLevel         = resp.PassLevel;
        IsFinishTutorial  = resp.IsFinishTutorial;
        ItemNum_Undo      = resp.ItemNum_Undo;
        ItemNum_Shuffle   = resp.ItemNum_Shuffle;
        ItemNum_Hint      = resp.ItemNum_Hint;
        ItemNum_ExtraSlot = resp.ItemNum_ExtraSlot;
        ItemNum_AddTime   = resp.ItemNum_AddTime;
        LoadPowerUpsLocal();
        SetServerExtraData(resp.DiamondCount ?? 0, resp.MonthlyRechargeMonth, resp.MonthlyRechargeAmount ?? 0, resp.UnlockedHeroes);
        serverLeftSeconds    = resp.LeftSeconds;
        
        InitAge(resp.BirthYear);
    }
    
    public void ReadLocalRoleInfo()
    {
        var ret  = SaveController.FindSaveObject<UserData>("LocalUserData");
        if (ret != null)
        {
            this.userData = ret;
        }
    }

    public bool IsHavePuItem(PUType pu)
    {
        if (!IsTutorialOver())
        {
            return true;
        }

        return GetPUAmount(pu) > 0;
    }

    public int GetPUAmount(PUType pu)
    {
        if (!IsTutorialOver())
        {
            return 1;
        }
        
        var num = this.GetRealPuAmount(pu);
        if (num < 0)
        {
            return 0;
        }

        if (num > 999)
        {
            num = 999;
        }
        return num;
    }

    private int GetRealPuAmount(PUType pu)
    {
        switch (pu)
        {
            case PUType.Undo      : return ItemNum_Undo;
            case PUType.Hint      : return ItemNum_Hint;
            case PUType.Shuffle   : return ItemNum_Shuffle;
            case PUType.ExtraSlot : return ItemNum_ExtraSlot;
            case PUType.AddTime   : return ItemNum_AddTime;
        }

        return 0;
    }


    public void UsePU(PUType pu)
    {
        switch (pu)
        {
            case PUType.Undo      : ItemNum_Undo--; break;
            case PUType.Hint      : ItemNum_Hint--; break;
            case PUType.Shuffle   : ItemNum_Shuffle--; break;
            case PUType.ExtraSlot : ItemNum_ExtraSlot--; break;
            case PUType.AddTime   : ItemNum_AddTime--; break;
        }
        CheckAllItemNum();
        SavePowerUpsLocal();
    }

    private void CheckAllItemNum()
    {
        CheckItemValue(ref ItemNum_Undo);
        CheckItemValue(ref ItemNum_Hint);
        CheckItemValue(ref ItemNum_Shuffle);
        CheckItemValue(ref ItemNum_ExtraSlot);
        CheckItemValue(ref ItemNum_AddTime);
    }

    private void CheckItemValue(ref int ItemNum)
    {
        if (ItemNum < 0)
        {
            ItemNum = 0;
        }
        else if (ItemNum >= 999)
        {
            ItemNum = 999;
        }
    }

    public void SaveLocalRoleInfo()
    {
        var save = SaveController.GetSaveObject<UserData>("LocalUserData");
        save.UserId = this.userData.UserId;
        save.HeadIcon = this.userData.HeadIcon;
        save.Nickname = this.userData.Nickname;
        save.Account = this.userData.Account;
        save.token = this.userData.token;
        
        SaveController.Save(true);
    }


    public override void TickModule()
    {
        if (!isLogin)
        {
            return;
        }
        AdultLoginCheck();
    }

    private void AdultLoginCheck()
    {
        if (IsAdult())
        {
            return;
        }

        double loginLeft = GetAntiAddictionLeftSeconds();
        
        if (!isNotifyLeft && loginLeft > 0 && loginLeft <= AntiAddictionLeftNotifySeconds)
        {
            NotifyDialog.NotifyClose(DialogState.NoticeConfirmOnly, "防沉迷提示", "        您当前登录的是未成年人帐号，已被纳入防沉迷系统。根据国家新闻出版署《关于防止未成年人沉迷网络游戏的通知》与《关于进一步严格管理 切实防止未成年人沉迷网络游戏的通知》，您可在周五、周六、周日和法定节假日的20:00-21:00登入游戏。\n        您当日剩余游戏时长已不足15分钟，请注意您的游戏时长。");
            isNotifyLeft = true;
        }
        
        if (!isNotifyTimeOver && loginLeft <= 0)
        {
            GameController.isGamePause = true;
            NotifyDialog.NotifyClose(DialogState.QuitGame, "防沉迷提示", "        您已被强制下线。根据国家新闻出版署《关于防止未成年人沉迷网络游戏的通知》与《关于进一步严格管理 切实防止未成年人沉迷网络游戏的通知》，您可在周五、周六、周日和法定节假日的20:00-21:00登入游戏。");
            isNotifyTimeOver = true;
        }
    }

    private double GetAntiAddictionLeftSeconds()
    {
        if (CanPlaySeconds <= 0)
        {
            return 0;
        }

        TimeSpan passTime = DateTime.Now - LoginTick;
        return CanPlaySeconds - passTime.TotalSeconds;
    }

    public bool IsMinorPlayableTimeNow()
    {
        if (IsAdult())
        {
            return true;
        }

        return serverLeftSeconds > 0;
    }
}