using UnityEngine;
using UnityEngine.UI;

namespace Watermelon.Map
{
    public class MapLevelBehavior : MapLevelAbstractBehavior
    {
        [SerializeField] Image innerCircle;
        [SerializeField] GameObject outerCircle;
        [SerializeField] GameObject levelLock;

        [Space]
        [SerializeField] Color reachedText;
        [SerializeField] Color reachedCircle;
        [Space]
        [SerializeField] Color openedText;
        [SerializeField] Color openedCircle;
        [Space]
        [SerializeField] Color closedText;
        [SerializeField] Color closedCircle;

        protected override void Awake()
        {
            base.Awake();

            if (outerCircle == null)
            {
                Transform outerCircleTransform = transform.Find("Canvas/Outer Circle");
                if (outerCircleTransform != null)
                {
                    outerCircle = outerCircleTransform.gameObject;
                }
            }

            if (levelLock == null)
            {
                Transform levelLockTransform = transform.Find("Canvas/levelLock");
                if (levelLockTransform != null)
                {
                    levelLock = levelLockTransform.gameObject;
                }
            }
        }

        protected override void InitOpen()
        {
            levelNumber.color = openedText;
            innerCircle.color = openedCircle;

            SetLockState(false);
            button.gameObject.SetActive(true);
        }

        protected override void InitClose() 
        {
            levelNumber.color = closedText;
            innerCircle.color = closedCircle;

            SetLockState(true);
            button.gameObject.SetActive(false);
        }

        protected override void InitCurrent()
        {
            levelNumber.color = reachedText;
            innerCircle.color = reachedCircle;

            SetLockState(true);
            button.gameObject.SetActive(true);
        }

        private void SetLockState(bool locked)
        {
            if (outerCircle != null)
            {
                outerCircle.SetActive(!locked);
            }

            if (levelLock != null)
            {
                levelLock.SetActive(locked);
            }
        }
    }
}
