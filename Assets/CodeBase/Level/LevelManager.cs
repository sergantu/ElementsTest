using System.Collections.Generic;
using System.Xml.Linq;
using CodeBase.Balloons.Service;
using CodeBase.Game.Service;
using CodeBase.Infrastructure.AssetManagement;
using CodeBase.Infrastructure.Factory;
using CodeBase.Input.Service;
using CodeBase.PersistentProgress;
using CodeBase.SaveLoad.Service;
using CodeBase.UI.Service;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeBase.Level
{
    public class LevelManager
    {
        private readonly GameFactory _gameFactory;
        private readonly AssetProvider _assetProvider;
        private readonly LogicService _logicService;
        private readonly SwipeController _swipeController;
        private readonly BalloonSpawner _balloonSpawner;
        private readonly PersistentProgressService _progressService;
        private readonly UIService _uiService;
        private readonly SaveLoadService _saveLoadService;

        private readonly List<string> _levels = new List<string>();
        private Game.Model.Level _currentLevel;

        public LevelManager(GameFactory gameFactory,
                            AssetProvider assetProvider,
                            LogicService logicService,
                            SwipeController swipeController,
                            BalloonSpawner balloonSpawner,
                            PersistentProgressService progressService,
                            UIService uiService,
                            SaveLoadService saveLoadService)
        {
            _saveLoadService = saveLoadService;
            _gameFactory = gameFactory;
            _assetProvider = assetProvider;
            _logicService = logicService;
            _swipeController = swipeController;
            _balloonSpawner = balloonSpawner;
            _progressService = progressService;
            _uiService = uiService;
        }

        public async UniTask Init()
        {
            string text = await _assetProvider.LoadTextAsset(AssetAddress.LevelsXml);
            if (string.IsNullOrEmpty(text)) {
                Debug.LogError("Empty levels.xml");
            }
            XDocument xDoc = XDocument.Parse(text);
            foreach (var level in xDoc.Descendants("Level")) {
                _levels.Add(level.Value);
            }

            _logicService.OnWinning += () => { LoadNextLevel().Forget(); };
        }

        public async UniTask LoadNextLevel()
        {
            _logicService.StopGame();
            _balloonSpawner.Clear();
            Object.Destroy(_currentLevel.gameObject);
            _progressService.Progress.GameData.CurrentLevel++;
            if (_progressService.Progress.GameData.CurrentLevel >= _levels.Count) {
                _progressService.Progress.GameData.CurrentLevel = 0;
            }
            _progressService.Progress.GameData.ElementsField = null;
            _saveLoadService.SaveProgress();
            
            await LoadLevel();
        }

        public async UniTask LoadLevel()
        {
            _swipeController.Lock();
            _uiService.ShowLoadingCurtain();
            GameObject levelObj = await _gameFactory.CreateLevel(_levels[_progressService.Progress.GameData.CurrentLevel]);
            _currentLevel = levelObj.GetComponent<Game.Model.Level>();
            await _logicService.InitLevel(_currentLevel);
            await _balloonSpawner.Init(_currentLevel.BalloonContainer);
            _balloonSpawner.SpawnBalloon();
            _swipeController.Unlock();
            _uiService.HideLoadingCurtain();
        }

        public async UniTask RestartLevel()
        {
            _logicService.StopGame();
            _uiService.ShowLoadingCurtain();
            _swipeController.Lock();
            await _logicService.RestartLevel();
            _swipeController.Unlock();
            _uiService.HideLoadingCurtain();
        }
    }
}