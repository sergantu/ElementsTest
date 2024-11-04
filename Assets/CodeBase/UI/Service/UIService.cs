using CodeBase.Extension;
using CodeBase.Infrastructure.AssetManagement;
using CodeBase.Infrastructure.Factory;
using CodeBase.UI.Component;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CodeBase.UI.Service
{
    public class UIService
    {
        private readonly GameFactory _gameFactory;
        private UIRoot _uiRoot;

        public UIService(GameFactory gameFactory)
        {
            _gameFactory = gameFactory;
        }

        public async UniTask Init()
        {
            GameObject root = await _gameFactory.CreateUIRoot();
            _uiRoot = root.GetComponent<UIRoot>();
        }

        public void ShowLoadingCurtain()
        {
            _uiRoot.ShowLoadingCurtain();
        }

        public void HideLoadingCurtain()
        {
            _uiRoot.HideLoadingCurtain();
        }
    }
}