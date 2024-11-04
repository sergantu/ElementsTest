using System.Collections.Generic;
using CodeBase.Balloons.Model;
using CodeBase.Infrastructure.AssetManagement;
using CodeBase.Infrastructure.Factory;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CodeBase.Balloons.Service
{
    public class BalloonSpawner
    {
        private const int PoolSize = 3;

        private readonly GameFactory _gameFactory;

        private List<Balloon> _balloonPool = new();
        private readonly float _screenWidth = 1080;

        public BalloonSpawner(GameFactory gameFactory)
        {
            _gameFactory = gameFactory;
            float screenHeight = UnityEngine.Camera.main.orthographicSize * 2;
            _screenWidth = screenHeight * UnityEngine.Camera.main.aspect;
        }

        public async UniTask Init(Transform container)
        {
            for (int i = 0; i < PoolSize; i++) {
                string path = Random.value > 0.5f ? AssetAddress.Balloon1 : AssetAddress.Balloon2;
                GameObject balloonObj = await _gameFactory.CreateBalloon(path, container);
                var balloon = balloonObj.GetComponent<Balloon>();
                if (UnityEngine.Camera.main != null) {
                    balloon.Initialize(UnityEngine.Camera.main.orthographicSize, _screenWidth);
                }
                balloonObj.gameObject.SetActive(false);
                _balloonPool.Add(balloon);
            }
        }

        public void Clear()
        {
            _balloonPool = new List<Balloon>();
        }

        public void SpawnBalloon()
        {
            foreach (Balloon balloon in _balloonPool) {
                if (balloon == null) {
                    return;
                }
                if (!balloon.gameObject.activeInHierarchy) {
                    balloon.Create();
                }
            }
        }

        public void OnBalloonDeactivated(Balloon balloon)
        {
            balloon.Create();
        }
    }
}