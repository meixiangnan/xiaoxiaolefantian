using System;
using System.Collections.Generic;
using System.Reflection;

namespace Watermelon.GameModule
{
    public class GameModuleBase
    {
        protected GameModuleManager moduleManager;
        
        public virtual void Init(GameModuleManager mngr)
        {
            moduleManager = mngr;
        }
        
        public T GetModule<T>() where T : GameModuleBase
        {
            return moduleManager.GetModule<T>();
        }

        public virtual void TickModule()
        {
        }
    }
    
    public class GameModuleManager
    {
        private Dictionary<Type, GameModuleBase> Modules = new Dictionary<Type, GameModuleBase>();
        public void AddModule(GameModuleBase module)
        {
            Modules.Add(module.GetType(), module);
        }
        
        public T GetModule<T>() where T : GameModuleBase
        {
            return (T)Modules[typeof(T)];
        }

        public void Init()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                if (type.IsSubclassOf(typeof(GameModuleBase)))
                {
                    GameModuleBase module = (GameModuleBase)Activator.CreateInstance(type);
                    module.Init(this);
                    AddModule(module);
                }
            }
        }

        public void TickModule()
        {
            foreach (var kv in Modules)
            {
                kv.Value.TickModule();
            }
        }

    }
}