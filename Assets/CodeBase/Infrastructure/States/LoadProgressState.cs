using CodeBase.PersistentProgress;
using CodeBase.SaveLoad.Service;
using Cysharp.Threading.Tasks;

namespace CodeBase.Infrastructure.States
{
    public class LoadProgressState : IState
    {
        private readonly GameStateMachine _stateMachine;
        private readonly PersistentProgressService _progressService;
        private readonly SaveLoadService _saveLoadService;

        public LoadProgressState(GameStateMachine stateMachine, PersistentProgressService progressService, SaveLoadService saveLoadService)
        {
            _stateMachine = stateMachine;
            _progressService = progressService;
            _saveLoadService = saveLoadService;
        }

        public async UniTaskVoid Enter()
        {
            await LoadProgressOrInitNew();
            _stateMachine.Enter<LoadLevelState>().Forget();
        }

        public async UniTaskVoid Exit()
        {
        }

        private async UniTask LoadProgressOrInitNew()
        {
            var playerProgress = await _saveLoadService.LoadProgress();
            _progressService.Progress = playerProgress ?? NewProgress();
            _saveLoadService.SaveProgress();
        }

        private PlayerProgress NewProgress()
        {
            var progress = new PlayerProgress();
            return progress;
        }
    }
}