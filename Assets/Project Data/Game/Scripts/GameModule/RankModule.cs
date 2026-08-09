using System.Collections;
using System.Collections.Generic;
using Watermelon;
using Watermelon.GameModule;
using Watermelon.Message;

public class RankModule : GameModuleBase
{
    public LeadBoardInfo myRandData     = new();
    public List<LeadBoardInfo> rankData = new();

    public IEnumerator UploadRoleData()
    {
        AsyncRes res = new AsyncRes();
        MsgUploadLeaderBoardReq req = new MsgUploadLeaderBoardReq();
        var roleMdl = GetModule<RoleModule>(); 
        req.UserId = roleMdl.userData.UserId;
        req.PassLevel = roleMdl.PassLevel;
        req.IsFinishTutorial = roleMdl.IsFinishTutorial;
        req.ItemNum_Undo      = roleMdl.ItemNum_Undo;
        req.ItemNum_Shuffle   = roleMdl.ItemNum_Shuffle;
        req.ItemNum_Hint      = roleMdl.ItemNum_Hint;
        req.ItemNum_ExtraSlot = roleMdl.ItemNum_ExtraSlot;
        req.ItemNum_AddTime   = roleMdl.ItemNum_AddTime;

        var diamondMdl = GetModule<DiamondModule>();
        req.DiamondCount = diamondMdl.DiamondCount;
        req.MonthlyRechargeMonth = diamondMdl.GetMonthlyRechargeMonth();
        req.MonthlyRechargeAmount = diamondMdl.MonthlyRechargeAmount;
        req.UnlockedHeroes = roleMdl.UnlockedHeroes;
        
        RequestContext rc = new RequestContext();
        rc.Req = req;
        rc.method = ServerMethod.UploadPassLevel;
        
        yield return ServerHelper.RequestServer<MsgUploadLeaderBoardRsp>(rc);

        if (rc.ErrCode != (int)GameErrorCode.Succ)
        {
            yield break;
        }
        
        MsgUploadLeaderBoardRsp rsp = rc.Resp as MsgUploadLeaderBoardRsp;
        if (rsp == null)
        {
            yield break;
        }
        res.Code = (int)GameErrorCode.Succ;
        res.Message = "Upload Passed Level Success";
    }

    public IEnumerator LoadRandData(AsyncRes res)
    {
        rankData.Clear();
        
        MsgLoadLeaderBoardReq req = new MsgLoadLeaderBoardReq();
        req.UserId = GetModule<RoleModule>().userData.UserId;
        
        RequestContext rc = new RequestContext();
        rc.Req = req;
        rc.method = ServerMethod.LoadLeaderBoard;
        
        yield return ServerHelper.RequestServer<MsgLoadLeaderBoardRsp>(rc);

        if (rc.ErrCode != (int)GameErrorCode.Succ)
        {
            res.Code = (int)GameErrorCode.Fail;
            res.Message = "Load Rank Data Fail";
            yield break;
        }
        
        MsgLoadLeaderBoardRsp rsp = rc.Resp as MsgLoadLeaderBoardRsp;
        if (rsp == null)
        {
            res.Code = (int)GameErrorCode.Fail;
            res.Message = "Load Rank Data Fail";
            yield break;
        }

        res.Code = (int)GameErrorCode.Succ;
        rankData = rsp.RankData;
        myRandData = rsp.SelfRank;
    }

}