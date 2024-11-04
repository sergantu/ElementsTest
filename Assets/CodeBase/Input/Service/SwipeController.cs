using CodeBase.Game.Service;
using CodeBase.Input.Model;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;

namespace CodeBase.Input.Service
{
    public class SwipeController : ITickable
    {
        private readonly LogicService _logicService;
        private readonly UnityEngine.Camera _mainCamera;

        private Vector2Int _startSwipePosition;

        private bool _isSwiping;
        private SwipeState _swipeState = SwipeState.Blocked;

        public SwipeController(LogicService logicService)
        {
            _logicService = logicService;
            _mainCamera = UnityEngine.Camera.main;
            _swipeState = SwipeState.Blocked;
        }

        public void Tick()
        {
            if (!CanSwipe()) {
                return;
            }

            if (Application.isMobilePlatform) {
                DetectSwipeMobile();
            } else {
                DetectSwipePC();
            }
        }

        public void Lock()
        {
            _swipeState = SwipeState.Blocked;
        }

        public void Unlock()
        {
            _swipeState = SwipeState.Ready;
        }

        private bool CanSwipe() => _swipeState == SwipeState.Ready;

        private void DetectSwipeMobile()
        {
            if (UnityEngine.Input.touchCount == 0) {
                return;
            }

            Touch touch = UnityEngine.Input.GetTouch(0);
            Vector2Int tilePosition = GetTilePositionFromScreen(touch.position);

            switch (touch.phase) {
                case TouchPhase.Began:
                    TryStartSwipe(tilePosition);
                    break;
                case TouchPhase.Ended:
                    if (_isSwiping) {
                        HandleSwipe(tilePosition).Forget();
                    }
                    break;
            }
        }

        private void DetectSwipePC()
        {
            if (UnityEngine.Input.GetMouseButtonDown(0)) {
                Vector2Int tilePosition = GetTilePositionFromScreen(UnityEngine.Input.mousePosition);
                TryStartSwipe(tilePosition);
            } else if (UnityEngine.Input.GetMouseButtonUp(0) && _isSwiping) {
                Vector2Int tilePosition = GetTilePositionFromScreen(UnityEngine.Input.mousePosition);
                HandleSwipe(tilePosition).Forget();
            }
        }

        private Vector2Int GetTilePositionFromScreen(Vector3 screenPosition)
        {
            Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(screenPosition);
            Vector3Int gridPosition = _logicService.GetTilemap.WorldToCell(worldPosition);
            return (Vector2Int) gridPosition;
        }

        private void TryStartSwipe(Vector2Int tilePosition)
        {
            if (!_logicService.IsTileAt(tilePosition)) {
                return;
            }
            _startSwipePosition = tilePosition;
            _isSwiping = true;
        }

        private async UniTaskVoid HandleSwipe(Vector2Int endSwipePosition)
        {
            Vector2Int direction = endSwipePosition - _startSwipePosition;
            if (direction.sqrMagnitude > 0) {
                Vector2Int targetPosition = _startSwipePosition + GetSwipeDirection(direction);
                await ExecuteSwipe(_startSwipePosition, targetPosition);
            }
            _isSwiping = false;
        }

        private Vector2Int GetSwipeDirection(Vector2Int direction)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) {
                return new Vector2Int(direction.x > 0 ? 1 : -1, 0);
            }

            if (direction.y > 0 && _logicService.HasTileAbove(_startSwipePosition)) {
                return Vector2Int.up;
            }

            return new Vector2Int(0, direction.y > 0 ? 1 : -1);
        }

        private async UniTask ExecuteSwipe(Vector2Int from, Vector2Int to)
        {
            Lock();
            await _logicService.TrySwipe(from, to);
            Unlock();
        }
    }
}