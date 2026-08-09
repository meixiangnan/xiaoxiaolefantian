using System.Collections;
using System.Reflection;
using UnityEngine;

namespace Watermelon.Message
{
    public enum GameErrorCode
    {
        Fail = -1,
        Succ = 0,
        InvaildAccount,
        PasswordError,
        InvaildUserId,
        InvaildToken,
        DBError,
        IdCardLengthError,
        IdCardDigitError,
        IdCardAgeError,
        IdCardAreaError,
        IdCardCheckSumError,
        NickNameIsNull,
        HeadIconIsNull,
        AccountExists,
        UserOrPwdNull,
        AgeCannotLoginNow,
        AgeDayDuringMoreThanOneHour,
    }

    //定义一个服务器提供方法枚举
    public enum ServerMethod
    {
        QuickLogin,
        Login,
        Create,
        CreateRole,
        LoadLeaderBoard,
        UploadPassLevel,
        ClearAllData,
    }
    
    //定义请求上下文
    public class RequestContext
    {
        public ServerMethod method;
        public MessageReq Req;
        public MessageRsp Resp;
        
        public int ErrCode = 0;
        public int ErrAge  = -1;
    }
    
    public class ServerHelper
    {
        //public const string ServerURL = "https://bzhero.online";
        //public const string ServerURL = "https://knoll-underdone-constant.ngrok-free.dev";
        //public const string ServerURL = "http://192.168.0.110:33055";
        //public const string ServerURL = "https://82.157.147.7";
        public const string ServerURL = "https://bzhero.online:8443";
        //public const string ServerURL = "https://82.157.147.7";
        
        
        //根据枚举返回对应的URL
        private static string GetURL(ServerMethod method)
        {
            switch (method)
            {
                case ServerMethod.QuickLogin:
                    return ServerURL + "/user/quick_login";
                case ServerMethod.Login:
                    return ServerURL + "/user/login";
                case ServerMethod.Create:
                    return ServerURL + "/user/create";
                case ServerMethod.CreateRole:
                    return ServerURL + "/user/create_role";
                case ServerMethod.LoadLeaderBoard:
                    return ServerURL + "/game/load_leaderboard";
                case ServerMethod.UploadPassLevel:
                    return ServerURL + "/game/upload_pass_level";
                case ServerMethod.ClearAllData:
                    return ServerURL + "/admin/clear_all_data";
            }
            Debug.LogError("ServerHelper GetURL method not found");
            return "";
        }
        
        //协程方法，发送req消息，成功了则将返回值解析为resp消息
        public static IEnumerator RequestServer<TResp>(RequestContext ctx)  where TResp: MessageRsp
        {
            string url = GetURL(ctx.method);
            if (ctx.Req == null)
            {
                yield break;
            }
            string json = JsonUtility.ToJson(ctx.Req);
            yield return HttpManager.Instance.ReqTask<TResp>(url, ctx);
        }

        public static string GetClearAllDataUrl()
        {
            return ServerURL + "/admin/clear_all_data";
        }
    }
}
