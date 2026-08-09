using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Project_Data.SDK
{
    public enum SDKType
    {
        Dummy,
        Taptap,
    }
    
    public enum ErrorAuth
    {
        Succ = 0,
        UserCancel,
        NoLocalAuth,
        
        NeedLogoutFirst,
        
        SdkNotSupport,
        Failed,
    }
    
    public enum LeaderBoardType
    {
        Total,
        Friend,
    }
    

    abstract public class AuthResult
    {
        public ErrorAuth ErrorCode = 0;
        public string    Msg = "";

        abstract public SDKType GetSDKType();
    }

    
    public interface IAuthSDK
    {
        SDKType GetSDKType();
        void InitSDK();

        Task<AuthResult> CheckLocalAuth();
        Task<AuthResult> Auth();

        AuthResult GetAuthResult();

        void LogOut();

        void OpenSDKLeaderBoard(LeaderBoardType leaderBoardType);

        void CommitLeaderBoardScore(int score);
    }

    enum LocalAuthCheckStatue
    {
        Init,
        Checking,
        Done,
    }

    

    public class SDKProxy
    {
        private Dictionary<SDKType, IAuthSDK> authSDKs = new();
        
        private IAuthSDK DummySdk = null;
        private IAuthSDK UsingSdk = null;
 
        private LocalAuthCheckStatue LACS = LocalAuthCheckStatue.Init;

        public static SDKProxy Inst = null;

        public bool IsAuthSucc()
        {
            if (null != UsingSdk && UsingSdk.GetAuthResult().ErrorCode == ErrorAuth.Succ)
            {
                return false;
            }

            return false;
        }

        public async Task LocalInit()
        {
            if (LACS != LocalAuthCheckStatue.Init)
            {
                return;
            }

            LACS  = LocalAuthCheckStatue.Checking;
            Task<AuthResult>[] tasks = new Task<AuthResult>[authSDKs.Count];
            
            int cnt = 0;
            foreach (var kv in authSDKs)
            {
                tasks[cnt++] = kv.Value.CheckLocalAuth();
            }
            var results = await Task.WhenAll(tasks);

            foreach (var authRet in results)
            {
                if (authRet.ErrorCode == ErrorAuth.Succ)
                {
                    UsingSdk = GetAuthSDK(authRet.GetSDKType());
                }
            }

            LACS  = LocalAuthCheckStatue.Done;
        }
        
        public async Task<ErrorAuth> Auth(SDKType sdkType)
        {
            if (null != UsingSdk)
            {
                return ErrorAuth.NeedLogoutFirst;
            }

            var sdk = GetAuthSDK(sdkType);
            if (null == sdk)
            {
                return ErrorAuth.SdkNotSupport;
            }

            
            var sdkRet = await sdk.Auth();

            if (null == sdkRet)
            {
                return ErrorAuth.Failed;
            }

            if (ErrorAuth.Succ == sdkRet.ErrorCode)
            {
                UsingSdk = sdk;
            }

            return sdkRet.ErrorCode;
        }

        
        
        public void InitSDK()
        {
            Inst = this;
            DummySdk = new SDK_Dummy();
            
            authSDKs.Add(DummySdk.GetSDKType(), DummySdk);

            foreach (var kv in authSDKs)
            {
                kv.Value.InitSDK();
            }
        }


        public IAuthSDK GetAuthSDK(SDKType sdkType) 
        {
            IAuthSDK findValue = null;
            var ret = authSDKs.TryGetValue(sdkType, out findValue);
            if (!ret)
            {
                return null;
            }
            return findValue;
        }

        public IAuthSDK UsingSDK()
        {
            return UsingSdk;
        }

        public void LogOut()
        {
            if (null != UsingSdk)
            {
                UsingSdk = null;
            }
        }
    }
}