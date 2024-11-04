using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace CodeBase.Utils
{
    public class FullAreaResizerComponent : MonoBehaviour
    {
        private const double TOLERANCE = 1;
        private Vector2 _resolution;

        private void Start()
        {
            SaveResolution();
            ResizeToScreenHeight();
        }

        private void Update()
        {
#if !UNITY_EDITOR
                return;
#endif
            CheckResolutionChanged();
        }

        private void CheckResolutionChanged()
        {
            int screenWidth = Screen.width;
            int screenHeight = Screen.height;

            if (Math.Abs(_resolution.x - screenWidth) < TOLERANCE && Math.Abs(_resolution.y - screenHeight) < TOLERANCE) {
                return;
            }
            ResizeToScreenHeight();
            SaveResolution();
        }

        private void SaveResolution()
        {
            _resolution = new Vector2(Screen.width, Screen.height);
        }

        private void ResizeToScreenHeight()
        {
            var rectTransform = gameObject.GetComponent<RectTransform>();
            if (rectTransform == null) {
                return;
            }

            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            Rect rect = rectTransform.rect;
            float localWidth = rect.width;
            float localHeight = rect.height;
            
            float screenAspect = screenWidth / screenHeight;
            float localAspect = localWidth / localHeight;

            float uiHeightScale = GetUIHeightScale();

            float heightResizeCoefficient = screenHeight / localHeight;
            float widthResizeCoefficient = screenWidth / localWidth;

            float resizeCoefficient = heightResizeCoefficient > widthResizeCoefficient ? heightResizeCoefficient : widthResizeCoefficient;

            resizeCoefficient /= uiHeightScale;
            
            if (screenWidth < localWidth) {
                resizeCoefficient += localAspect - screenAspect;
            }
            
            rectTransform.localScale = new Vector3(resizeCoefficient, resizeCoefficient, 1F);
        }

        private float GetUIHeightScale()
        {
            RectTransform uiRectTransform = GetUIRectTransform();
            return uiRectTransform == null ? 1F : uiRectTransform.localScale.y;
        }

        [NotNull]
        private RectTransform GetUIRectTransform()
        {
            RectTransform[] parentRectTransforms = GetComponentsInParent<RectTransform>();
            return parentRectTransforms.First(rt => rt.gameObject.name == "UIRoot(Clone)");
        }
    }
}