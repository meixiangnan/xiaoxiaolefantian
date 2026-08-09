using System.IO;
using UnityEditor;
using UnityEngine;

// Menues customization tutorial
// https://hugecalf-studios.github.io/unity-lessons/lessons/editor/menuitem/#:~:text=To%20disable%20a%20menu%20item,return%20false%20from%20this%20method.

namespace Watermelon
{
    public static class CustomActionsMenu
    {

      
      
    }

// 假设你的ScriptableObject有一个PrefabName属性
    public class FixScriptableObjectNames : EditorWindow
    {
        // 添加菜单选项
        [MenuItem("Tools/修复ScriptableObject名称不匹配")]
        public static void ShowWindow()
        {
            // 显示窗口并执行修复操作
            GetWindow<FixScriptableObjectNames>("修复进度");
            FixAllMismatchedNames();
        }
        
        [MenuItem("Tools/将所有LevelData加入LevelData")]
        public static void AddAllLevelData()
        {
            // 显示窗口并执行修复操作
            GetWindow<FixScriptableObjectNames>("搜寻");
            AddAllLevelDataToDataBase();
        }

        public static void AddAllLevelDataToDataBase()
        {
            // 获取项目中所有ScriptableObject类型的资源
            string[] guids = AssetDatabase.FindAssets("t:ScriptableObject");
        
            int fixedCount = 0;
            int totalCount = guids.Length;

            LevelDatabase findDB = null;
            LevelData[] findData = new LevelData[1024];
            int DataCnt = 0;
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                ScriptableObject so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);

                if (so == null) continue;

                if (so is LevelDatabase)
                {
                    findDB = so as LevelDatabase;
                    continue;
                }

                if (so is LevelData)
                {
                    findData[DataCnt++] = so as LevelData;
                    continue;
                }
            }

            foreach (var level in findData)
            {
                if (null == level)
                {
                    return;
                }

                string soName = GetScriptableObjectName(level);
                if (soName.StartsWith("Level_"))
                {
                    var levelId = int.Parse(soName.Substring("Level_".Length));
                    Debug.Log($"已找到: {levelId} 关卡");
                    if (findDB.Levels.Length > (levelId - 1))
                    {
                        findDB.Levels[levelId - 1] = level;    
                    }
                }
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"已找到: {DataCnt} 个LevelData");
        }

        private static void FixAllMismatchedNames()
        {
            // 获取项目中所有ScriptableObject类型的资源
            string[] guids = AssetDatabase.FindAssets("t:ScriptableObject");
        
            int fixedCount = 0;
            int totalCount = guids.Length;

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                ScriptableObject so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            
                if (so == null) continue;

                
                // 使用反射获取PrefabName属性（假设你的ScriptableObject有这个属性）
                string soName = GetScriptableObjectName(so);

                // 检查名称是否匹配
                if (so.name != soName)
                {
                    // 修复名称不匹配的情况
                    so.name = soName;
                    fixedCount++;

                    // 记录修复信息
                    Debug.Log($"已修复: {path} - 从 {so.name} 改为 {soName}");
                }
            }

            // 保存修改
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"修复完成 - 共检查 {totalCount} 个ScriptableObject，修复了 {fixedCount} 个不匹配项");
        }

        public static string GetScriptableObjectName(ScriptableObject so)
        {
            if (so == null)
            {
                Debug.LogError("ScriptableObject 为空！");
                return null;
            }

            // 获取资源在项目中的路径（如：Assets/MyScriptableObject.asset）
            string assetPath = AssetDatabase.GetAssetPath(so);
        
            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogError("该对象不是有效的资源（可能是临时实例）！");
                return null;
            }

            // 从路径中提取文件名（不含扩展名）
            string fileName = Path.GetFileNameWithoutExtension(assetPath);
            return fileName;
        }
        private void OnGUI()
        {
            GUILayout.Label("修复操作已执行", EditorStyles.boldLabel);
            GUILayout.Label("请查看Console窗口获取详细信息");
        
            if (GUILayout.Button("关闭窗口"))
            {
                Close();
            }
        }
    }
}