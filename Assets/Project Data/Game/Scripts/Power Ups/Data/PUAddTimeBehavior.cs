using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class PUAddTimeBehavior : PUBehavior
    {
        private PUAddTimeSettings customSettings;

        public override void Initialise()
        {
            customSettings = (PUAddTimeSettings)settings;
        }

        public override bool Activate()
        {
            if(!LevelController.IsBusy)
            {
                LevelController.LevelMaxDuring += 60;
                return true;
            }
            return false;
        }

        public override void ResetBehavior()
        {
            IsBusy = false;
        }
    }
}
