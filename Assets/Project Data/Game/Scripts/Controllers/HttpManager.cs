using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Watermelon.Message;

public class AcceptAllCertificates : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}

public class HttpManager : MonoBehaviour
{
    private static HttpManager _instance;
    public static HttpManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("HttpManager");
                _instance = obj.AddComponent<HttpManager>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }

    public void Init()
    {
    }

    public void Get(string url, System.Action<string> onSuccess, System.Action<string> onError)
    {
        StartCoroutine(CoGet(url, onSuccess, onError));
    }

    public void Post(string url, Dictionary<string, string> formData, System.Action<string> onSuccess, System.Action<string> onError)
    {
        StartCoroutine(CoPost(url, formData, onSuccess, onError));
    }

    public void PostJson(string url, string jsonData, System.Action<string> onSuccess, System.Action<string> onError)
    {
        StartCoroutine(CoPostJson(url, jsonData, onSuccess, onError));
    }

    private IEnumerator CoGet(string url, System.Action<string> onSuccess, System.Action<string> onError)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            webRequest.certificateHandler = new AcceptAllCertificates();
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(webRequest.downloadHandler.text);
            }
            else
            {
                string errorMsg = $"GET request failed: {webRequest.error}\nURL: {url}";
                Debug.LogError(errorMsg);
                onError?.Invoke(errorMsg);
            }
        }
    }

    public IEnumerator CoPost(string url, Dictionary<string, string> formData, System.Action<string> onSuccess, System.Action<string> onError)
    {
        WWWForm form = new WWWForm();
        foreach (var item in formData)
        {
            form.AddField(item.Key, item.Value);
        }

        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, form))
        {
            webRequest.certificateHandler = new AcceptAllCertificates();
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(webRequest.downloadHandler.text);
            }
            else
            {
                string errorMsg = $"POST request failed: {webRequest.error}\nURL: {url}";
                Debug.LogError(errorMsg);
                onError?.Invoke(errorMsg);
            }
        }
    }

    public IEnumerator CoPostJson(string url, string jsonData, System.Action<string> onSuccess, System.Action<string> onError)
    {
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.certificateHandler = new AcceptAllCertificates();

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(webRequest.downloadHandler.text);
            }
            else
            {
                string errorMsg = $"POST JSON request failed: {webRequest.error}\nURL: {url}";
                Debug.LogError(errorMsg);
                onError?.Invoke(errorMsg);
            }
        }
    }

    public IEnumerator ReqTask<TResp>(string url, RequestContext ctx) where TResp : MessageRsp
    {
        string json = JsonUtility.ToJson(ctx.Req);
        Debug.Log($"[HTTP] Send request -> {url}\n[HTTP] Body: {json}");

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.certificateHandler = new AcceptAllCertificates();

            yield return webRequest.SendWebRequest();

            Debug.Log($"[HTTP] Response status: {webRequest.result}, HTTP code: {webRequest.responseCode}");

            string responseText = webRequest.downloadHandler?.text;
            Debug.Log($"[HTTP] Response content: {responseText}");

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    var response = JsonHelper.Deserialize<TResp>(responseText);
                    if (response == null)
                    {
                        Debug.LogError($"[HTTP] Empty or invalid API response. URL: {url}");
                        ctx.ErrCode = -1;
                        yield break;
                    }

                    ctx.Resp = response.data;
                    ctx.ErrCode = response.code;
                    ctx.ErrAge  = response.age;
                    if (response.code != 0)
                    {
                        Debug.LogError($"[HTTP] API error: code={response.code}");
                    }
                    else
                    {
                        Debug.Log($"[HTTP] Request success, code=0");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"[HTTP] Response parse failed: {e.Message}\n[HTTP] URL: {url}\n[HTTP] Body: {responseText}");
                    ctx.ErrCode = -1;
                }
            }
            else
            {
                Debug.LogError($"[HTTP] Request failed: {webRequest.error}\n[HTTP] URL: {url}\n[HTTP] Result: {webRequest.result}\n[HTTP] Code: {webRequest.responseCode}\n[HTTP] Body: {responseText}");
                ctx.ErrCode = -1;
            }
        }
    }
}
