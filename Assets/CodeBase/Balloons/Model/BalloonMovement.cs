using UnityEngine;

namespace CodeBase.Balloons.Model
{
    public class BalloonMovement : MonoBehaviour
    {
        [SerializeField]
        [Range(0.1f, 3f)]
        public float minSpeed = 0.5f;

        [SerializeField]
        [Range(0.1f, 3f)]
        public float maxSpeed = 2.5f;

        [SerializeField]
        [Range(0.1f, 3f)]
        private float minAmplitude = 0.5f;

        [SerializeField]
        [Range(0.1f, 3f)]
        private float maxAmplitude = 2.5f;

        [SerializeField]
        [Range(0.1f, 3f)]
        private float minFrequency = 0.5f;

        [SerializeField]
        [Range(0.1f, 3f)]
        private float maxFrequency = 2.5f;

        private float _amplitude;
        private float _frequency;
        private float _horizontalSpeed;
        private Vector3 _startPosition;

        public void Init(Vector3 startPosition, int speedDirection)
        {
            transform.position = startPosition;
            _startPosition = startPosition;
            _amplitude = Random.Range(minAmplitude, maxAmplitude);
            _frequency = Random.Range(minFrequency, maxFrequency);
            _horizontalSpeed = Random.Range(minSpeed, maxSpeed) * speedDirection;
        }

        private void Update()
        {
            MoveBalloon();
        }
        
        private void MoveBalloon()
        {
            Vector3 newPosition = transform.position;
            newPosition.x -= _horizontalSpeed * Time.deltaTime; // Движение влево по оси X
            newPosition.y = _startPosition.y + _amplitude * Mathf.Sin(Time.time * _frequency); // Синусоидальное движение по Y
            transform.position = newPosition;
        }
    }
}
