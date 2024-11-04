using System;
using CodeBase.Infrastructure.Factory;
using CodeBase.Level;
using CodeBase.PersistentProgress;
using CodeBase.SaveLoad.Service;
using CodeBase.UI.Service;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CodeBase.Infrastructure.States
{
    public class LoadLevelState : IState
    {
        private readonly GameFactory _gameFactory;
        private readonly UIService _uiService;
        private readonly GameStateMachine _stateMachine;
        private readonly LevelManager _levelManager;

        public LoadLevelState(GameFactory gameFactory, UIService uiService, GameStateMachine stateMachine, LevelManager levelManager)
        {
            _gameFactory = gameFactory;
            _uiService = uiService;
            _stateMachine = stateMachine;
            _levelManager = levelManager;
        }

        public async UniTaskVoid Enter()
        {
            _gameFactory.Cleanup();
            try {
                OnLoaded().Forget();
            } catch (Exception e) {
                Debug.LogError("Enter LoadLevelState1 error " + e.Message);
            }
        }

        public async UniTaskVoid Exit()
        {
            _uiService.HideLoadingCurtain();
        }

        private async UniTaskVoid OnLoaded()
        {
            try {
                await InitLevelManager();
                await _levelManager.LoadLevel();
            } catch (Exception e) {
                Debug.LogError("LoadLevelState1 error " + e.Message);
            }

            _stateMachine.Enter<GameLoopState>().Forget();
        }

        private async UniTask InitLevelManager()
        {
            await _levelManager.Init();
        }
    }
}