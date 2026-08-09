using System.Collections.Generic;
using Newtonsoft.Json;

namespace Watermelon.Message
{
    /// <summary>
    /// 通用API响应类
    /// </summary>
    [System.Serializable]
    public class ApiResponse<T>
    {
        [JsonProperty("code")] public int code;
        [JsonProperty("data")] public T data;
        [JsonProperty("Age")]  public int age;
    }

    //定义请求基类
    public class MessageReq
    {
    }

    //定义回复基类
    public class MessageRsp
    {
    }
    
    
    //定义登录请求
    public class MsgQuickLoginReq : MessageReq
    {
        public int    UserId;
        public string Account;
        public string Token;
    }

    //登录回复
    public class MsgQuickLoginRsp : MessageRsp
    {
        [JsonProperty("UserId")] 
        public int UserId;
        [JsonProperty("Token")]
        public string Token;
        [JsonProperty("BirthYear")]  
        public string BirthYear;
        [JsonProperty("LeftSeconds")]  public int LeftSeconds;
        
        [JsonProperty("HeadIcon")] 
        public string HeadIcon;
        [JsonProperty("NickName")] 
        public string Nickname;
        [JsonProperty("PassLevel")] 
        public int PassLevel;
        [JsonProperty("IsFinishTutorial")] 
        public int IsFinishTutorial;
        [JsonProperty("ItemNum_Undo")]     
        public int ItemNum_Undo;
        [JsonProperty("ItemNum_Shuffle")]  
        public int ItemNum_Shuffle;
        [JsonProperty("ItemNum_Hint")]     
        public int ItemNum_Hint;
        [JsonProperty("ItemNum_ExtraSlot")] 
        public int ItemNum_ExtraSlot;
        [JsonProperty("ItemNum_AddTime")] 
        public int ItemNum_AddTime;
        [JsonProperty("DiamondCount")]
        public int? DiamondCount;
        [JsonProperty("MonthlyRechargeMonth")]
        public string MonthlyRechargeMonth;
        [JsonProperty("MonthlyRechargeAmount")]
        public int? MonthlyRechargeAmount;
        [JsonProperty("UnlockedHeroes")]
        public string UnlockedHeroes;
    }
    

    //定义登录请求
    public class MsgLoginReq : MessageReq
    {
        public string Account;
        public string Password;
    }

    //登录回复
    public class MsgLoginRsp : MessageRsp
    {
        [JsonProperty("HeadIcon")] public string HeadIcon;
        [JsonProperty("NickName")] public string Nickname;
        [JsonProperty("UserId")]   public int UserId;
        [JsonProperty("Token")]    public string Token;
        [JsonProperty("Account")]  public string Account;
        [JsonProperty("BirthYear")]  public string BirthYear;
        [JsonProperty("LeftSeconds")]  public int LeftSeconds;
        
        [JsonProperty("PassLevel")] 
        public int PassLevel;
        [JsonProperty("IsFinishTutorial")] 
        public int IsFinishTutorial;
        [JsonProperty("ItemNum_Undo")]     
        public int ItemNum_Undo;
        [JsonProperty("ItemNum_Shuffle")]  
        public int ItemNum_Shuffle;
        [JsonProperty("ItemNum_Hint")]     
        public int ItemNum_Hint;
        [JsonProperty("ItemNum_ExtraSlot")] 
        public int ItemNum_ExtraSlot;
        [JsonProperty("ItemNum_AddTime")] 
        public int ItemNum_AddTime;
        [JsonProperty("DiamondCount")]
        public int? DiamondCount;
        [JsonProperty("MonthlyRechargeMonth")]
        public string MonthlyRechargeMonth;
        [JsonProperty("MonthlyRechargeAmount")]
        public int? MonthlyRechargeAmount;
        [JsonProperty("UnlockedHeroes")]
        public string UnlockedHeroes;
    }

    public class MsgCreateReq : MessageReq
    {
        public string Account;
        public string Password;
        
        public string IdName;
        public string IdCard;
    }

    //登录回复
    public class MsgCreateRsp : MessageRsp
    {
        [JsonProperty("UserId")] 
        public int UserId;
        [JsonProperty("Token")]  
        public string Token;
        [JsonProperty("BirthYear")]  
        public string BirthYear;
        
        [JsonProperty("PassLevel")] 
        public int PassLevel;
        [JsonProperty("IsFinishTutorial")] 
        public int IsFinishTutorial;
        [JsonProperty("ItemNum_Undo")]     
        public int ItemNum_Undo;
        [JsonProperty("ItemNum_Shuffle")]  
        public int ItemNum_Shuffle;
        [JsonProperty("ItemNum_Hint")]     
        public int ItemNum_Hint;
        [JsonProperty("ItemNum_ExtraSlot")] 
        public int ItemNum_ExtraSlot;
        [JsonProperty("ItemNum_AddTime")] 
        public int ItemNum_AddTime;
        [JsonProperty("DiamondCount")]
        public int? DiamondCount;
        [JsonProperty("MonthlyRechargeMonth")]
        public string MonthlyRechargeMonth;
        [JsonProperty("MonthlyRechargeAmount")]
        public int? MonthlyRechargeAmount;
        [JsonProperty("UnlockedHeroes")]
        public string UnlockedHeroes;
    }

    
    public class MsgCreateRoleReq : MessageReq
    {
        public int UserId;
        public string Token;
        public string HeadIcon;
        public string NickName;
        
    }

    //登录回复
    public class MsgCreateRoleRsp : MessageRsp
    {
        
    }
    

    public class LeadBoardInfo
    {
        [JsonProperty("Name")] public string Name;
        [JsonProperty("HeadIcon")] public string HeadIcon;
        [JsonProperty("Score")] public string Score;
        [JsonProperty("Rank")] public int Rank;
    }

    public class MsgLoadLeaderBoardReq : MessageReq
    {
        public int UserId;
    }

    public class MsgLoadLeaderBoardRsp : MessageRsp
    {
        [JsonProperty("RankData")] public List<LeadBoardInfo> RankData = new();
        [JsonProperty("SelfRank")] public LeadBoardInfo SelfRank       = new();
    }

    public class MsgUploadLeaderBoardReq : MessageReq
    {
        public int UserId;
        public int PassLevel;
        public int IsFinishTutorial;
        public int ItemNum_Undo;
        public int ItemNum_Shuffle;
        public int ItemNum_Hint;
        public int ItemNum_ExtraSlot;
        public int ItemNum_AddTime;
        public int DiamondCount;
        public string MonthlyRechargeMonth;
        public int MonthlyRechargeAmount;
        public string UnlockedHeroes;
    }

    public class MsgUploadLeaderBoardRsp : MessageRsp
    {
    }

    public class AsyncRes
    {
        public int Code;
        public string Message;
    }
}