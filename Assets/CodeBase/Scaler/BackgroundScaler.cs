namespace CodeBase.Scaler
{
    using UnityEngine;

    public class BackgroundScaler : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        private void Start()
        {
            Camera mainCamera = Camera.main;
            float screenHeight = mainCamera.orthographicSize * 2;
            float screenWidth = screenHeight * mainCamera.aspect;

            Vector2 spriteSize = _spriteRenderer.sprite.bounds.size;
            float scaleX = screenWidth / spriteSize.x;
            float scaleY = screenHeight / spriteSize.y;
            float scale = Mathf.Max(scaleX, scaleY);

            transform.localScale = new Vector3(scale, scale, 1);
        }
    }
}