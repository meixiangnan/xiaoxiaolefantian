using UnityEngine;

namespace Watermelon
{
    [RegisterModule("Notify Dialog", Core = true)]
    public class NotifyDialogInitModule : InitModule
    {
        [SerializeField] GameObject canvas;

        public override void CreateComponent(Initialiser Initialiser)
        {
            GameObject canvasGameObject = Instantiate(canvas);
            canvasGameObject.transform.SetParent(null);
            canvasGameObject.transform.localScale = Vector3.one;
            canvasGameObject.transform.localPosition = Vector3.zero;
            canvasGameObject.transform.localRotation = Quaternion.identity;
            DontDestroyOnLoad(canvasGameObject);

            Canvas c = canvasGameObject.GetComponent<Canvas>();
            if (c != null)
            {
                c.renderMode = RenderMode.ScreenSpaceOverlay;
                c.sortingOrder = 998;
            }

            UnityEngine.UI.CanvasScaler scaler = canvasGameObject.GetComponent<UnityEngine.UI.CanvasScaler>();
            if (scaler != null)
            {
                scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1080, 1920);
                scaler.matchWidthOrHeight = 1f;
            }

            canvasGameObject.GetComponent<NotifyDialog>().Initialise();
        }

        public NotifyDialogInitModule()
        {
            moduleName = "Notify Dialog";
        }
    }
}