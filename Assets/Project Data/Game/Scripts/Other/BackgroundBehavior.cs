using UnityEngine;

namespace Watermelon
{ 
    public class BackgroundBehavior : MonoBehaviour
    {
        [SerializeField] SpriteRenderer spriteRenderer;

        private void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            Refresh();
        }

        public void Refresh()
        {
            Camera camera = Camera.main;
            if (camera == null || spriteRenderer == null || spriteRenderer.sprite == null)
            {
                return;
            }

            transform.position = camera.transform.position + camera.transform.forward * (camera.farClipPlane - 0.01f);
            transform.forward = camera.transform.forward;

            var spriteSize = spriteRenderer.sprite.textureRect.size;
            var spriteAspect = spriteSize.x / spriteSize.y;

            var cameraHeight = camera.orthographicSize * 2;
            var cameraWidth = cameraHeight * camera.aspect;

            if (camera.aspect > spriteAspect)
            {
                spriteRenderer.size = new Vector2(cameraWidth, cameraWidth / spriteAspect);
            }
            else
            {
                spriteRenderer.size = new Vector2(cameraHeight * spriteAspect, cameraHeight);
            }
        }
    }
}
