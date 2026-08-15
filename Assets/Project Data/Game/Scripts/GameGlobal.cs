using System.Collections;
using UnityEngine;
using Watermelon.GameModule;

namespace Watermelon
{
    public class GameGlobal : MonoBehaviour
    {
        public static GameGlobal Instance;
        private void Awake()
        {
            Instance = this;
        }
        
        GameModuleManager gameModuleManager = new GameModuleManager();
        
        private bool modulesInitialized;

        public void Init()
        {
            gameModuleManager.Init();
            modulesInitialized = true;
        }

        public T GetModule<T>() where T : GameModuleBase
        {
            return gameModuleManager.GetModule<T>();
        }

        public void Update()
        {
            gameModuleManager.TickModule();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus && modulesInitialized)
            {
                gameModuleManager.GetModule<RoleModule>().RequestAntiAddictionSync();
            }
        }

        private bool isUpload = false;
        private bool pendingUpload = false;

        public void UploadRoleData()
        {
            if (isUpload)
            {
                pendingUpload = true;
                return;
            }

            isUpload = true;
            StartCoroutine(UploadRoleDataTask());
        }

        private IEnumerator UploadRoleDataTask()
        {
            do
            {
                pendingUpload = false;
                var mdl = GetModule<RankModule>();
                yield return mdl.UploadRoleData();
            }
            while (pendingUpload);

            isUpload = false;
        }
    }
}
