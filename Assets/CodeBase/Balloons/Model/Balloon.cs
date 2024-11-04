using CodeBase.Balloons.Service;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

namespace CodeBase.Balloons.Model
{
    public class Balloon : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        [SerializeField]
        private BalloonMovement _balloonMovement;

        [Inject]
        private BalloonSpawner _spawner;
        
        private float _leftXSpawn;
        private float _rightXSpawn;
        private float _minYSpawn;
        private float _maxYSpawn;

        private bool _isRightSide;

        public void Initialize(float orthographicSize, float screenWidth)
        {
            Vector2 objectSize = _spriteRenderer.bounds.size;
            _leftXSpawn = (-screenWidth / 2 - objectSize.x / 2);
            _rightXSpawn = (screenWidth / 2 + objectSize.x / 2);

            _minYSpawn = -orthographicSize + (objectSize.y / 2);
            _maxYSpawn = orthographicSize - (objectSize.y / 2);
        }

        private void Update()
        {
            CheckIfOutOfView();
        }

        public void Create()
        {
            _isRightSide = Random.value > 0.5f;
            float x = _isRightSide ? _rightXSpawn : _leftXSpawn;
            float y = Random.Range(_minYSpawn, _maxYSpawn);
            int speedDirection = _isRightSide ? 1 : -1;
            _balloonMovement.Init(new Vector3(x, y, 0), speedDirection);
            gameObject.SetActive(true);
        }

        private void CheckIfOutOfView()
        {
            if ((_isRightSide && transform.position.x < _leftXSpawn) || (!_isRightSide && transform.position.x > _rightXSpawn)) {
                DeactivateBalloon();
            }
        }

        private void DeactivateBalloon()
        {
            gameObject.SetActive(false);
            _spawner.OnBalloonDeactivated(this);
        }
    }
}