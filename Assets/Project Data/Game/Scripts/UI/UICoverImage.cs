using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    /// <summary>
    /// Scales an Image to cover its parent while preserving aspect ratio (crop overflow).
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Image))]
    public class UICoverImage : MonoBehaviour
    {
        private RectTransform rectTransform;
        private RectTransform parentRect;
        private Image image;
        private Vector2 lastParentSize;
        private float lastSpriteAspect = -1f;

        private void Awake()
        {
            CacheRefs();
        }

        private void OnEnable()
        {
            CacheRefs();
            Refresh(force: true);
        }

        private void OnRectTransformDimensionsChange()
        {
            if (!isActiveAndEnabled)
                return;

            Refresh();
        }

        private void LateUpdate()
        {
            Refresh();
        }

        private void CacheRefs()
        {
            if (rectTransform == null)
                rectTransform = transform as RectTransform;

            if (image == null)
                image = GetComponent<Image>();

            if (transform.parent != null)
                parentRect = transform.parent as RectTransform;
        }

        public void Refresh(bool force = false)
        {
            CacheRefs();

            if (rectTransform == null || parentRect == null || image == null || image.sprite == null)
                return;

            Vector2 parentSize = parentRect.rect.size;
            if (parentSize.x <= 0f || parentSize.y <= 0f)
                return;

            float spriteAspect = image.sprite.rect.width / image.sprite.rect.height;
            if (!force && parentSize == lastParentSize && Mathf.Approximately(spriteAspect, lastSpriteAspect))
                return;

            lastParentSize = parentSize;
            lastSpriteAspect = spriteAspect;

            float parentAspect = parentSize.x / parentSize.y;
            float width;
            float height;

            if (parentAspect > spriteAspect)
            {
                width = parentSize.x;
                height = width / spriteAspect;
            }
            else
            {
                height = parentSize.y;
                width = height * spriteAspect;
            }

            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(width, height);

            image.preserveAspect = false;
        }
    }
}
