using UnityEngine;

namespace CodeBase.Scaler
{
    public class CameraResizer : MonoBehaviour
    {
        public UnityEngine.Camera mainCamera;

        private void Start()
        {
            float sceneWidth = 8.868f;
            float unitsPerPixel = sceneWidth / Screen.width;
            float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;

            mainCamera.orthographicSize = desiredHalfHeight;
        }
    }
}