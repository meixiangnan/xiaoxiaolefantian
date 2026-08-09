using System;
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Watermelon.Message
{
    public class JsonHelper
    {
        /// <summary>
        /// 将C#对象转换为JSON字符串
        /// </summary>
        /// <param name="obj">要转换的对象</param>
        /// <returns>JSON字符串</returns>
        public static string ToJson(object obj)
        {
            return JsonUtility.ToJson(obj);
        }

        /// <summary>
        /// 将JSON字符串转换为指定类型的C#对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="json">JSON字符串</param>
        /// <returns>转换后的对象</returns>
        public static T FromJson<T>(string json)
        {
            return JsonUtility.FromJson<T>(json);
        }

        /// <summary>
        /// 将JSON字符串转换为指定类型的C#对象，并填充到现有对象中
        /// </summary>
        /// <param name="json">JSON字符串</param>
        /// <param name="obj">要填充的目标对象</param>
        public static void FromJsonOverwrite(string json, object obj)
        {
            JsonUtility.FromJsonOverwrite(json, obj);
        }

        /// <summary>
        /// 将对象列表转换为JSON字符串
        /// 注意：Unity的JsonUtility不直接支持数组，需要包装
        /// </summary>
        /// <typeparam name="T">列表项的类型</typeparam>
        /// <param name="list">对象列表</param>
        /// <returns>JSON字符串</returns>
        public static string ToJsonArray<T>(List<T> list)
        {
            Wrapper<T> wrapper = new Wrapper<T> { items = list };
            return JsonUtility.ToJson(wrapper);
        }

        /// <summary>
        /// 将JSON字符串转换为对象列表
        /// </summary>
        /// <typeparam name="T">列表项的类型</typeparam>
        /// <param name="json">JSON字符串</param>
        /// <returns>对象列表</returns>
        public static List<T> FromJsonArray<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.items;
        }

        /// <summary>
        /// 用于包装数组/列表的辅助类
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        [System.Serializable]
        private class Wrapper<T>
        {
            public List<T> items;
        }
        
        public static ApiResponse<T> Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<ApiResponse<T>>(json);
        }
        
        // 新增一个反射方法，用于动态类型反序列化
        public static object Deserialize(string json, Type dataType)
        {
            // 构造泛型类型 ApiResponse<T>
            Type responseType = typeof(ApiResponse<>).MakeGenericType(dataType);
            // 调用JsonConvert的DeserializeObject方法，使用反射
            object result = JsonConvert.DeserializeObject(json, responseType);
            return result;
        }
    }
}