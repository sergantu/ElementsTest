using CodeBase.Infrastructure.AssetManagement;
using CodeBase.UI.Service;
using Cysharp.Threading.Tasks;

namespace CodeBase.Infrastructure.States
{
    public class BootstrapState : IState
    {
        private readonly AssetProvider _assetProvider;
        private readonly GameStateMachine _stateMachine;
        private readonly UIService _uiService;

        public BootstrapState(AssetProvider assetProvider, GameStateMachine stateMachine, UIService uiService)
        {
            _assetProvider = assetProvider;
            _stateMachine = stateMachine;
            _uiService = uiService;
        }

        public async UniTaskVoid Enter()
        {
            EnterLoadLevel().Forget();
        }

        public async UniTaskVoid Exit()
        {
        }

        private async UniTaskVoid EnterLoadLevel()
        {
            _assetProvider.Construct();
            await InitUIRoot();
            _uiService.ShowLoadingCurtain();
            _stateMachine.Enter<LoadProgressState>().Forget();
        }
        
        private async UniTask InitUIRoot() => await _uiService.Init();
    }
}