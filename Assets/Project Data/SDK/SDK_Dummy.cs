using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

namespace Project_Data.SDK
{
    public class AuthResultDummy : AuthResult
    {
        public string OpenId = "";
        public override SDKType GetSDKType()
        {
            return SDKType.Dummy;
        }
    }

    public class SDK_Dummy : IAuthSDK
    {
        AuthResultDummy LastRet = new AuthResultDummy(){ ErrorCode = ErrorAuth.Failed, }; 
        public static string sInputOpenId = "";
        public SDKType GetSDKType()
        {
            return SDKType.Dummy;
        }

        public void InitSDK()
        {
            
        }

        public Task<AuthResult> CheckLocalAuth()
        {
            
            #if UNITY_EDITOR
                LastRet.ErrorCode = ErrorAuth.Succ;
            #else
                LastRet.ErrorCode = ErrorAuth.Failed;
            #endif
            
            return Task.FromResult((AuthResult)LastRet);
        }

        public Task<AuthResult> Auth()
        {
            LastRet.OpenId = sInputOpenId;
            LastRet.ErrorCode = ErrorAuth.Succ;
            
            return Task.FromResult((AuthResult)LastRet);
        }

        public AuthResult GetAuthResult()
        {
            return LastRet;
        }

        public void LogOut()
        {
            
        }

        public void OpenSDKLeaderBoard(LeaderBoardType leaderBoardType)
        {
            Debug.Log("OpenSDKLeaderBoard :" + leaderBoardType);
            
        }
        

        public void CommitLeaderBoardScore(int score)
        {
            Debug.Log("CommitLeaderBoardScore :" + score);
        }
    }
}